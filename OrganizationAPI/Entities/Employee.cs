namespace OrganizationAPI.Entities
{
	public class Employee
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }
		public Company Company { get; set; } = null!;

		public string? Title { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Phone { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		// ak je zamestnanec manažér nejakej jednotky
		public ICollection<OrganizationUnit> ManagedUnits { get; set; } = new List<OrganizationUnit>();
	}
}
