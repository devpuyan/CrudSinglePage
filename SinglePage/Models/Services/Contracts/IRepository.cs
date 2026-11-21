using SinglePage.Frameworks.ResponseFrameworks.Contracts;

namespace SinglePage.Models.Services.Contracts;

public interface IRepository<T, TCollection>
{
    Task<IResponse<TCollection>> SellectAll();
    Task<IResponse<T>> Select(T Obj);
    Task<IResponse<T>> Insert(T Obj);
    Task<IResponse<T>> Update(T Obj);
    Task<IResponse<T>> Delete(T Obj);
}