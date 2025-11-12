using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace DesktopClient
{
	// C:\Users\[Username]\AppData\Roaming\SecureChat\session.dat - storage place
	public static class SecureStorage
	{
		private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Oleja123");
		// path to the file with token
		private static readonly string StoragePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"SecureChat",
			"session.dat");

		public static void SaveSessionToken(string token)
		{
			try
			{
				if (string.IsNullOrEmpty(token))
				{
					DeleteSessionToken();
					return;
				}

				var directory = Path.GetDirectoryName(StoragePath);
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}

				// encrypting the token with DPAPI ( i dont fuckin know what is it)
				byte[] encryptedData = ProtectedData.Protect(
					userData: Encoding.UTF8.GetBytes(token),
					optionalEntropy: Entropy,
					scope: DataProtectionScope.CurrentUser); // only current user can decrypt

				File.WriteAllBytes(StoragePath, encryptedData);
			}
			catch(Exception ex) 
			{
				MessageBox.Show($"Ошибка сохранения сессии: {ex.Message}",
				   "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static string LoadSessionToken()
		{
			try
			{
				if (!File.Exists(StoragePath))
				{
					return null;
				}

				byte[] encryptedData = File.ReadAllBytes(StoragePath);

				byte[] decryptedData = ProtectedData.Unprotect(
					encryptedData: encryptedData,
					optionalEntropy: Entropy,
					scope: DataProtectionScope.CurrentUser
					);

				string token = Encoding.UTF8.GetString(decryptedData);
				return token;
			}
			catch (Exception ex) {
				return null;
			}
		}

		public static void DeleteSessionToken()
		{
			try
			{
				if (File.Exists(StoragePath))
				{
					File.Delete(StoragePath);
				}
			}
			catch (Exception ex) {
				MessageBox.Show($"Ошибка удаления сессии: {ex.Message}",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} 
		}

		public static bool HasSessionToken()
		{
			return File.Exists(StoragePath);
		}
	}
}
