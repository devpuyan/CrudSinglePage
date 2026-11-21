using System.Net;
using Azure;
using Microsoft.EntityFrameworkCore;
using SinglePage.Frameworks;
using SinglePage.Frameworks.ResponseFrameworks.Contracts;
using SinglePage.Models.DomainModels.PersonAggregate;
using SinglePage.Models.Services.Contracts;

namespace SinglePage.Models.Services.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly ProjectDbContext _dbContext;

    #region [ - constructor - ]

    public PersonRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion


    #region [ - SelectAll() - ] 

    public async Task<IResponse<IEnumerable<Person>>> SellectAll()
    {
        try
        {
            var persons = await _dbContext.Person.AsNoTracking().ToListAsync();
            return persons.Count == 0
                ? new Frameworks.ResponseFrameworks.Response<IEnumerable<Person>>(false,
                    HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null)
                : new Frameworks.ResponseFrameworks.Response<IEnumerable<Person>>(true, HttpStatusCode.OK,
                    ResponseMessages.SuccessfullOperation, persons);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region [ - Select() - ] 

    public async Task<IResponse<Person>> Select(Person person)
    {
        try
        {
            var responseValue = new Person();
            if (person.Id.ToString() != "")
            {
                //responseValue = await _projectDbContext.Person.FindAsync(person.Email);
                responseValue = await _dbContext.Person.Where(c=>c.Email==person.Email).SingleOrDefaultAsync();
            }
            else
            {
                responseValue = await _dbContext.Person.FindAsync(person.Id);
            }
            return responseValue is null ?
                new Frameworks.ResponseFrameworks.Response<Person>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null) :
                new Frameworks.ResponseFrameworks.Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, responseValue);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    } 
    #endregion
    
    #region [ - Insert() - ] 

    
    public async Task<IResponse<Person>> Insert(Person person)
    {
            try
            {
                if (person is null)
                {
                    return new Frameworks.ResponseFrameworks.Response<Person>(false,
                        HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
                }

                await _dbContext.AddAsync(person);
                await _dbContext.SaveChangesAsync(); 
                var response = new Frameworks.ResponseFrameworks.Response<Person>(true, HttpStatusCode.OK,
                    ResponseMessages.SuccessfullOperation, person);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    #endregion
    
    #region [ - Update() - ] 

    public async Task<IResponse<Person>> Update(Person person)
    {
        try
        {
            if (person is null || person.Id == Guid.Empty)
            {
                return new Frameworks.ResponseFrameworks.Response<Person>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
            }

            var existingPerson = await _dbContext.Person.FindAsync(person.Id);
            if (existingPerson is null)
            {
                return new Frameworks.ResponseFrameworks.Response<Person>(false, HttpStatusCode.NotFound, ResponseMessages.Error, null);
            }

            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.Email = person.Email;

            await _dbContext.SaveChangesAsync();
        
            return new Frameworks.ResponseFrameworks.Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingPerson);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    #endregion
    #region [ - Delete() - ] 

    public async Task<IResponse<Person>> Delete(Person person)
    {
        try
        {
            if (person is null || person.Id == Guid.Empty)
            {
                return new Frameworks.ResponseFrameworks.Response<Person>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
            }

            var existingPerson = await _dbContext.Person.FindAsync(person.Id);
            if (existingPerson is null)
            {
                return new Frameworks.ResponseFrameworks.Response<Person>(false, HttpStatusCode.NotFound, ResponseMessages.Error, null);
            }

            _dbContext.Person.Remove(existingPerson);
            await _dbContext.SaveChangesAsync();

            return new Frameworks.ResponseFrameworks.Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingPerson);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    #endregion
}