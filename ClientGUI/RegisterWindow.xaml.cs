using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using Common;

namespace ClientGUI
{
	public partial class RegisterWindow : Window
	{
		private string serverIP;
		private int serverPort;

		public RegisterWindow(string ip, int port)
		{
			InitializeComponent();
			serverIP = ip;
			serverPort = port;
		}

		private void BtnBrowse_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog
			{
				CheckFileExists = false,
				CheckPathExists = true,
				FileName = "Выберите папку",
				Title = "Выбор папки"
			};

			if (dialog.ShowDialog() == true)
			{
				TxtPath.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
			}
		}

		private void BtnRegister_Click(object sender, RoutedEventArgs e)
		{
			string login = TxtLogin.Text;
			string password = TxtPassword.Password;
			string path = TxtPath.Text;

			if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(path))
			{
				MessageBox.Show("Заполните все поля!", "Ошибка");
				return;
			}

			try
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
				socket.Connect(endPoint);

				ViewModelSend request = new ViewModelSend
				{
					Message = $"register {login} {password} {path}",
					Id = -1
				};

				string json = JsonConvert.SerializeObject(request);
				byte[] data = Encoding.UTF8.GetBytes(json);
				socket.Send(data);

				byte[] buffer = new byte[1024 * 1024];
				int received = socket.Receive(buffer);
				string response = Encoding.UTF8.GetString(buffer, 0, received);

				socket.Shutdown(SocketShutdown.Both);
				socket.Close();

				ViewModelMessage result = JsonConvert.DeserializeObject<ViewModelMessage>(response);
				MessageBox.Show(result.Message, "Результат");

				if (result.Message.Contains("успешна"))
				{
					this.DialogResult = true;
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
			}
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}
