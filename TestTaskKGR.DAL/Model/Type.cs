using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;

public class Type : TypeViewModel
{
    public required virtual ICollection<Violation> Violations {get; set;}
    public Type()
    {
        Violations = new HashSet<Violation>();
    }
}