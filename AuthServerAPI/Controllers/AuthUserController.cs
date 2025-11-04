using AuthServerAPI.Logic;
using AuthServerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthUserController : ControllerBase
	{
		private readonly IUserLogic _userLogic;
		//private readonly ILogger<AuthUserController> _logger;

		public AuthUserController(IUserLogic userLogic)
		{
			_userLogic = userLogic;
		}

		// Доступен всем
		
		[HttpGet]
		public IActionResult GetAll()
		{
			//_logger.LogInformation("Попытка получения списка факультетов");
			return Ok(_userLogic.ReadListAsync(null));
		}

		// Доступен всем
		
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			//_logger.LogInformation($"Попытка получения факультета по id{id}");
			var userulty = _userLogic.ReadElementAsync(new UserSearchModel { Id = id });
			if (userulty == null)
			{
				//_logger.LogWarning($"Факультет по id{id} не найден");
				return NotFound();
			}
			//_logger.LogInformation($"Факультет по id{id} найден");
			return Ok(userulty);
		}

		// Только для админа
		[HttpPost]
		public IActionResult Create([FromBody] UserBindingModel model)
		{
			try
			{
				//_logger.LogInformation($"Попытка создания факультета c id{model.Id}");
				if (!_userLogic.CreateAsync(model))
				{
					//_logger.LogWarning($"Факультет c id{model.Id} не был создан");
					return BadRequest("Ошибка при создании факультета");
				}
				//_logger.LogInformation($"Факультет c id{model.Id} был создан");
				return Ok("Факультет успешно создан");
			}
			catch (Exception ex)
			{
				return BadRequest("Ошибка при создании факультета" + ex.Message);
			}
		}

		// Только для админа
		[HttpPut]
		public IActionResult Update([FromBody] UserBindingModel model)
		{
			try
			{
				if (!_userLogic.UpdateAsync(model))
				{
					//_logger.LogWarning($"Факультет c id{model.Id} не был обновлен");
					return BadRequest("Ошибка при обновлении факультета");
				}
				//_logger.LogInformation($"Факультет c id{model.Id} был обновлен");
				return Ok("Факультет успешно обновлён");
			}
			catch (Exception ex)
			{
				return BadRequest("Ошибка при обновлении факультета" + ex.Message);
			}
		}

		// Только для админа
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			if (!_userLogic.DeleteAsync(new UserBindingModel { Id = id }))
			{
				//_logger.LogWarning($"Факультет c id{id} не был удален");
				return BadRequest("Ошибка при удалении факультета");
			}
			//_logger.LogInformation($"Факультет c id{id} был удален");
			return Ok("Факультет успешно удалён");
		}
	}
}
