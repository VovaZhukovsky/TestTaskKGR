using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;

public class Role : RoleViewModel
{
    public required virtual ICollection<Violation> Violations {get; set;}
    public Role()
    {
        Violations = new HashSet<Violation>();
    }
}