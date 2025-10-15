using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;
public interface IRole
{
    Task<RoleViewModel> GetAsync(int id);
    Task<RoleViewModel> GetByNameAsync(string name);
}
