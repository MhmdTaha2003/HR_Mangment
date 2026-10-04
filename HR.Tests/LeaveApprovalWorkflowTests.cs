using System.Reflection;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.Interfaces.Repositories;
using HR.Application.Services;
using HR.Domain.Entity;
using HR.Domain.Enums;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HR.Tests;

public class LeaveApprovalWorkflowTests
{
    public class RepositoryProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> Handler = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
    }

    static T Proxy<T>(Func<MethodInfo, object?[], object?> handler) where T : class
    {
        var value = DispatchProxy.Create<T, RepositoryProxy>();
        ((RepositoryProxy)(object)value).Handler = handler;
        return value;
    }

    sealed class Fixture
    {
        public readonly LeaveRequest Request = new()
        {
            Id = 1, EmployeeId = 11, LeaveTypeId = 2,
            StartDate = new DateTime(2026, 10, 1), RequestedDays = 3,
            Status = LeaveRequestStatus.Pending
        };
        public readonly LeaveRequestApproval Approval = new()
        {
            Id = 7, LeaveRequestId = 1, ApprovalLevel = 1,
            ApproverEmployeeId = 5, Action = ApprovalAction.Pending
        };
        public readonly EmployeeLeaveBalance Balance = new()
        {
            EmployeeId = 11, LeaveTypeId = 2, Year = 2026,
            EntitledDays = 20, UsedDays = 2
        };
        public int SaveCount;
        public int AddCount;
        public LeaveRequestApprovalService Service()
        {
            var approvals = Proxy<ILeaveRequestApprovalRepository>((method, args) => method.Name switch
            {
                "GetTrackedByIdAsync" => Task.FromResult<LeaveRequestApproval?>(Approval),
                "GetByIdAsync" => Task.FromResult<LeaveRequestApproval?>(Approval),
                "LevelExistsAsync" => Task.FromResult(AddCount > 0),
                "AddAsync" => Add(),
                "SaveChangesAsync" => Save(),
                _ => throw new NotSupportedException(method.Name)
            });
            var requests = Proxy<ILeaveRequestRepository>((method, _) => method.Name switch
            {
                "GetByIdAsync" or "GetTrackedByIdAsync" => Task.FromResult<LeaveRequest?>(Request),
                _ => throw new NotSupportedException(method.Name)
            });
            var employees = Proxy<IEmployeeRepository>((method, _) => method.Name switch
            {
                "ExistsAsync" => Task.FromResult(true),
                _ => throw new NotSupportedException(method.Name)
            });
            var balances = Proxy<IEmployeeLeaveBalanceRepository>((method, _) => method.Name switch
            {
                "GetTrackedByKeyAsync" => Task.FromResult<EmployeeLeaveBalance?>(Balance),
                _ => throw new NotSupportedException(method.Name)
            });
            return new LeaveRequestApprovalService(approvals, requests, employees, balances);
        }
        Task Add() { AddCount++; return Task.CompletedTask; }
        Task Save() { SaveCount++; return Task.CompletedTask; }
    }

    [Fact]
    public async Task Approval_deducts_once_and_replay_cannot_create_or_action_again()
    {
        var state = new Fixture();
        var service = state.Service();
        Assert.True(await service.UpdateAsync(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, "yes"), default));
        Assert.Equal(LeaveRequestStatus.Approved, state.Request.Status);
        Assert.Equal(ApprovalAction.Approved, state.Approval.Action);
        Assert.Equal(5, state.Balance.UsedDays);
        Assert.Equal(1, state.SaveCount);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null), default));
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreateLeaveRequestApprovalDto(1, 1, 5, ApprovalAction.Pending, null), default));
        Assert.Equal(5, state.Balance.UsedDays);
        Assert.Equal(1, state.SaveCount);
        Assert.Equal(0, state.AddCount);
    }

    [Fact]
    public async Task Rejection_changes_status_without_touching_balance_and_cannot_repeat()
    {
        var state = new Fixture();
        var service = state.Service();
        Assert.True(await service.UpdateAsync(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Rejected, "no"), default));
        Assert.Equal(LeaveRequestStatus.Rejected, state.Request.Status);
        Assert.Equal(ApprovalAction.Rejected, state.Approval.Action);
        Assert.Equal(2, state.Balance.UsedDays);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Rejected, null), default));
        Assert.Equal(1, state.SaveCount);
    }

    [Fact]
    public async Task Pending_approval_creation_does_not_change_request_or_balance_and_duplicate_level_is_blocked()
    {
        var state = new Fixture();
        var service = state.Service();
        await service.CreateAsync(new CreateLeaveRequestApprovalDto(1, 1, 5, ApprovalAction.Pending, null), default);
        Assert.Equal(LeaveRequestStatus.Pending, state.Request.Status);
        Assert.Equal(2, state.Balance.UsedDays);
        Assert.Equal(1, state.AddCount);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreateLeaveRequestApprovalDto(1, 1, 5, ApprovalAction.Pending, null), default));
        Assert.Equal(1, state.AddCount);
    }

    [Fact]
    public void Existing_status_and_used_days_columns_are_concurrency_tokens()
    {
        using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=.;Database=HrWorkflowModelOnly;Trusted_Connection=True;TrustServerCertificate=True")
            .Options);
        Assert.True(context.Model.FindEntityType(typeof(LeaveRequest))!.FindProperty(nameof(LeaveRequest.Status))!.IsConcurrencyToken);
        Assert.True(context.Model.FindEntityType(typeof(EmployeeLeaveBalance))!.FindProperty(nameof(EmployeeLeaveBalance.UsedDays))!.IsConcurrencyToken);
    }
}
