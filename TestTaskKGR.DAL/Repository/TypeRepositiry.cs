using Mapster;
using TestTaskKGR.ViewModel;

namespace TestTaskKGR.DAL;
public class TypeRepository : IRepository<TypeViewModel>
{
    private readonly ApplicationContext _dbcontext;
    private bool disposed = false;

    public TypeRepository(ApplicationContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public IEnumerable<TypeViewModel> GetItemList()
    {
        return _dbcontext.Types.Select(c => c.Adapt<TypeViewModel>());
    }
    public TypeViewModel? GetItem(int id)
    {
        return _dbcontext.Types.Find(id).Adapt<TypeViewModel>();
    }
    public TypeViewModel? GetItemByName(string name)
    {
        return _dbcontext.Types.Where(t => t.Name == name).FirstOrDefault().Adapt<TypeViewModel>();
    }
    public TypeViewModel Create(TypeViewModel Type)
    {
        return _dbcontext.Types.Add(Type.Adapt<Type>()).Entity.Adapt<TypeViewModel>();
    }
    public void Update(TypeViewModel currency)
    {
        _dbcontext.Types.Update(currency.Adapt<Type>());
    }
    public void Delete(int id)
    {
        var currency = _dbcontext.Types.Find(id);

        if (currency is not null)
            _dbcontext.Types.Remove(currency);
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