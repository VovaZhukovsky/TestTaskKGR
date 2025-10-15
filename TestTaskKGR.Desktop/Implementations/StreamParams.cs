namespace TestTaskKGR.Desktop.Implementations;

public class StreamParams
{
    public bool IsRunDetection { get; set; } = false;
    public bool IsTrackingEnabled { get; set; } = false;
    public bool IsFilterEnabled { get; set; } = false;
    public double ConfidenceThreshold { get; set; } = 0.8;
}
