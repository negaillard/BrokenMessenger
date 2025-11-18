using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace DesktopClient
{
	public class APIClient
	{
		private readonly HttpClient _httpClient;
		private string _baseUrl;
		private string _sessionToken;


		public APIClient(string baseUrl = "http://localhost:8080")
		{
			_baseUrl = baseUrl;
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			_sessionToken = SecureStorage.LoadSessionToken();
			UpdateAuthHeader();
		}

		public void SetSessionToken(string token)
		{
			_sessionToken = token;
			UpdateAuthHeader();
			SecureStorage.SaveSessionToken(token);
		}

		public void ClearSession()
		{
			_sessionToken = null;
			_httpClient.DefaultRequestHeaders.Authorization = null;
			SecureStorage.DeleteSessionToken();
		}

		private void UpdateAuthHeader()
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
			if (!string.IsNullOrEmpty(_sessionToken))
			{
				_httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", _sessionToken);
			}
		}

		public async Task<T> GetAsync<T>(string url)
		{
			var response = await _httpClient.GetAsync(_baseUrl + url);
			return await HandleResponse<T>(response);
		}
		public async Task<T> PutAsync<T>(string url, object data = null)
		{
			var content = CreateJsonDocument(data);
			var response = await _httpClient.PutAsync($"{_baseUrl}{url}", content);
			return await HandleResponse<T>(response);
		}

		public async Task<T> PostAsync<T>(string url, object data = null)
		{
			var content = CreateJsonDocument(data);
			var response = await _httpClient.PostAsync(_baseUrl + url, content);
			return await HandleResponse<T>(response);
		}

		public async Task DeleteAsync(string url)
		{
			var response = await _httpClient.DeleteAsync(_baseUrl + url);
			await HandleResponse<object>(response);
		}

		public async Task<T> PostAnonymousAsync<T>(string url, object data = null)
		{
			_httpClient.DefaultRequestHeaders.Authorization = null;
			try
			{
				var content = CreateJsonDocument(data);
				var response = await _httpClient.PostAsync(_baseUrl + url, content);

				return await HandleResponse<T>(response);
			}
			finally
			{
				UpdateAuthHeader();
			}
		}

		private StringContent CreateJsonDocument(object data)
		{
			if (data == null)
			{
				return new StringContent("", Encoding.UTF8, "application/json");
			}

			var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			});

			return new StringContent(json, Encoding.UTF8, "application/json");
		}

		private async Task<T> HandleResponse<T>(HttpResponseMessage response)
		{
			var content = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"HTTP Error: {response.StatusCode}\n{content}");
			}

			if (string.IsNullOrEmpty(content))
			{
				return default(T);
			}

			return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});
		}

		public void Dispose()
		{
			_httpClient.Dispose();
		}
	}
}
