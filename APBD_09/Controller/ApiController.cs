using APBD_09.Service;
using Microsoft.AspNetCore.Mvc;

namespace APBD_09.Controller;

[ApiController]
public class ApiController
{

    private ApiService _service;

    public ApiController(ApiService service)
    {
        _service = service;
    }

    [Route("api/trips")]
    [HttpGet]
    public IResult GetAllTrips()
    {
        
        return Results.Ok(_service.GetAllTripsByStartDateDesc());
    }
}