using Microsoft.EntityFrameworkCore;
using OrganizationAPI.Entities;
namespace OrganizationAPI.Data
{
	public static class DatabaseSeeder
	{
		public static async Task SeedAsync(AppDbContext dbContext)
		{
			if (await dbContext.Companies.AnyAsync())
			{
				return; // databáza už je naplnená
			}

			var companies = new List<Company>
		{
			new() { Id = 1, Name = "TechCorp", Code = "TECH" },
			new() { Id = 2, Name = "FinGroup", Code = "FIN" },
			new() { Id = 3, Name = "HealthPlus", Code = "HEALTH" }
		};

			var employees = new List<Employee>
		{
			new() { Id = 1, CompanyId = 1, Title = "Ing.", FirstName = "Ján", LastName = "Novák", Phone = "+421900111111", Email = "jan.novak@techcorp.com" },
			new() { Id = 2, CompanyId = 1, Title = "Mgr.", FirstName = "Eva", LastName = "Kováčová", Phone = "+421900111112", Email = "eva.kovacova@techcorp.com" },
			new() { Id = 3, CompanyId = 1, Title = "Bc.", FirstName = "Peter", LastName = "Horváth", Phone = "+421900111113", Email = "peter.horvath@techcorp.com" },

			new() { Id = 4, CompanyId = 2, Title = "Ing.", FirstName = "Martin", LastName = "Varga", Phone = "+421900222221", Email = "martin.varga@fingroup.com" },
			new() { Id = 5, CompanyId = 2, Title = "Mgr.", FirstName = "Lucia", LastName = "Balážová", Phone = "+421900222222", Email = "lucia.balazova@fingroup.com" },
			new() { Id = 6, CompanyId = 2, Title = "Bc.", FirstName = "Tomáš", LastName = "Marek", Phone = "+421900222223", Email = "tomas.marek@fingroup.com" },

			new() { Id = 7, CompanyId = 3, Title = "MUDr.", FirstName = "Zuzana", LastName = "Bieliková", Phone = "+421900333331", Email = "zuzana.bielikova@healthplus.com" },
			new() { Id = 8, CompanyId = 3, Title = "Ing.", FirstName = "David", LastName = "Kollár", Phone = "+421900333332", Email = "david.kollar@healthplus.com" },
			new() { Id = 9, CompanyId = 3, Title = "Mgr.", FirstName = "Mária", LastName = "Šimková", Phone = "+421900333333", Email = "maria.simkova@healthplus.com" }
		};

			var units = new List<OrganizationUnit>();

			int id = 1;

			foreach (var company in companies)
			{
				var employeeIds = employees
					.Where(e => e.CompanyId == company.Id)
					.Select(e => e.Id)
					.ToList();

				var companyUnit = new OrganizationUnit
				{
					Id = id++,
					CompanyId = company.Id,
					ParentId = null,
					Name = company.Name,
					Code = company.Code,
					Type = OrganizationUnitType.Company,
					ManagerId = employeeIds[0]
				};

				units.Add(companyUnit);

				for (int d = 1; d <= 2; d++)
				{
					var division = new OrganizationUnit
					{
						Id = id++,
						CompanyId = company.Id,
						ParentId = companyUnit.Id,
						Name = $"Division {d} - {company.Name}",
						Code = $"{company.Code}-DIV-{d}",
						Type = OrganizationUnitType.Division,
						ManagerId = employeeIds[1]
					};

					units.Add(division);

					for (int p = 1; p <= 2; p++)
					{
						var project = new OrganizationUnit
						{
							Id = id++,
							CompanyId = company.Id,
							ParentId = division.Id,
							Name = $"Project {p} - Division {d}",
							Code = $"{company.Code}-D{d}-PRJ-{p}",
							Type = OrganizationUnitType.Project,
							ManagerId = employeeIds[2]
						};

						units.Add(project);

						for (int dep = 1; dep <= 2; dep++)
						{
							units.Add(new OrganizationUnit
							{
								Id = id++,
								CompanyId = company.Id,
								ParentId = project.Id,
								Name = $"Department {dep} - Project {p}",
								Code = $"{company.Code}-D{d}-P{p}-DEP-{dep}",
								Type = OrganizationUnitType.Department,
								ManagerId = employeeIds[dep - 1]
							});
						}
					}
				}
			}

			dbContext.Companies.AddRange(companies);
			dbContext.Employees.AddRange(employees);
			dbContext.OrganizationUnits.AddRange(units);

			await dbContext.SaveChangesAsync();
		}
	}
}
