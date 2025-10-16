using System.Windows.Controls;

namespace TestTaskKGR.Desktop.UserControls.Views;

public partial class Stream1 : UserControl
{

    public Stream1()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.Stream1Control.StreamFrame = Stream1Frame;
               
            }
        };
    }
}
