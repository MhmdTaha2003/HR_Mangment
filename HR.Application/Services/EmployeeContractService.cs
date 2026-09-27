using HR.Application.DTOs.Employees;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class EmployeeContractService : IEmployeeContractService
{
    private readonly IEmployeeContractRepository _employeeContractRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeContractService(
        IEmployeeContractRepository employeeContractRepository,
        IEmployeeRepository employeeRepository)
    {
        _employeeContractRepository = employeeContractRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<List<EmployeeContractDto>> GetAllAsync(CancellationToken cancellationToken) => (await _employeeContractRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<EmployeeContractDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var contract = await _employeeContractRepository.GetByIdAsync(id, cancellationToken);
        return contract is null ? null : Map(contract);
    }

    public async Task<EmployeeContractDto> CreateAsync(CreateEmployeeContractDto dto, CancellationToken cancellationToken)
    {
        if (!await _employeeRepository.ExistsAsync(dto.EmployeeId, cancellationToken))
            throw new InvalidOperationException("Employee not found.");
        var normalizedContractNumber = dto.ContractNumber.Trim().ToUpperInvariant();
        if (await _employeeContractRepository.NumberExistsAsync(normalizedContractNumber, cancellationToken))
            throw new InvalidOperationException("Contract number already exists.");
        var contract = new EmployeeContract
        {
            EmployeeId = dto.EmployeeId,
            ContractNumber = normalizedContractNumber,
            ContractType = dto.ContractType,
            Status = dto.Status,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ProbationEndDate = dto.ProbationEndDate,
            Salary = dto.Salary,
            Notes = dto.Notes?.Trim()
        };
        await _employeeContractRepository.AddAsync(contract, cancellationToken);
        await _employeeContractRepository.SaveChangesAsync(cancellationToken);
        return Map(contract);
    }

    public async Task<bool> UpdateAsync(long id, UpdateEmployeeContractDto dto, CancellationToken cancellationToken)
    {
        var contract = await _employeeContractRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (contract is null)
            return false;
        contract.ContractType = dto.ContractType;
        contract.Status = dto.Status;
        contract.StartDate = dto.StartDate;
        contract.EndDate = dto.EndDate;
        contract.ProbationEndDate = dto.ProbationEndDate;
        contract.Salary = dto.Salary;
        contract.Notes = dto.Notes?.Trim();
        contract.UpdatedAt = DateTime.UtcNow;
        await _employeeContractRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static EmployeeContractDto Map(EmployeeContract contract) => new(contract.Id, contract.EmployeeId, contract.ContractNumber, contract.ContractType, contract.Status, contract.StartDate, contract.EndDate, contract.ProbationEndDate, contract.Salary, contract.Notes);
}

