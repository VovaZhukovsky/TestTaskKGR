using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;

public partial class TestTaskKGRApiClient: ITestTaskKGRApiClient
{
    private class TypeClient(HttpClient httpClient) : BaseClient(httpClient), IType
    {
        private readonly string routeUri = "Type";

        public async Task<TypeViewModel> GetAsync(int id)
        {
            string uri = $"{routeUri}/{id}";
            return await GetAsync<TypeViewModel>(uri);
        }
        public async Task<TypeViewModel> GetByNameAsync(string name)
        {
            string uri = $"{routeUri}/{name}";
            return await GetAsync<TypeViewModel>(uri);
        }
    }
}