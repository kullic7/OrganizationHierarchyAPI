using OrganizationAPI.DTOs.OrganizationUnits;
namespace OrganizationAPI.Services.Interfaces
{
	public interface IOrganizationUnitService
	{
		Task<List<OrganizationUnitGetDto>> GetAllAsync();
		Task<OrganizationUnitGetDto?> GetByIdAsync(int id);
		Task<OrganizationUnitGetDto> CreateAsync(OrganizationUnitInputDto dto);
		Task<bool> UpdateAsync(int id, OrganizationUnitInputDto dto);
		Task<bool> DeleteAsync(int id);
		Task<OrganizationUnitTreeDto?> GetCompanyTreeAsync(int companyId);
	}
}
