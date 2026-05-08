using Microsoft.EntityFrameworkCore;
using OrganizationAPI.Data;
using OrganizationAPI.DTOs.Companies;
using OrganizationAPI.Entities;
using OrganizationAPI.Services.Interfaces;
namespace OrganizationAPI.Services
{
	public class CompanyService : ICompanyService
	{
		private readonly AppDbContext _dbContext;

		public CompanyService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<CompanyGetDto>> GetAllAsync()
		{
			return await _dbContext.Companies
				.AsNoTracking()
				.Select(c => new CompanyGetDto
				{
					Id = c.Id,
					Name = c.Name,
					Code = c.Code
				})
				.ToListAsync();
		}

		public async Task<CompanyGetDto?> GetByIdAsync(int id)
		{
			return await _dbContext.Companies
				.AsNoTracking()
				.Where(c => c.Id == id)
				.Select(c => new CompanyGetDto
				{
					Id = c.Id,
					Name = c.Name,
					Code = c.Code
				})
				.FirstOrDefaultAsync();
		}

		public async Task<CompanyGetDto> CreateAsync(CompanyInputDto dto)
		{
			ValidateInput(dto);

			var codeExists = await _dbContext.Companies
				.AnyAsync(c => c.Code == dto.Code);

			if (codeExists)
				throw new ArgumentException("Company code already exists.");

			var company = new Company
			{
				Name = dto.Name.Trim(),
				Code = dto.Code.Trim()
			};

			_dbContext.Companies.Add(company);
			await _dbContext.SaveChangesAsync();

			return new CompanyGetDto
			{
				Id = company.Id,
				Name = company.Name,
				Code = company.Code
			};
		}

		public async Task<bool> UpdateAsync(int id, CompanyInputDto dto)
		{
			ValidateInput(dto);

			var company = await _dbContext.Companies.FindAsync(id);

			if (company == null)
				return false;

			var codeExists = await _dbContext.Companies
				.AnyAsync(c => c.Code == dto.Code && c.Id != id);

			if (codeExists)
				throw new ArgumentException("Company code already exists.");

			company.Name = dto.Name.Trim();
			company.Code = dto.Code.Trim();

			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var company = await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.OrganizationUnits)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (company == null)
				return false;

			if (company.Employees.Any())
				throw new InvalidOperationException("Company cannot be deleted because it has employees.");

			if (company.OrganizationUnits.Any())
				throw new InvalidOperationException("Company cannot be deleted because it has organization units.");

			_dbContext.Companies.Remove(company);
			await _dbContext.SaveChangesAsync();

			return true;
		}

		private static void ValidateInput(CompanyInputDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Company name is required.");

			if (string.IsNullOrWhiteSpace(dto.Code))
				throw new ArgumentException("Company code is required.");
		}
	}
}
