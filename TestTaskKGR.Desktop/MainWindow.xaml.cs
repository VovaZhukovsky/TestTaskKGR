using System.Windows;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;

namespace TestTaskKGR.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(
        WpfLogger wpfLogger,
        StreamParams streamParams,
        CommonParams common)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(wpfLogger, streamParams, common);
    }
}