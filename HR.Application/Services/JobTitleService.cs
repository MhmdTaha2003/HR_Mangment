using HR.Application.DTOs.Departments;
using HR.Application.DTOs.JobTitles;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class JobTitleService : IJobTitleService
{
    private readonly IJobTitleRepository _jobTitleRepository;

    public JobTitleService(
        IJobTitleRepository jobTitleRepository)
    {
        _jobTitleRepository = jobTitleRepository;
    }

    public async Task<List<JobTitleDto>> GetAllAsync(CancellationToken cancellationToken) => (await _jobTitleRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<JobTitleDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var jobTitle = await _jobTitleRepository.GetByIdAsync(id, cancellationToken);
        return jobTitle is null ? null : Map(jobTitle);
    }

    public async Task<JobTitleDto> CreateAsync(CreateJobTitleDto dto, CancellationToken cancellationToken)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await _jobTitleRepository.CodeExistsAsync(normalizedCode, cancellationToken))
            throw new InvalidOperationException("Job title code already exists.");
        var jobTitle = new JobTitle
        {
            Code = normalizedCode,
            NameEn = dto.NameEn.Trim(),
            NameAr = dto.NameAr.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true
        };
        await _jobTitleRepository.AddAsync(jobTitle, cancellationToken);
        await _jobTitleRepository.SaveChangesAsync(cancellationToken);
        return Map(jobTitle);
    }

    public async Task<bool> UpdateAsync(long id, UpdateJobTitleDto dto, CancellationToken cancellationToken)
    {
        var jobTitle = await _jobTitleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (jobTitle is null)
            return false;
        jobTitle.NameEn = dto.NameEn.Trim();
        jobTitle.NameAr = dto.NameAr.Trim();
        jobTitle.Description = dto.Description?.Trim();
        jobTitle.IsActive = dto.IsActive;
        jobTitle.UpdatedAt = DateTime.UtcNow;
        await _jobTitleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var jobTitle = await _jobTitleRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (jobTitle is null)
            return false;
        jobTitle.IsActive = false;
        jobTitle.UpdatedAt = DateTime.UtcNow;
        await _jobTitleRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static JobTitleDto Map(JobTitle jobTitle) => new(jobTitle.Id, jobTitle.Code, jobTitle.NameEn, jobTitle.NameAr, jobTitle.Description, jobTitle.IsActive);
}

