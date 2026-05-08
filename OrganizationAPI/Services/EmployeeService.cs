using Microsoft.EntityFrameworkCore;
using OrganizationAPI.Data;
using OrganizationAPI.DTOs.Employees;
using OrganizationAPI.Entities;
using OrganizationAPI.Services.Interfaces;
namespace OrganizationAPI.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly AppDbContext _dbContext;

		public EmployeeService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<EmployeeGetDto>> GetAllAsync()
		{
			return await _dbContext.Employees
				.AsNoTracking()
				.Select(e => new EmployeeGetDto
				{
					Id = e.Id,
					CompanyId = e.CompanyId,
					Title = e.Title,
					FirstName = e.FirstName,
					LastName = e.LastName,
					Phone = e.Phone,
					Email = e.Email
				})
				.ToListAsync();
		}

		public async Task<EmployeeGetDto?> GetByIdAsync(int id)
		{
			return await _dbContext.Employees
				.AsNoTracking()
				.Where(e => e.Id == id)
				.Select(e => new EmployeeGetDto
				{
					Id = e.Id,
					CompanyId = e.CompanyId,
					Title = e.Title,
					FirstName = e.FirstName,
					LastName = e.LastName,
					Phone = e.Phone,
					Email = e.Email
				})
				.FirstOrDefaultAsync();
		}
		public async Task<EmployeeGetDto> CreateAsync(EmployeeInputDto dto)
		{
			ValidateInput(dto);
			var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == dto.CompanyId);
			if (!companyExists)
				throw new ArgumentException("Company does not exist.");

			var employee = new Employee
			{
				CompanyId = dto.CompanyId,
				Title = dto.Title,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Phone = dto.Phone,
				Email = dto.Email
			};

			_dbContext.Employees.Add(employee);
			await _dbContext.SaveChangesAsync();

			return new EmployeeGetDto
			{
				Id = employee.Id,
				CompanyId = employee.CompanyId,
				Title = employee.Title,
				FirstName = employee.FirstName,
				LastName = employee.LastName,
				Phone = employee.Phone,
				Email = employee.Email
			};
		}

		public async Task<bool> UpdateAsync(int id, EmployeeInputDto dto)
		{
			ValidateInput(dto);
			var employee = await _dbContext.Employees.FindAsync(id);
			if (employee == null)
				return false;

			var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == dto.CompanyId);
			if (!companyExists)
				throw new ArgumentException("Company does not exist.");

			employee.CompanyId = dto.CompanyId;
			employee.Title = dto.Title;
			employee.FirstName = dto.FirstName;
			employee.LastName = dto.LastName;
			employee.Phone = dto.Phone;
			employee.Email = dto.Email;

			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var employee = await _dbContext.Employees
		.Include(e => e.ManagedUnits)
		.FirstOrDefaultAsync(e => e.Id == id);

			if (employee == null)
				return false;

			if (employee.ManagedUnits.Any())
				throw new InvalidOperationException(
					"Employee is manager of organization unit and cannot be deleted.");

			_dbContext.Employees.Remove(employee);

			await _dbContext.SaveChangesAsync();

			return true;
		}
		private static void ValidateInput(EmployeeInputDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.FirstName))
				throw new ArgumentException("First name is required.");

			if (string.IsNullOrWhiteSpace(dto.LastName))
				throw new ArgumentException("Last name is required.");

			if (string.IsNullOrWhiteSpace(dto.Phone))
				throw new ArgumentException("Phone is required.");

			if (string.IsNullOrWhiteSpace(dto.Email))
				throw new ArgumentException("Email is required.");
		}
	}
}
