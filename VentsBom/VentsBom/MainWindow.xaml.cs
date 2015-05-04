using System.Windows;
using System.Windows.Input;

namespace VentsBom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           Window.Children.Add(new BomControl());
        }
    }
}
