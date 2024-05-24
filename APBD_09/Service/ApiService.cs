using APBD_09.Context;
using APBD_09.Models;

namespace APBD_09.Service;

public class ApiService
{
    private s28201DbContext _context;

    public ApiService(s28201DbContext context)
    {
        _context = context;
    }

    public List<Trip> GetAllTripsByStartDateDesc()
    {
        return [.._context.Trips.ToList().OrderByDescending(e => e.DateFrom)];
    }

}