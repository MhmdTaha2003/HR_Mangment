public interface IManagerTeamService
{
    Task<IReadOnlyList<long>> GetTeamEmployeeIdsAsync(
        long managerEmployeeId,
        CancellationToken cancellationToken = default);

    Task<bool> IsEmployeeInTeamAsync(
        long managerEmployeeId,
        long employeeId,
        CancellationToken cancellationToken = default);
}