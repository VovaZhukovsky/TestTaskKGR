using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;

public partial class TestTaskKGRApiClient: ITestTaskKGRApiClient
{
    private class ViolationClient(HttpClient httpClient) : BaseClient(httpClient), IViolation
    {
        private readonly string routeUri = "Violation";

        public async Task<ViolationViewModel> GetAsync(int id)
        {
            string uri = $"{routeUri}/{id}";
            return await GetAsync<ViolationViewModel>(uri);
        }
        public async Task PostAsync(ViolationViewModel model)
        {
            string uri = $"{routeUri}";
            await PostAsync<ViolationViewModel,ViolationViewModel>(uri,model);
        }
    }
}