using OrganizationAPI.DTOs.Employees;
namespace OrganizationAPI.Services.Interfaces
{
	public interface IEmployeeService
	{
		Task<List<EmployeeGetDto>> GetAllAsync();
		Task<EmployeeGetDto?> GetByIdAsync(int id);
		Task<EmployeeGetDto> CreateAsync(EmployeeInputDto dto);
		Task<bool> UpdateAsync(int id, EmployeeInputDto dto);
		Task<bool> DeleteAsync(int id);
	}
}
