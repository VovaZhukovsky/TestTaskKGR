using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace TestTaskKGR.Desktop.Implementations;

public class ConfidentTrasholdSliderBehavior: Behavior<Slider>
{
    public static readonly DependencyProperty CommandProperty =
    DependencyProperty.Register(nameof(Command), typeof(ICommand),
        typeof(ConfidentTrasholdSliderBehavior));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.ValueChanged += ValueChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.ValueChanged -= ValueChanged;
    }

    private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (Command?.CanExecute(e) == true)
        {
            Command.Execute(e);
        }
    }
}
