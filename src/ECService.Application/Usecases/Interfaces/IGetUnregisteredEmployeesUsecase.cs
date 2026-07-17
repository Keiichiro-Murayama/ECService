using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

public interface IGetUnregisteredEmployeesUsecase
{
    Task<List<Employee>> ExecuteAsync();
}