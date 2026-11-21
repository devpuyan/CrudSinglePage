using SinglePage.ApplicationServices.Dtos;
using SinglePage.Frameworks.ResponseFrameworks.Contracts;

namespace SinglePage.ApplicationServices.Contracts;

public interface IPersonService : IService<PostPersonDto, GetPersonDto,GetAllPersonDto,PutPersonDto,DeletePersonDto>
{
    
}