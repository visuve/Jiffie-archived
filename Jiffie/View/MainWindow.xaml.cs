using System.Windows;

namespace Jiffie
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }
    }
}