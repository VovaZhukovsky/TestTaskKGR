using System.Windows;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;

namespace TestTaskKGR.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }
}