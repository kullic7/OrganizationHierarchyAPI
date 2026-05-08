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
				return;
			}

			var seedDefinitions = new[]
			{
				new
				{
					Name = "TechCorp",
					Code = "TECH",
					Employees = new[]
					{
						new Employee { Title = "Ing.", FirstName = "Ján", LastName = "Novák", Phone = "+421900111111", Email = "jan.novak@techcorp.com" },
						new Employee { Title = "Mgr.", FirstName = "Eva", LastName = "Kováčová", Phone = "+421900111112", Email = "eva.kovacova@techcorp.com" },
						new Employee { Title = "Bc.", FirstName = "Peter", LastName = "Horváth", Phone = "+421900111113", Email = "peter.horvath@techcorp.com" }
					}
				},
				new
				{
					Name = "FinGroup",
					Code = "FIN",
					Employees = new[]
					{
						new Employee { Title = "Ing.", FirstName = "Martin", LastName = "Varga", Phone = "+421900222221", Email = "martin.varga@fingroup.com" },
						new Employee { Title = "Mgr.", FirstName = "Lucia", LastName = "Balážová", Phone = "+421900222222", Email = "lucia.balazova@fingroup.com" },
						new Employee { Title = "Bc.", FirstName = "Tomáš", LastName = "Marek", Phone = "+421900222223", Email = "tomas.marek@fingroup.com" }
					}
				},
				new
				{
					Name = "HealthPlus",
					Code = "HEALTH",
					Employees = new[]
					{
						new Employee { Title = "MUDr.", FirstName = "Zuzana", LastName = "Bieliková", Phone = "+421900333331", Email = "zuzana.bielikova@healthplus.com" },
						new Employee { Title = "Ing.", FirstName = "David", LastName = "Kollár", Phone = "+421900333332", Email = "david.kollar@healthplus.com" },
						new Employee { Title = "Mgr.", FirstName = "Mária", LastName = "Šimková", Phone = "+421900333333", Email = "maria.simkova@healthplus.com" }
					}
				}
			};

			foreach (var seed in seedDefinitions)
			{
				var company = new Company
				{
					Name = seed.Name,
					Code = seed.Code
				};

				var employees = seed.Employees.ToList();

				foreach (var employee in employees)
				{
					employee.Company = company;
				}

				var companyUnit = new OrganizationUnit
				{
					Company = company,
					Parent = null,
					Name = company.Name,
					Code = company.Code,
					Type = OrganizationUnitType.Company,
					Manager = employees[0]
				};

				for (int d = 1; d <= 2; d++)
				{
					var division = new OrganizationUnit
					{
						Company = company,
						Parent = companyUnit,
						Name = $"Division {d} - {company.Name}",
						Code = $"{company.Code}-DIV-{d}",
						Type = OrganizationUnitType.Division,
						Manager = employees[1]
					};

					for (int p = 1; p <= 2; p++)
					{
						var project = new OrganizationUnit
						{
							Company = company,
							Parent = division,
							Name = $"Project {p} - Division {d}",
							Code = $"{company.Code}-D{d}-PRJ-{p}",
							Type = OrganizationUnitType.Project,
							Manager = employees[2]
						};

						for (int dep = 1; dep <= 2; dep++)
						{
							var department = new OrganizationUnit
							{
								Company = company,
								Parent = project,
								Name = $"Department {dep} - Project {p}",
								Code = $"{company.Code}-D{d}-P{p}-DEP-{dep}",
								Type = OrganizationUnitType.Department,
								Manager = employees[dep - 1]
							};

							dbContext.OrganizationUnits.Add(department);
						}

						dbContext.OrganizationUnits.Add(project);
					}

					dbContext.OrganizationUnits.Add(division);
				}

				dbContext.Companies.Add(company);
				dbContext.Employees.AddRange(employees);
				dbContext.OrganizationUnits.Add(companyUnit);
			}

			await dbContext.SaveChangesAsync();
		}
	}
}