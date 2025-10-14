using System.Windows;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;

namespace TestTaskKGR.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(
        WpfLogger wpfLogger, 
        SKPaintSurfaceBehavior sKPaintSurfaceBehavior,
        StreamParams runDetection,
        CommonParams common)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(wpfLogger, sKPaintSurfaceBehavior, runDetection,common);
    }
}