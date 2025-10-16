using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;
public interface IType
{
    Task<TypeViewModel> GetAsync(int id);
    Task<TypeViewModel> GetByNameAsync(string name);
}
