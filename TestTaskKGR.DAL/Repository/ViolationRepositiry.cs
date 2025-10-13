using Mapster;
using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;
public class ViolationRepository : IRepository<ViolationViewModel>
{
    private readonly ApplicationContext _dbcontext;
    private bool disposed = false;

    public ViolationRepository(ApplicationContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public IEnumerable<ViolationViewModel> GetItemList()
    {
        return _dbcontext.Violations.Select(c => c.Adapt<ViolationViewModel>());
    }
    public ViolationViewModel? GetItem(int id)
    {
        return _dbcontext.Violations.Find(id).Adapt<ViolationViewModel>();
    }
    public ViolationViewModel Create(ViolationViewModel Violation)
    {
        return _dbcontext.Violations.Add(Violation.Adapt<Violation>()).Entity.Adapt<ViolationViewModel>();
    }
    public void Update(ViolationViewModel currency)
    {
        _dbcontext.Violations.Update(currency.Adapt<Violation>());
    }
    public void Delete(int id)
    {
        var currency = _dbcontext.Violations.Find(id);

        if (currency is not null)
            _dbcontext.Violations.Remove(currency);
    }
    public void Save()
    {
        _dbcontext.SaveChanges();
    }

    public virtual void Dispose(bool disposing)
    {
        if(disposed)
        {
            if(disposing)
            {
                _dbcontext.Dispose();
            }
        }
        disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}