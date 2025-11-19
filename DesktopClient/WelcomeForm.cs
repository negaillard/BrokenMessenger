using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DesktopClient
{
	public partial class WelcomeForm : Form
	{
		public WelcomeForm()
		{
			InitializeComponent();

			// Подписываемся на событие после полной загрузки формы
			this.Load += WelcomeForm_Load;
			this.SizeChanged += WelcomeForm_SizeChanged;
		}

		private void WelcomeForm_Load(object sender, EventArgs e)
		{
			// Теперь все элементы гарантированно созданы
			CenterControls();
		}
		#region Для визуала
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Application.Exit();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		private void WelcomeForm_SizeChanged(object sender, EventArgs e)
		{
			// Проверяем что форма уже загружена и элементы созданы
			if (this.IsHandleCreated && panelMain != null)
			{
				CenterControls();
			}
		}

		private void CenterControls()
		{
			try
			{
				if (panelMain == null || lblTitle == null)
					return;

				// Центрируем заголовок
				lblTitle.Left = (panelMain.Width - lblTitle.Width) / 2;

				// Центрируем панель кнопок
				var buttonPanel = panelMain.Controls.OfType<Panel>()
					.FirstOrDefault();
				if (buttonPanel != null)
				{
					buttonPanel.Left = (panelMain.Width - buttonPanel.Width) / 2;
					buttonPanel.Top = (panelMain.Height - buttonPanel.Height) / 2;
				}

				// Обновляем позиции кнопок выхода и сворачивания
				var exitButton = panelMain.Controls.OfType<Button>()
					.FirstOrDefault(b => b.Text == "✕");
				if (exitButton != null)
				{
					exitButton.Left = panelMain.Width - 50;
				}

				var minimizeButton = panelMain.Controls.OfType<Button>()
					.FirstOrDefault(b => b.Text == "─");
				if (minimizeButton != null)
				{
					minimizeButton.Left = panelMain.Width - 90;
				}

				// Обновляем позицию версии
				var versionLabel = panelMain.Controls.OfType<Label>()
					.FirstOrDefault(l => l.Text.StartsWith("v"));
				if (versionLabel != null)
				{
					versionLabel.Top = panelMain.Height - 30;
				}
			}
			catch (Exception ex)
			{
				// Логируем ошибку, но не падаем
				System.Diagnostics.Debug.WriteLine($"CenterControls error: {ex.Message}");
			}
		}
		#endregion

		private void BtnLogin_Click(object sender, EventArgs e)
		{
			Program.ShowLoginForm();
		}

		private void BtnRegister_Click(object sender, EventArgs e)
		{
			Program.ShowRegisterForm();
		}
	}
}
