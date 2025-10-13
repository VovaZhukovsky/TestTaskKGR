using Mapster;
using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;
public class MapsterConfig
{
    public MapsterConfig()
    {
        TypeAdapterConfig<TypeViewModel,Type>.NewConfig();
        TypeAdapterConfig<RoleViewModel,Role>.NewConfig();
        TypeAdapterConfig<ViolationViewModel, Violation>.NewConfig();
    }
}