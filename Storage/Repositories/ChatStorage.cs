using Microsoft.EntityFrameworkCore;
using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;

namespace Storage.Repositories
{
	public class ChatStorage : IChatStorage
	{
		private readonly ChatDatabase _context;

		public ChatStorage(string username)
		{
			_context = new ChatDatabase(username);
			_context.Database.EnsureCreated();
		}

		public async Task<ChatViewModel?> DeleteAsync(ChatBindingModel model)
		{
			var element = await _context.Chats.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			if (element != null)
			{
				_context.Chats.Remove(element);
				await _context.SaveChangesAsync();
				return element.GetViewModel;
			}
			return null;
		}

		public async Task<ChatViewModel?> GetElementAsync(ChatSearchModel model)
		{
			if ((string.IsNullOrEmpty(model.CurrentUser) || string.IsNullOrEmpty(model.Interlocutor)) && !model.Id.HasValue)
			{
				return null;
			}
			var entity = await _context.Chats
				.FirstOrDefaultAsync(x =>
					(x.CurrentUser == model.CurrentUser &&
					x.Interlocutor == model.Interlocutor) ||
					(model.Id.HasValue && x.Id == model.Id));

			if (entity != null)
			{
				return entity.GetViewModel;
			}
			return null;
		}

		public async Task<List<ChatViewModel>> GetFilteredListAsync(ChatSearchModel model)
		{
			var query = _context.Chats.AsQueryable();

			if (!string.IsNullOrEmpty(model.CurrentUser))
			{
				query = query.Where(x => x.CurrentUser == model.CurrentUser);
			}

			if (!string.IsNullOrEmpty(model.Interlocutor))
			{
				query = query.Where(x => x.Interlocutor == model.Interlocutor);
			}

			return await query
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<List<ChatViewModel>> GetFullListAsync()
		{
			return await _context.Chats
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<ChatViewModel?> InsertAsync(ChatBindingModel model)
		{
			var newChat = Chat.Create(model);
			if (newChat == null)
			{
				return null;
			}
			await _context.Chats.AddAsync(newChat);
			await _context.SaveChangesAsync();
			return newChat.GetViewModel;
		}

		public async Task<ChatViewModel?> UpdateAsync(ChatBindingModel model)
		{
			var chat = await _context.Chats.FirstOrDefaultAsync(x => x.Id == model.Id);
			if (chat == null)
			{
				return null;
			}
			chat.Update(model);
			await _context.SaveChangesAsync();
			return chat.GetViewModel;
		}

		public async Task<PaginatedResult<ChatViewModel>> GetRecentChatsAsync(int page, int pageSize)
		{
			// Базовый запрос для чатов с сообщениями
			var baseQuery = _context.Chats
				.Where(c => c.Messages.Any())
				.AsQueryable();

			// Получаем общее количество
			int totalCount = await baseQuery.CountAsync();
			int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			// Выбираем данные с пагинацией
			var chats = await baseQuery
				// Загружаем последнее сообщение для каждого чата
				.Select(c => new
				{
					Chat = c,
					LastMessage = c.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault()
				})
				// Сортируем по дате последнего сообщения (новые сверху)
				.OrderByDescending(x => x.LastMessage.Timestamp)
				// Применяем пагинацию
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Преобразуем в ViewModel
			var viewModels = chats.Select(x => new ChatViewModel
			{
				Id = x.Chat.Id,
				CurrentUser = x.Chat.CurrentUser,
				Interlocutor = x.Chat.Interlocutor,
				LastMessageText = x.LastMessage?.Content ?? "",
				LastMessageTime = x.LastMessage?.Timestamp
			}).ToList();

			return new PaginatedResult<ChatViewModel>
			{
				Items = viewModels,
				Page = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				TotalCount = totalCount
			};
		}

		public async Task<PaginatedResult<ChatViewModel>> SearchChatsAsync(
			ChatSearchModel searchModel,
			int page = 1,
			int pageSize = 30)
		{
			// Базовый запрос
			var baseQuery = _context.Chats
				.Where(c => c.Messages.Any()) // Только чаты с сообщениями
				.AsQueryable();

			// Применяем фильтры из searchModel
			if (searchModel.Id.HasValue)
			{
				baseQuery = baseQuery.Where(c => c.Id == searchModel.Id.Value);
			}

			if (!string.IsNullOrEmpty(searchModel.CurrentUser))
			{
				baseQuery = baseQuery.Where(c => c.CurrentUser.Contains(searchModel.CurrentUser));
			}

			if (!string.IsNullOrEmpty(searchModel.Interlocutor))
			{
				// Поиск по имени собеседника (регистронезависимый)
				baseQuery = baseQuery.Where(c =>
					c.Interlocutor.ToLower().Contains(searchModel.Interlocutor.ToLower()));
			}

			// Получаем общее количество с учетом фильтров
			int totalCount = await baseQuery.CountAsync();
			int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

			// Получаем данные с пагинацией
			var chats = await baseQuery
				.Select(c => new
				{
					Chat = c,
					LastMessage = c.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault()
				})
				// Сортируем по релевантности: сначала точное совпадение, затем по дате
				.OrderByDescending(x =>
					!string.IsNullOrEmpty(searchModel.Interlocutor) &&
					x.Chat.Interlocutor.ToLower() == searchModel.Interlocutor.ToLower() ? 1 : 0)
				.ThenByDescending(x => x.LastMessage.Timestamp)
				// Пагинация
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Преобразуем в ViewModel
			var viewModels = chats.Select(x => new ChatViewModel
			{
				Id = x.Chat.Id,
				CurrentUser = x.Chat.CurrentUser,
				Interlocutor = x.Chat.Interlocutor,
				LastMessageText = x.LastMessage?.Content ?? "",
				LastMessageTime = x.LastMessage?.Timestamp
			}).ToList();

			return new PaginatedResult<ChatViewModel>
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
