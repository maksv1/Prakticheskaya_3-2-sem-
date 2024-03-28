using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Audio_Plyer
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void UpdateHistoryListBox(List<string> history)
        {
            foreach (var item in history)
            {
                HistoryListBox.Items.Add(item);
            }
        }

        private void ShowHistoryWindow(List<string> history)
        {
            Window1 Window1 = new Window1();
            Window1.UpdateHistoryListBox(history);

            if (Window1.ShowDialog() == false)
            {
                Close();
            }
        }

        private void HistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
