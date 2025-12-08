using Microsoft.EntityFrameworkCore;
using AuthServerAPI.Models;
using Models;

namespace AuthServerAPI.Storage
{
	public class UserStorage : IUserStorage
	{
		private readonly Context _context;

		public UserStorage()
		{
			_context = new Context();
		}
		public async Task<UserBindingModel?> DeleteAsync(UserBindingModel model)
		{
			var element = await _context.Users.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			if (element != null)
			{
				_context.Users.Remove(element);
				await _context.SaveChangesAsync();
				return element.GetBindingModel;
			}
			return null;
		}

		public async Task<UserBindingModel?> GetElementAsync(UserSearchModel model)
		{
			var query = _context.Users.AsQueryable();


			if (!string.IsNullOrEmpty(model.Username))
			{
				query = query.Where(x => x.Username == model.Username);
			}

			if (!string.IsNullOrEmpty(model.Email))
			{
				query = query.Where(x => x.Email == model.Email);
			}

			var entity = await query.FirstOrDefaultAsync();

			if (entity != null)
			{
				return new UserBindingModel
				{
					Id = entity.Id,
					Username = entity.Username,
					Email = entity.Email,
				};
			}

			return null;
		}

		public async Task<List<UserBindingModel>> GetFilteredListAsync(UserSearchModel model)
		{
			var query = _context.Users.AsQueryable();

			if (!string.IsNullOrEmpty(model.Username))
			{
				query = query.Where(x => x.Username == model.Username);
			}

			if (!string.IsNullOrEmpty(model.Email))
			{
				query = query.Where(x => x.Email == model.Email);
			}

			return await query
				.Select(x => x.GetBindingModel)
				.ToListAsync();
		}

		public async Task<List<UserBindingModel>> GetFullListAsync()
		{
			return await _context.Users
				.Select(x => x.GetBindingModel)
				.ToListAsync();
		}

		public async Task<UserBindingModel?> InsertElementAsync(UserBindingModel model)
		{
			var newUser = AuthUser.Create(model);
			if (newUser == null)
			{
				return null;
			}
			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();
			return newUser.GetBindingModel;
		}

		public async Task<UserBindingModel?> UpdateAsync(UserBindingModel model)
		{
			var chat = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
			if (chat == null)
			{
				return null;
			}
			chat.Update(model);
			await _context.SaveChangesAsync();
			return chat.GetBindingModel;
		}

		public async Task<PaginatedResult<UserBindingModel>> SearchUsersAsync(
		UserSearchModel searchModel,
		int page = 1,
		int pageSize = 30)
		{
			// Базовый запрос
			var baseQuery = _context.Users.AsQueryable();

			// Применяем фильтр по имени пользователя
			if (!string.IsNullOrEmpty(searchModel.Username))
			{
				baseQuery = baseQuery.Where(u =>
					u.Username.ToLower().Contains(searchModel.Username.ToLower()));
			}

			// Получаем общее количество
			int totalCount = await baseQuery.CountAsync();
			int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			// Получаем данные с пагинацией
			var users = await baseQuery
				// Сортируем по релевантности: сначала точное совпадение
				.OrderByDescending(u =>
					!string.IsNullOrEmpty(searchModel.Username) &&
					u.Username.ToLower() == searchModel.Username.ToLower() ? 1 : 0)
				.ThenBy(u => u.Username) // Затем по алфавиту
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Преобразуем в ViewModel
			var viewModels = users.Select(x => x.GetBindingModel).ToList();

			return new PaginatedResult<UserBindingModel>
			{
				Items = viewModels,
				Page = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				TotalCount = totalCount
			};
		}
	}
}

