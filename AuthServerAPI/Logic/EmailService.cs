using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;

namespace AuthServerAPI.Logic
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;
		private readonly ILogger<EmailService> _logger;

		public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
		{
			_emailSettings = emailSettings.Value;
			_logger = logger;
		}

		public async Task<bool> SendVerificationCodeAsync(string email, string code, VerificationType type)
		{
			try
			{
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.MailLogin));
				message.To.Add(new MailboxAddress("", email));

				if (type == VerificationType.Registration)
				{
					message.Subject = "Код подтверждения регистрации";
				}
				else // Login
				{
					message.Subject = "Код для входа в аккаунт";
				}

				var bodyBuilder = new BodyBuilder();

				if (type == VerificationType.Registration)
				{
					bodyBuilder.TextBody = $"Ваш код подтверждения для регистрации: {code}\n\n" +
										  "Код действителен 15 минут.\n" +
										  "Если вы не запрашивали регистрацию, проигнорируйте это письмо.";
				}
				else
				{
					bodyBuilder.TextBody = $"Ваш код для входа: {code}\n\n" +
										  "Код действителен 15 минут.\n" +
										  "Если вы не запрашивали вход, проигнорируйте это письмо.";
				}

				message.Body = bodyBuilder.ToMessageBody();

				using var client = new MailKit.Net.Smtp.SmtpClient();
				await client.ConnectAsync(_emailSettings.SmtpClientHost, _emailSettings.SmtpClientPort,
										_emailSettings.EnableSsl);
				await client.AuthenticateAsync(_emailSettings.MailLogin, _emailSettings.MailPassword);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);

				_logger.LogInformation($"Email sent to {email}");
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to send email to {email}");
				return false;
			}
		}
	}
}
