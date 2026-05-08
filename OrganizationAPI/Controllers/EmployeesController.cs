using Microsoft.AspNetCore.Mvc;
using OrganizationAPI.DTOs.Employees;
using OrganizationAPI.Services.Interfaces;

namespace OrganizationAPI.Controllers
{
	[ApiController]
	[Route("api/employees")]
	public class EmployeesController : ControllerBase
	{
		private readonly IEmployeeService _employeeService;

		public EmployeesController(IEmployeeService employeeService)
		{
			_employeeService = employeeService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var employees = await _employeeService.GetAllAsync();
			return Ok(employees);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var employee = await _employeeService.GetByIdAsync(id);

			if (employee == null)
			{
				return NotFound();
			}

			return Ok(employee);
		}
		[HttpPost]
		public async Task<IActionResult> Create(EmployeeInputDto dto)
		{
			try
			{
				var createdEmployee = await _employeeService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, EmployeeInputDto dto)
		{
			try
			{
				var updated = await _employeeService.UpdateAsync(id, dto);

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
				var deleted = await _employeeService.DeleteAsync(id);

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
