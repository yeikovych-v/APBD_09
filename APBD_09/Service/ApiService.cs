using APBD_09.Context;
using APBD_09.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_09.Service;

public class ApiService(Apbd09Context context)
{
    public List<object> GetAllTripsByStartDateDesc()
    {
        return
        [
            ..context.Trips
                .Include(trip => trip.IdCountries)
                .Include(trip => trip.ClientTrips)
                .ToList()
                .OrderByDescending(t => t.DateFrom)
                .Select(t =>
                    new
                    {
                        t.Name,
                        t.Description,
                        t.DateFrom,
                        t.DateTo,
                        t.MaxPeople,
                        Countries = t.IdCountries
                            .Select(c => c.Name),
                        Clients = t.ClientTrips
                            .Join(
                                context.Clients,
                                ct => ct.IdClient,
                                cl => cl.IdClient,
                                (ct, cl) => new { cl.FirstName, cl.LastName }
                            )
                    }
                )
        ];
    }

    public Client? GetClientById(int id)
    {
        return context.Clients.FirstOrDefault(c => c.IdClient == id);
    }

    public List<Trip> GetAllTripsByClientId(int clientId)
    {
        return
        [
            ..context.Trips
                .Join(
                    context.ClientTrips,
                    t => t.IdTrip,
                    ct => ct.IdTrip,
                    (t, ct) => new { t, ct.IdClient }
                )
                .Where(j => j.IdClient == clientId)
                .Select(j => j.t)
        ];
    }


    public bool DeleteClientById(int clientId)
    {
        var client = GetClientById(clientId);
        if (client == null) return false;
        context.Remove(client);
        context.SaveChanges();
        return true;
    }

    public Client? GetClientByPesel(string pesel)
    {
        return context.Clients.FirstOrDefault(c => c.Pesel == pesel);
    }

    public Trip? GetTripById(int id)
    {
        return context.Trips.FirstOrDefault(t => t.IdTrip == id);
    }

    public void InsertClient(Client client)
    {
        context.Clients.Add(client);
        context.SaveChanges();
    }

    public void InsertClientToTrip(Client client, int idTrip, DateTime paymentDate)
    {
        context.ClientTrips.Add(new ClientTrip()
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = paymentDate
        });
        context.SaveChanges();
    }
}