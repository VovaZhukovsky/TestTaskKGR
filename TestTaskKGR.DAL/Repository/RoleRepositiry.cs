using Mapster;
using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;
public class RoleRepository : IRepository<RoleViewModel>
{
    private readonly ApplicationContext _dbcontext;
    private bool disposed = false;

    public RoleRepository(ApplicationContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public IEnumerable<RoleViewModel> GetItemList()
    {
        return _dbcontext.Roles.Select(c => c.Adapt<RoleViewModel>());
    }
    public RoleViewModel? GetItem(int id)
    {
        return _dbcontext.Roles.Find(id).Adapt<RoleViewModel>();
    }
    public RoleViewModel Create(RoleViewModel role)
    {
        return _dbcontext.Roles.Add(role.Adapt<Role>()).Entity.Adapt<RoleViewModel>();
    }
    public void Update(RoleViewModel currency)
    {
        _dbcontext.Roles.Update(currency.Adapt<Role>());
    }
    public void Delete(int id)
    {
        var currency = _dbcontext.Roles.Find(id);

        if (currency is not null)
            _dbcontext.Roles.Remove(currency);
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