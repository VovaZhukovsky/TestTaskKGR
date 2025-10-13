using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTaskKGR.Desktop.Implementations;

namespace TestTaskKGR.Desktop;

public class MainWindowViewModel
{
    public WpfLogger LogConsole { get; set; }

    public MainWindowViewModel(WpfLogger wpfLogger)
    {
        LogConsole = wpfLogger;

    }

}