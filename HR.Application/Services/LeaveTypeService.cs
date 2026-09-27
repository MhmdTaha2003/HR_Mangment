using HR.Application.DTOs.Departments;
using HR.Application.DTOs.JobTitles;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class LeaveTypeService : ILeaveTypeService
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public LeaveTypeService(
        ILeaveTypeRepository leaveTypeRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;
    }

    public async Task<List<LeaveTypeDto>> GetAllAsync(CancellationToken cancellationToken) => (await _leaveTypeRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<LeaveTypeDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(id, cancellationToken);
        return leaveType is null ? null : Map(leaveType);
    }

    public async Task<LeaveTypeDto> CreateAsync(CreateLeaveTypeDto dto, CancellationToken cancellationToken)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await _leaveTypeRepository.CodeExistsAsync(normalizedCode, cancellationToken))
            throw new InvalidOperationException("Leave type code already exists.");
        var leaveType = new LeaveType
        {
            Code = normalizedCode,
            NameEn = dto.NameEn.Trim(),
            NameAr = dto.NameAr.Trim(),
            DefaultDays = dto.DefaultDays,
            IsPaid = dto.IsPaid,
            RequiresApproval = dto.RequiresApproval,
            RequiresAttachment = dto.RequiresAttachment,
            IsActive = true
        };
        await _leaveTypeRepository.AddAsync(leaveType, cancellationToken);
        await _leaveTypeRepository.SaveChangesAsync(cancellationToken);
        return Map(leaveType);
    }

    public async Task<bool> UpdateAsync(long id, UpdateLeaveTypeDto dto, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (leaveType is null)
            return false;
        leaveType.NameEn = dto.NameEn.Trim();
        leaveType.NameAr = dto.NameAr.Trim();
        leaveType.DefaultDays = dto.DefaultDays;
        leaveType.IsPaid = dto.IsPaid;
        leaveType.RequiresApproval = dto.RequiresApproval;
        leaveType.RequiresAttachment = dto.RequiresAttachment;
        leaveType.IsActive = dto.IsActive;
        leaveType.UpdatedAt = DateTime.UtcNow;
        await _leaveTypeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (leaveType is null)
            return false;
        leaveType.IsActive = false;
        leaveType.UpdatedAt = DateTime.UtcNow;
        await _leaveTypeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static LeaveTypeDto Map(LeaveType leaveType) => new(leaveType.Id, leaveType.Code, leaveType.NameEn, leaveType.NameAr, leaveType.DefaultDays, leaveType.IsPaid, leaveType.RequiresApproval, leaveType.RequiresAttachment, leaveType.IsActive);
}

