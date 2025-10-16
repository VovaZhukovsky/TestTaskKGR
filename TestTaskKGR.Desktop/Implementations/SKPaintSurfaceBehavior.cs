using Microsoft.Xaml.Behaviors;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Input;
using System.Windows;

namespace TestTaskKGR.Desktop.Implementations;

public class SKPaintSurfaceBehavior : Behavior<SKElement>
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand),
            typeof(SKPaintSurfaceBehavior));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PaintSurface += OnPaintSurface;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PaintSurface -= OnPaintSurface;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (Command?.CanExecute(e) == true)
        {
            Command.Execute(e);
        }
    }
}
