using Coursework;
using System.Windows;

namespace Курсовая
{
    public partial class Window1 : Window
    {
        private MainWindow mainWindow;

        public Window1(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        // Обработчик нажатия кнопки для возврата к основному окну
        private void ReturnToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show(); // Показать основное окно
            Close(); // Закрыть текущее окно
        }
    }
}
