using Microsoft.AspNetCore.Mvc;
using OrganizationAPI.DTOs.OrganizationUnits;
using OrganizationAPI.Services.Interfaces;
namespace OrganizationAPI.Controllers
{
	[ApiController]
	[Route("api/organization-units")]
	public class OrganizationUnitsController : ControllerBase
	{
		private readonly IOrganizationUnitService _organizationUnitService;

		public OrganizationUnitsController(IOrganizationUnitService organizationUnitService)
		{
			_organizationUnitService = organizationUnitService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var units = await _organizationUnitService.GetAllAsync();
			return Ok(units);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var unit = await _organizationUnitService.GetByIdAsync(id);

			if (unit == null)
				return NotFound();

			return Ok(unit);
		}

		[HttpPost]
		public async Task<IActionResult> Create(OrganizationUnitInputDto dto)
		{
			try
			{
				var createdUnit = await _organizationUnitService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = createdUnit.Id }, createdUnit);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, OrganizationUnitInputDto dto)
		{
			try
			{
				var updated = await _organizationUnitService.UpdateAsync(id, dto);

				if (!updated)
					return NotFound();

				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var deleted = await _organizationUnitService.DeleteAsync(id);

				if (!deleted)
					return NotFound();

				return NoContent();
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("company/{companyId:int}/tree")]
		public async Task<IActionResult> GetCompanyTree(int companyId)
		{
			var tree = await _organizationUnitService.GetCompanyTreeAsync(companyId);

			if (tree == null)
				return NotFound();

			return Ok(tree);
		}
	}
}
