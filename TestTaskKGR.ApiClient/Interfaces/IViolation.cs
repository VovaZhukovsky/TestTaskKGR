using TestTaskKGR.ViewModel;

namespace TestTaskKGR.ApiClient;
public interface IViolation
{
    Task<ViolationViewModel> GetAsync(int id);
    Task PostAsync(ViolationViewModel model);
}
