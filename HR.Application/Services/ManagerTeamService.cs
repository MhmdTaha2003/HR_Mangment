using HR.Application.Interfaces.Repositories;

namespace HR.Application.Services;

public sealed class ManagerTeamService : IManagerTeamService
{
    private readonly IEmployeeAssignmentRepository _assignments;

    public ManagerTeamService(IEmployeeAssignmentRepository assignments) => _assignments = assignments;

    public async Task<IReadOnlyList<long>> GetTeamEmployeeIdsAsync(long managerEmployeeId, CancellationToken cancellationToken = default) =>
        await _assignments.GetCurrentDirectReportIdsAsync(managerEmployeeId, cancellationToken);

    public Task<bool> IsEmployeeInTeamAsync(long managerEmployeeId, long employeeId, CancellationToken cancellationToken = default) =>
        _assignments.IsCurrentDirectReportAsync(managerEmployeeId, employeeId, cancellationToken);
}
