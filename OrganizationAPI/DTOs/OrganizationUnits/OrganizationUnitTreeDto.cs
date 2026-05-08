using OrganizationAPI.Entities;
namespace OrganizationAPI.DTOs.OrganizationUnits
{
	public class OrganizationUnitTreeDto
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }

		public int? ParentId { get; set; }

		public int? ManagerId { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;

		public OrganizationUnitType Type { get; set; }

		public List<OrganizationUnitTreeDto> Children { get; set; } = new();
	}
}
