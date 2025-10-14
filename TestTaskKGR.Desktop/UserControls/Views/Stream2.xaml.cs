using System.Windows.Controls;

namespace TestTaskKGR.Desktop.UserControls.Views;

public partial class Stream2 : UserControl
{
    public Stream2()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.Stream2Control.StreamFrame = Stream2Frame;
            }
        };
    }
}
