using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestTaskKGR.Desktop.Implementations;
using WebcamDemo;

namespace TestTaskKGR.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(WpfLogger wpfLogger, StreamRunner streamRunner)
    {
        var skElement = Stream1View.Stream1FrameControl;

        DataContext = new MainWindowViewModel(this, wpfLogger,streamRunner);
        InitializeComponent();

    }
}