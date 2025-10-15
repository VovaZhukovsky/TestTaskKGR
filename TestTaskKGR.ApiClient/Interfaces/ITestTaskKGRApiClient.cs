namespace TestTaskKGR.ApiClient;
public interface ITestTaskKGRApiClient
{
    IRole Role { get; }
    IType Type { get; }
    IViolation Violation { get; }
}
