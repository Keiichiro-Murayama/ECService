using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

public class GetUnregisteredEmployeesUsecase : IGetUnregisteredEmployeesUsecase
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetUnregisteredEmployeesUsecase(
        IEmployeeRepository employeeRepository
    )
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<List<Employee>> ExecuteAsync()
    {
        return await _employeeRepository.SelectUnregisteredAsync();
    }
}