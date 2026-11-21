using SinglePage.Models.DomainModels.PersonAggregate;

namespace SinglePage.Models.Services.Contracts;

public interface IPersonRepository : IRepository<Person, IEnumerable<Person>>
{
    
}