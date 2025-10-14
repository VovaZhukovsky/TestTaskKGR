using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SkiaSharp.Views.WPF;

namespace TestTaskKGR.Desktop.UserControls.Views
{
    /// <summary>
    /// Interaction logic for Stream1Control.xaml
    /// </summary>
    public partial class Stream1Control : UserControl
    {
        public SKElement Stream1FrameControl => Stream1Frame;

        public Stream1Control()
        {
            InitializeComponent();
        }
    }
}
