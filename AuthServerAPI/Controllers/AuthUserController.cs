using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
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

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByUsername(string username)
		{
			//_logger.LogInformation($"Попытка получения факультета по id{id}");
			var user = await _userLogic.ReadElementAsync(new UserSearchModel { Username = username });
			if (user == null)
			{
				//_logger.LogWarning($"Факультет по id{id} не найден");
				return NotFound();
			}
			//_logger.LogInformation($"Факультет по id{id} найден");
			return Ok(user);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByEmail(string email)
		{
			//_logger.LogInformation($"Попытка получения факультета по id{id}");
			var user = await _userLogic.ReadElementAsync(new UserSearchModel { Email = email });
			if (user == null)
			{
				//_logger.LogWarning($"Факультет по id{id} не найден");
				return NotFound();
			}
			//_logger.LogInformation($"Факультет по id{id} найден");
			return Ok(user);
		}

		// Только для админа
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserBindingModel model)
		{
			try
			{
				// _logger.LogInformation($"Попытка создания факультета c id{model.Id}");
				var result = await _userLogic.CreateAsync(model); // ← await
				if (!result)
				{
					//_logger.LogWarning($"Факультет c id{model.Id} не был создан");

					return BadRequest("Ошибка при создании пользователя");
				}
				//_logger.LogInformation($"Факультет c id{model.Id} был создан");
				return Ok("Пользователь успешно создан");
			}
			catch (Exception ex)
			{
				return BadRequest("Ошибка при создании пользователя" + ex.Message);
			}
		}

		#region Useless methods
		// Доступен всем
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			//_logger.LogInformation("Попытка получения списка факультетов");
			var users = await _userLogic.ReadListAsync(null);
			return Ok(users);
		}

		// Доступен всем
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			//_logger.LogInformation($"Попытка получения факультета по id{id}");
			var user = await _userLogic.ReadElementAsync(new UserSearchModel { Id = id });
			if (user == null)
			{
				//_logger.LogWarning($"Факультет по id{id} не найден");
				return NotFound();
			}
			//_logger.LogInformation($"Факультет по id{id} найден");
			return Ok(user);
		}

		// Только для админа
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UserBindingModel model)
		{
			try
			{
				var updatedUser = await _userLogic.UpdateAsync(model);
				if (!updatedUser)
				{
					//_logger.LogWarning($"Факультет c id{model.Id} не был обновлен");
					return BadRequest("Ошибка при обновлении пользователя");
				}
				//_logger.LogInformation($"Факультет c id{model.Id} был обновлен");
				return Ok("Пользователь успешно обновлён");
			}
			catch (Exception ex)
			{
				return BadRequest("Ошибка при обновлении пользователя" + ex.Message);
			}
		}

		// Только для админа
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var deletedUser = await _userLogic.DeleteAsync(new UserBindingModel { Id = id });
			if (!deletedUser)
			{
				//_logger.LogWarning($"Факультет c id{id} не был удален");
				return BadRequest("Ошибка при удалении пользователя");
			}
			//_logger.LogInformation($"Факультет c id{id} был удален");
			return Ok("Пользователь успешно удалён");
		}
		#endregion
	}
}
