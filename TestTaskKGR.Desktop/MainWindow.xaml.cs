using System.Windows;
using TestTaskKGR.Desktop.Implementations;

namespace TestTaskKGR.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(
        WpfLogger wpfLogger, 
        SKPaintSurfaceBehavior sKPaintSurfaceBehavior,
        RunDetection runDetection)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(wpfLogger, sKPaintSurfaceBehavior, runDetection);
    }
}