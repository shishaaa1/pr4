using System.Collections.Generic;
using System.Windows;

namespace ClientGUI
{
	public partial class HistoryWindow : Window
	{
		public HistoryWindow(List<string> history)
		{
			InitializeComponent();

			if (history == null || history.Count == 0)
			{
				ListHistory.Items.Add("История пуста");
			}
			else
			{
				int num = 1;
				foreach (string record in history)
				{
					ListHistory.Items.Add($"{num}. {record}");
					num++;
				}
			}
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
