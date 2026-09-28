using FluentValidation;
using HR.Application.Interfaces.Services;
using HR.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IJobTitleService, JobTitleService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeAssignmentService, EmployeeAssignmentService>();
        services.AddScoped<IEmployeeContractService, EmployeeContractService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IEmployeeDocumentService, EmployeeDocumentService>();
        services.AddScoped<ILeaveTypeService, LeaveTypeService>();
        services.AddScoped<IEmployeeLeaveBalanceService, EmployeeLeaveBalanceService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<ILeaveRequestApprovalService, LeaveRequestApprovalService>();

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);


        return services;
    }
}
