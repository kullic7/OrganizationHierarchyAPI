using OrganizationAPI.DTOs.Companies;
namespace OrganizationAPI.Services.Interfaces
{
	public interface ICompanyService
	{
		Task<List<CompanyGetDto>> GetAllAsync();
		Task<CompanyGetDto?> GetByIdAsync(int id);
		Task<CompanyGetDto> CreateAsync(CompanyInputDto dto);
		Task<bool> UpdateAsync(int id, CompanyInputDto dto);
		Task<bool> DeleteAsync(int id);
	}
}
