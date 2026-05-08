using Microsoft.AspNetCore.Mvc;
using OrganizationAPI.DTOs.Companies;
using OrganizationAPI.Services.Interfaces;
namespace OrganizationAPI.Controllers
{
	[ApiController]
	[Route("api/companies")]
	public class CompaniesController : ControllerBase
	{
		private readonly ICompanyService _companyService;

		public CompaniesController(ICompanyService companyService)
		{
			_companyService = companyService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var companies = await _companyService.GetAllAsync();
			return Ok(companies);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var company = await _companyService.GetByIdAsync(id);

			if (company == null)
				return NotFound();

			return Ok(company);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CompanyInputDto dto)
		{
			try
			{
				var createdCompany = await _companyService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = createdCompany.Id }, createdCompany);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, CompanyInputDto dto)
		{
			try
			{
				var updated = await _companyService.UpdateAsync(id, dto);

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
				var deleted = await _companyService.DeleteAsync(id);

				if (!deleted)
					return NotFound();

				return NoContent();
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
