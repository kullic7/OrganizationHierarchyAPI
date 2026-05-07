namespace OrganizationAPI.Entities
{
	public class OrganizationUnit
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }
		public Company Company { get; set; } = null!;

		// hierarchia (self reference)
		public int? ParentId { get; set; }
		public OrganizationUnit? Parent { get; set; }

		public ICollection<OrganizationUnit> Children { get; set; } = new List<OrganizationUnit>();

		// manažér
		public int? ManagerId { get; set; }
		public Employee? Manager { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;

		public OrganizationUnitType Type { get; set; }
	}
}
