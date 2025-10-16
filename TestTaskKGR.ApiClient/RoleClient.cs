using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;

public partial class TestTaskKGRApiClient: ITestTaskKGRApiClient
{
    private class RoleClient(HttpClient httpClient) : BaseClient(httpClient), IRole
    {
        private readonly string routeUri = "Role";

        public async Task<RoleViewModel> GetAsync(int id)
        {
            string uri = $"{routeUri}/{id}";
            return await GetAsync<RoleViewModel>(uri);
        }
                public async Task<RoleViewModel> GetByNameAsync(string name)
        {
            string uri = $"{routeUri}/{name}";
            return await GetAsync<RoleViewModel>(uri);
        }
    }
}