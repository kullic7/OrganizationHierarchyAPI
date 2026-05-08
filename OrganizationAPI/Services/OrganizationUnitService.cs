using Microsoft.EntityFrameworkCore;
using OrganizationAPI.Data;
using OrganizationAPI.DTOs.OrganizationUnits;
using OrganizationAPI.Entities;
using OrganizationAPI.Services.Interfaces;
namespace OrganizationAPI.Services
{
	public class OrganizationUnitService : IOrganizationUnitService
	{
		private readonly AppDbContext _dbContext;

		public OrganizationUnitService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<OrganizationUnitGetDto>> GetAllAsync()
		{
			return await _dbContext.OrganizationUnits
				.AsNoTracking()
				.Select(o => new OrganizationUnitGetDto
				{
					Id = o.Id,
					CompanyId = o.CompanyId,
					ParentId = o.ParentId,
					ManagerId = o.ManagerId,
					Name = o.Name,
					Code = o.Code,
					Type = o.Type
				})
				.ToListAsync();
		}

		public async Task<OrganizationUnitGetDto?> GetByIdAsync(int id)
		{
			return await _dbContext.OrganizationUnits
				.AsNoTracking()
				.Where(o => o.Id == id)
				.Select(o => new OrganizationUnitGetDto
				{
					Id = o.Id,
					CompanyId = o.CompanyId,
					ParentId = o.ParentId,
					ManagerId = o.ManagerId,
					Name = o.Name,
					Code = o.Code,
					Type = o.Type
				})
				.FirstOrDefaultAsync();
		}

		public async Task<OrganizationUnitGetDto> CreateAsync(OrganizationUnitInputDto dto)
		{
			await ValidateInputAsync(dto, null);

			var unit = new OrganizationUnit
			{
				CompanyId = dto.CompanyId,
				ParentId = dto.ParentId,
				ManagerId = dto.ManagerId,
				Name = dto.Name.Trim(),
				Code = dto.Code.Trim(),
				Type = dto.Type
			};

			_dbContext.OrganizationUnits.Add(unit);
			await _dbContext.SaveChangesAsync();

			return new OrganizationUnitGetDto
			{
				Id = unit.Id,
				CompanyId = unit.CompanyId,
				ParentId = unit.ParentId,
				ManagerId = unit.ManagerId,
				Name = unit.Name,
				Code = unit.Code,
				Type = unit.Type
			};
		}

		public async Task<bool> UpdateAsync(int id, OrganizationUnitInputDto dto)
		{
			var unit = await _dbContext.OrganizationUnits.FindAsync(id);

			if (unit == null)
				return false;

			await ValidateInputAsync(dto, id);

			unit.CompanyId = dto.CompanyId;
			unit.ParentId = dto.ParentId;
			unit.ManagerId = dto.ManagerId;
			unit.Name = dto.Name.Trim();
			unit.Code = dto.Code.Trim();
			unit.Type = dto.Type;

			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var unit = await _dbContext.OrganizationUnits
				.Include(o => o.Children)
				.FirstOrDefaultAsync(o => o.Id == id);

			if (unit == null)
				return false;

			if (unit.Children.Any())
				throw new InvalidOperationException("Organization unit cannot be deleted because it has child units.");

			_dbContext.OrganizationUnits.Remove(unit);
			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<OrganizationUnitTreeDto?> GetCompanyTreeAsync(int companyId)
		{
			var units = await _dbContext.OrganizationUnits
				.AsNoTracking()
				.Where(o => o.CompanyId == companyId)
				.ToListAsync();

			if (!units.Any())
				return null;

			var dtoLookup = units.ToDictionary(
				unit => unit.Id,
				unit => new OrganizationUnitTreeDto
				{
					Id = unit.Id,
					CompanyId = unit.CompanyId,
					ParentId = unit.ParentId,
					ManagerId = unit.ManagerId,
					Name = unit.Name,
					Code = unit.Code,
					Type = unit.Type
				});

			foreach (var unit in units)
			{
				if (unit.ParentId.HasValue &&
					dtoLookup.TryGetValue(unit.ParentId.Value, out var parent))
				{
					parent.Children.Add(dtoLookup[unit.Id]);
				}
			}

			return dtoLookup.Values
				.FirstOrDefault(u => u.ParentId == null && u.Type == OrganizationUnitType.Company);
		}
		private async Task ValidateInputAsync(OrganizationUnitInputDto dto, int? currentUnitId)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Organization unit name is required.");

			if (string.IsNullOrWhiteSpace(dto.Code))
				throw new ArgumentException("Organization unit code is required.");

			var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == dto.CompanyId);

			if (!companyExists)
				throw new ArgumentException("Company does not exist.");

			var codeExists = await _dbContext.OrganizationUnits.AnyAsync(o =>
				o.CompanyId == dto.CompanyId &&
				o.Code == dto.Code &&
				(!currentUnitId.HasValue || o.Id != currentUnitId.Value));

			if (codeExists)
				throw new ArgumentException("Organization unit code already exists in this company.");

			if (dto.ManagerId == null)
				throw new ArgumentException("Manager is required.");

			var manager = await _dbContext.Employees
				.AsNoTracking()
				.FirstOrDefaultAsync(e => e.Id == dto.ManagerId);

			if (manager == null)
				throw new ArgumentException("Manager does not exist.");

			if (manager.CompanyId != dto.CompanyId)
				throw new ArgumentException("Manager must belong to the same company.");

			OrganizationUnit? parent = null;

			if (dto.ParentId.HasValue)
			{
				parent = await _dbContext.OrganizationUnits
					.AsNoTracking()
					.FirstOrDefaultAsync(o => o.Id == dto.ParentId.Value);

				if (parent == null)
					throw new ArgumentException("Parent organization unit does not exist.");

				if (parent.CompanyId != dto.CompanyId)
					throw new ArgumentException("Parent must belong to the same company.");
			}

			ValidateHierarchy(dto.Type, parent);

			if (currentUnitId.HasValue && dto.ParentId == currentUnitId.Value)
				throw new ArgumentException("Organization unit cannot be parent of itself.");
		}

		private static void ValidateHierarchy(OrganizationUnitType type, OrganizationUnit? parent)
		{
			switch (type)
			{
				case OrganizationUnitType.Company:
					if (parent != null)
						throw new ArgumentException("Company unit cannot have parent.");
					break;

				case OrganizationUnitType.Division:
					if (parent == null || parent.Type != OrganizationUnitType.Company)
						throw new ArgumentException("Division must have parent of type Company.");
					break;

				case OrganizationUnitType.Project:
					if (parent == null || parent.Type != OrganizationUnitType.Division)
						throw new ArgumentException("Project must have parent of type Division.");
					break;

				case OrganizationUnitType.Department:
					if (parent == null || parent.Type != OrganizationUnitType.Project)
						throw new ArgumentException("Department must have parent of type Project.");
					break;

				default:
					throw new ArgumentException("Invalid organization unit type.");
			}
		}
	}
}
