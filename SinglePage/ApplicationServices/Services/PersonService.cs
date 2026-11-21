using System.Net;
using SinglePage.ApplicationServices.Contracts;
using SinglePage.ApplicationServices.Dtos;
using SinglePage.Frameworks;
using SinglePage.Frameworks.ResponseFrameworks;
using SinglePage.Frameworks.ResponseFrameworks.Contracts;
using SinglePage.Models.DomainModels.PersonAggregate;
using SinglePage.Models.Services.Contracts;
using NotImplementedException = System.NotImplementedException;

namespace SinglePage.ApplicationServices.Services;

public class PersonService : IPersonService
{
    
    private readonly IPersonRepository _personRepository;

    #region [ - Constructor - ] 

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    #endregion
    
    #region [ - GetAll() - ] 

    public async Task<IResponse<GetAllPersonDto>> GetAll()
    {
        var selectAllResponse = await _personRepository.SellectAll();
        if (selectAllResponse is null || !selectAllResponse.IsSuccessful)
        {
            return new Response<GetAllPersonDto>(false, HttpStatusCode.UnprocessableContent,
                ResponseMessages.Error, null);
        }

        var getAllPersonDto = new GetAllPersonDto() { GetPersonServiceDtos = new List<GetPersonDto>() };
        foreach (var item in selectAllResponse.Value)
        {
            var personDto = new GetPersonDto()
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email
            };
            getAllPersonDto.GetPersonServiceDtos.Add(personDto);
        }

        var response = new Response<GetAllPersonDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation,
            getAllPersonDto);
        return response;
    }
    #endregion
    
    #region [ - Get() - ] 

    public async Task<IResponse<GetPersonDto>> Get(GetPersonDto dto)
    {
        var person = new Person()
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        var selectResponse = await _personRepository.Select(person);

        if (selectResponse is null)
        {
            return new Response<GetPersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput,
                null);
        }

        if (!selectResponse.IsSuccessful)
        {
            return new Response<GetPersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.Error, null);
        }

        var getPersonServiceDto = new GetPersonDto()
        {
            Id = selectResponse.Value.Id,
            FirstName = selectResponse.Value.FirstName,
            LastName = selectResponse.Value.LastName,
            Email = selectResponse.Value.Email
        };
        var response = new Response<GetPersonDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation,
            getPersonServiceDto);
        return response;
    }

    #endregion
    
    #region [ - Post() - ] 

    public async Task<IResponse<PostPersonDto>> Post(PostPersonDto dto)
    {
        if (dto is null)
        {
            return new Response<PostPersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput,
                null);
        }

        var postedPerson = new Person()
        {
            Id = new Guid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        var insertedResponse = await _personRepository.Insert(postedPerson);

        if (!insertedResponse.IsSuccessful)
        {
            return new Response<PostPersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.Error, dto);
        }

        var response = new Response<PostPersonDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, dto);
        return response;
    }

    #endregion
    
    #region [ - Put() - ] 

    public async Task<IResponse<PutPersonDto>> Put(PutPersonDto dto)
    {
        if (dto is null || dto.Id == Guid.Empty)
        {
            return new Response<PutPersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput,
                dto);
        }

        var putedPerson = new Person()
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        var editedResponse = await _personRepository.Update(putedPerson);

        if (!editedResponse.IsSuccessful)
        {
            return new Response<PutPersonDto>(false, editedResponse.Status, editedResponse.Message, dto);
        }

        var response = new Response<PutPersonDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, dto);
        return response;
    }

    #endregion
    
    #region [ - Delete() - ] 
    public async Task<IResponse<DeletePersonDto>> Delete(DeletePersonDto dto)
    {
        if (dto is null || dto.Id == Guid.Empty)
        {
            return new Response<DeletePersonDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput,
                dto);
        }

        var removedPerson = new Person()
        {
            Id = dto.Id
        };

        var deleteResponse = await _personRepository.Delete(removedPerson);
        
        if (!deleteResponse.IsSuccessful)
        {
            return new Response<DeletePersonDto>(false, deleteResponse.Status, deleteResponse.Message, dto);
        }
        
        var response = new Response<DeletePersonDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, dto);
        return response;
    }
    #endregion
}