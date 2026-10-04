using System.Reflection;
using System.Security.Claims;
using FluentValidation;
using HR.API.Controllers;
using HR.Application.Common.Security;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Application.Services;
using HR.Domain.Entity;
using HR.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HR.Tests;

public class ManagerAuthorizationTests
{
    static readonly LeaveRequestDto Own = Request(1, 10);
    static readonly LeaveRequestDto Other = Request(2, 20);
    static readonly LeaveRequestDto Self = Request(3, 5);
    static LeaveRequestDto Request(long id, long employee) => new(id, employee, 1, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1, null, null, LeaveRequestStatus.Pending, DateTime.UtcNow);

    static T Proxy<T>(Func<MethodInfo, object?[], object?> handler) where T : class
    {
        var value = DispatchProxy.Create<T, Handler>();
        ((Handler)(object)value).InvokeHandler = handler;
        return value;
    }
    public class Handler : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> InvokeHandler = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => InvokeHandler(method!, args ?? []);
    }
    static ICurrentUserService User(long? employeeId) => Proxy<ICurrentUserService>((m, _) => m.Name == "GetEmployeeIdAsync" ? Task.FromResult(employeeId) : "user");
    static IManagerTeamService Teams(params long[] ids) => Proxy<IManagerTeamService>((m, a) => m.Name == "GetTeamEmployeeIdsAsync"
        ? Task.FromResult<IReadOnlyList<long>>(ids)
        : Task.FromResult(ids.Contains((long)a[1]!)));
    static ILeaveRequestService Requests() => Proxy<ILeaveRequestService>((m, a) => m.Name switch
    {
        "GetAllAsync" => Task.FromResult(new List<LeaveRequestDto> { Own, Other, Self }),
        "GetByIdAsync" => Task.FromResult<LeaveRequestDto?>(new[] { Own, Other, Self }.FirstOrDefault(x => x.Id == (long)a[0]!)),
        _ => throw new NotSupportedException(m.Name)
    });
    static LeaveRequestsController Controller(ILeaveRequestService service, string role)
    {
        var controller = new LeaveRequestsController(service, new InlineValidator<CreateLeaveRequestDto>(), new InlineValidator<UpdateLeaveRequestDto>());
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, role)], "test")) } };
        return controller;
    }
    static LeaveRequestApprovalsController Approvals(string role, LeaveRequestApprovalDto approval)
    {
        var service = Proxy<ILeaveRequestApprovalService>((m, a) => m.Name switch
        {
            "GetAllAsync" => Task.FromResult(new List<LeaveRequestApprovalDto> { approval }),
            "GetByIdAsync" => Task.FromResult<LeaveRequestApprovalDto?>(approval.Id == (long)a[0]! ? approval : null),
            "CreateAsync" => Task.FromResult(approval),
            "UpdateAsync" => Task.FromResult(true),
            _ => throw new NotSupportedException(m.Name)
        });
        var controller = new LeaveRequestApprovalsController(service, new InlineValidator<CreateLeaveRequestApprovalDto>(), new InlineValidator<UpdateLeaveRequestApprovalDto>());
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, role)], "test")) } };
        return controller;
    }

    [Fact]
    public async Task Manager_list_and_changed_id_are_scoped_to_own_team()
    {
        var controller = Controller(Requests(), AppRoles.Manager);
        var list = await controller.GetAll(User(5), Teams(10), default);
        Assert.Equal([1L], Assert.IsType<List<LeaveRequestDto>>(Assert.IsType<OkObjectResult>(list.Result).Value).Select(x => x.Id));
        Assert.IsType<OkObjectResult>((await controller.GetById(1, User(5), Teams(10), default)).Result);
        Assert.IsType<NotFoundResult>((await controller.GetById(2, User(5), Teams(10), default)).Result);
        Assert.IsType<NotFoundResult>((await controller.GetById(3, User(5), Teams(10), default)).Result);
    }

    [Fact]
    public async Task Missing_link_or_team_grants_no_records()
    {
        var controller = Controller(Requests(), AppRoles.Manager);
        foreach (var user in new[] { User(null), User(5) })
        {
            var list = await controller.GetAll(user, Teams(), default);
            Assert.Empty(Assert.IsType<List<LeaveRequestDto>>(Assert.IsType<OkObjectResult>(list.Result).Value));
            Assert.IsType<NotFoundResult>((await controller.GetById(1, user, Teams(), default)).Result);
        }
    }

    [Theory]
    [InlineData(AppRoles.Admin)]
    [InlineData(AppRoles.HR)]
    public async Task Admin_and_hr_retain_full_access(string role)
    {
        var controller = Controller(Requests(), role);
        var list = await controller.GetAll(User(null), Teams(), default);
        Assert.Equal(3, Assert.IsType<List<LeaveRequestDto>>(Assert.IsType<OkObjectResult>(list.Result).Value).Count);
        Assert.IsType<OkObjectResult>((await controller.GetById(2, User(null), Teams(), default)).Result);
    }

    [Fact]
    public async Task Manager_cannot_approve_self_or_another_team_or_missing_link()
    {
        var approval = new LeaveRequestApprovalDto(7, 1, 1, 5, ApprovalAction.Pending, null, null);
        var controller = Approvals(AppRoles.Manager, approval);
        var update = new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null);
        Assert.IsType<NotFoundResult>(await controller.Create(new CreateLeaveRequestApprovalDto(3, 1, 5, ApprovalAction.Pending, null), User(5), Teams(10), Requests(), default).ContinueWith(t => t.Result.Result));
        Assert.IsType<NotFoundResult>(await controller.Create(new CreateLeaveRequestApprovalDto(2, 1, 5, ApprovalAction.Pending, null), User(5), Teams(10), Requests(), default).ContinueWith(t => t.Result.Result));
        Assert.IsType<ForbidResult>(await controller.Update(7, update, User(null), Teams(10), Requests(), default));
        Assert.IsType<NotFoundResult>(await controller.Update(7, update, User(5), Teams(20), Requests(), default));
    }

    [Fact]
    public async Task Manager_can_action_own_report_but_cannot_action_someone_elses_approval()
    {
        var approval = new LeaveRequestApprovalDto(7, 1, 1, 5, ApprovalAction.Pending, null, null);
        var controller = Approvals(AppRoles.Manager, approval);
        Assert.IsType<NoContentResult>(await controller.Update(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null), User(5), Teams(10), Requests(), default));
        Assert.IsType<NotFoundResult>(await controller.Update(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Rejected, null), User(6), Teams(10), Requests(), default));
    }

    [Theory]
    [InlineData(AppRoles.Admin)]
    [InlineData(AppRoles.HR)]
    public async Task Admin_and_hr_can_action_approval_without_team_link(string role)
    {
        var approval = new LeaveRequestApprovalDto(7, 2, 1, 5, ApprovalAction.Pending, null, null);
        var controller = Approvals(role, approval);
        Assert.IsType<NoContentResult>(await controller.Update(7, new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null), User(null), Teams(), Requests(), default));
    }

    [Fact]
    public async Task Historical_assignments_are_excluded_by_team_service()
    {
        var assignments = new[] { new EmployeeAssignment { EmployeeId = 10, ManagerEmployeeId = 5, IsCurrent = true }, new EmployeeAssignment { EmployeeId = 20, ManagerEmployeeId = 5, IsCurrent = false } };
        var repo = Proxy<IEmployeeAssignmentRepository>((m, a) => m.Name switch
        {
            "GetCurrentDirectReportIdsAsync" => Task.FromResult(assignments.Where(x => x.IsCurrent && x.ManagerEmployeeId == (long)a[0]!).Select(x => x.EmployeeId).ToList()),
            "IsCurrentDirectReportAsync" => Task.FromResult(assignments.Any(x => x.IsCurrent && x.ManagerEmployeeId == (long)a[0]! && x.EmployeeId == (long)a[1]!)),
            _ => throw new NotSupportedException(m.Name)
        });
        var teams = new ManagerTeamService(repo);
        Assert.Equal([10L], await teams.GetTeamEmployeeIdsAsync(5));
        Assert.False(await teams.IsEmployeeInTeamAsync(5, 20));
    }

    [Fact]
    public async Task Manager_cannot_action_request_after_direct_report_assignment_ends()
    {
        var current = false;
        var repo = Proxy<IEmployeeAssignmentRepository>((method, args) => method.Name switch
        {
            "IsCurrentDirectReportAsync" => Task.FromResult(current && (long)args[0]! == 5 && (long)args[1]! == 10),
            _ => throw new NotSupportedException(method.Name)
        });
        var controller = Approvals(AppRoles.Manager,
            new LeaveRequestApprovalDto(7, 1, 1, 5, ApprovalAction.Pending, null, null));
        var team = new ManagerTeamService(repo);
        Assert.IsType<NotFoundResult>(await controller.Update(7,
            new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null),
            User(5), team, Requests(), default));
        current = true;
        Assert.IsType<NoContentResult>(await controller.Update(7,
            new UpdateLeaveRequestApprovalDto(ApprovalAction.Approved, null),
            User(5), team, Requests(), default));
    }
}
