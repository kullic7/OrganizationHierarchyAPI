namespace OrganizationAPI.DTOs.Employees
{
	public class EmployeeGetDto
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }

		public string? Title { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Phone { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;
	}
}
