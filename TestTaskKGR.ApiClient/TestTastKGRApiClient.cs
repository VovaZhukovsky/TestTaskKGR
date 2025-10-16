namespace TestTaskKGR.ApiClient;

public partial class TestTaskKGRApiClient(HttpClient httpClient) : ITestTaskKGRApiClient
{
    public IRole Role { get; } = new RoleClient(httpClient);
    public IType Type { get; } = new TypeClient(httpClient);
    public IViolation Violation { get; } = new ViolationClient(httpClient);
}
