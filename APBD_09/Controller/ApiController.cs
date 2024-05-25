using APBD_09.Context;
using APBD_09.Service;
using Microsoft.AspNetCore.Mvc;

namespace APBD_09.Controller;

[ApiController]
public class ApiController(ApiService service)
{
    [Route("api/trips")]
    [HttpGet]
    public IResult GetAllTrips(int page)
    {
        var tripsFromDb = service.GetAllTripsByStartDateDesc();
        if (page > tripsFromDb.Count || page < 1)
            return Results.BadRequest("Page should be a number between 1 and all pages.");
        var result = new
        {
            PageNum = page,
            PageSize = 10,
            AllPages = tripsFromDb.Count,
            Trips = tripsFromDb[page - 1]
        };
        return Results.Ok(result);
    }

    [Route("api/clients/{clientId:int}")]
    [HttpDelete]
    public IResult DeleteClient(int clientId)
    {
        var tripsByClient = service.GetAllTripsByClientId(clientId);

        if (tripsByClient.Count > 0)
        {
            return Results.BadRequest("Cannot delete client with existing trips.");
        }

        var wasDeleted = service.DeleteClientById(clientId);
        return !wasDeleted 
            ? Results.NotFound("Client with given id does not exist.") 
            : Results.Ok($"Client with id {clientId} was successfully deleted.");
    }

    [Route("api/trips/{idTrip:int}/clients")]
    [HttpPost]
    public IResult RegisterClientForTrip(ClientTripDto dto, int idTrip)
    {
        var peselClient = service.GetClientByPesel(dto.Pesel);

        if (peselClient != null)
        {
            return Results.BadRequest("Cannot register client that is already registered.");
        }
        
        var client = new Client
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };
        
        var tripsByClientId = service.GetAllTripsByClientId(client.IdClient);

        if (tripsByClientId.Select(t => t.IdTrip).Contains(idTrip))
        {
            return Results.BadRequest("Cannot register client for the trip twice.");
        }

        var clientTrip = service.GetTripById(dto.IdTrip);

        if (clientTrip == null)
        {
            return Results.BadRequest("Cannot register client for non-existent trip.");
        }

        if (clientTrip.DateFrom < DateTime.Now)
        {
            return Results.BadRequest("Cannot register client for the trip that has already occured.");
        }
        
        service.InsertClient(client);
        service.InsertClientToTrip(client, dto.IdTrip, dto.PaymentDate);

        return Results.Ok("Registered Successfully.");
    }
}