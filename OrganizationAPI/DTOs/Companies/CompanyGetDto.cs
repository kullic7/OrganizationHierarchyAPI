namespace OrganizationAPI.DTOs.Companies
{
	public class CompanyGetDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;
	}
}
