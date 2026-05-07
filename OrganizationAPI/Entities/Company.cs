namespace OrganizationAPI.Entities
{
	public class Company
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;

		// vzťahy
		public ICollection<Employee> Employees { get; set; } = new List<Employee>();

		public ICollection<OrganizationUnit> OrganizationUnits { get; set; } = new List<OrganizationUnit>();
	}
}
