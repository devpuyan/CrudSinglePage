using Microsoft.AspNetCore.Mvc;
using SinglePage.ApplicationServices.Contracts;
using SinglePage.ApplicationServices.Dtos;

namespace SinglePage.Controllers;

public class PersonController : Controller
{
    private readonly IPersonService _personService ;
    
    #region [ - Constructor - ] 

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    #endregion
    
    #region [ - Index() - ] 

    public IActionResult Index()
    {
        return View();
    }
    #endregion
    #region [ - GetAll() - ] 

    public async Task<IActionResult> GetAll()
    {
        var guard = Guard_PersonService();
        if (guard is not null) return guard;

        try
        {

            var getAllResponse = await _personService.GetAll();
            var response = getAllResponse.Value.GetPersonServiceDtos;
            return Json(response);

        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }
    #endregion
    #region [ - Get() - ] 

    public async Task<IActionResult> Get(GetPersonDto dto)
    {
        Guard_PersonService();
        var getResponse = await _personService.Get(dto);
        var response = getResponse.Value;
        if (response is null)
        {
            return Json("NotFound");
        }
        return Json(response);
    }
    #endregion
    
    #region [ - Post() - ] 
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostPersonDto dto)
    {
        Guard_PersonService();
        var postedDto = new GetPersonDto() { Email = dto.Email };
        var getResponse = await _personService.Get(postedDto);

        if (ModelState.IsValid && getResponse.Value is null)
        {
            var postResponse = await _personService.Post(dto);
            return postResponse.IsSuccessful ? Ok() : BadRequest();
        }
        else if (ModelState.IsValid && getResponse.Value is not null)
        {
            return Conflict(dto);
        }
        else
        {
            return BadRequest();
        }
    }
    
    #endregion
    
    #region [ - Put() - ] 

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] PutPersonDto dto)
    {
        var guard = Guard_PersonService();
        if (guard is not null) return guard; // 🔴 اینجا خروجی رو چک می‌کنیم

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var putResponse = await _personService.Put(dto);
    
        return putResponse.IsSuccessful ? Ok() : BadRequest(putResponse.Message);
    }
    
    #endregion
    
    #region [ - Delete() - ] 

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletePersonDto dto)
    {
        var guard = Guard_PersonService();
        if (guard is not null) return guard;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleteResponse = await _personService.Delete(dto);
    
        return deleteResponse.IsSuccessful ? Ok() : BadRequest(deleteResponse.Message);
    }
    
    #endregion
    
    #region [ - Guard() - ] 

    private ObjectResult Guard_PersonService()
    {
        if (_personService is null)
        {
            return Problem($" {nameof(_personService)} is null.");
        }

        return null;
    } 
    #endregion
}