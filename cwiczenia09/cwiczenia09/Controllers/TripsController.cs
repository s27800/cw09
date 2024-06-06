using cwiczenia09.Data;
using cwiczenia09.Models;
using cwiczenia09.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cwiczenia09.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly Apbd09Context _context;

    public TripsController(Apbd09Context context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _context.Trips.Select(e => new TripGetDTO()
            {
                Name = e.Name,
                Description = e.Description,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                MaxPeople = e.MaxPeople,
                Countries = e.IdCountries.Select(f => new CountryGetDTO()
                {
                    Name = f.Name
                }),
                Clients = e.ClientTrips.Select(f => new ClientGetDTO()
                {
                    FirstName = f.IdClientNavigation.FirstName,
                    LastName = f.IdClientNavigation.LastName
                })
            })
            .OrderBy(e => e.DateFrom)
            .ToListAsync();

        return Ok(trips);
    }

    [HttpGet("{pageNum}")]
    public async Task<IActionResult> GetTrips(int pageNum)
    {
        if (pageNum < 1)
            return BadRequest("Wrong page number.");
        
        int pageSize = 10;
        
        var trips = await _context.Trips.Select(e => new TripGetDTO()
            {
                Name = e.Name,
                Description = e.Description,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                MaxPeople = e.MaxPeople,
                Countries = e.IdCountries.Select(f => new CountryGetDTO()
                {
                    Name = f.Name
                }),
                Clients = e.ClientTrips.Select(f => new ClientGetDTO()
                {
                    FirstName = f.IdClientNavigation.FirstName,
                    LastName = f.IdClientNavigation.LastName
                })
            })
            .OrderBy(e => e.DateFrom)
            .ToListAsync();

        int numOfPages = (trips.Count-1) / pageSize + 1;
        
        if (pageNum > numOfPages)
            return BadRequest("There is no page number " + pageNum + ".");
        
        var tripsOnPage = new List<TripGetDTO>();

        for (int i = (pageNum - 1) * pageSize; i < trips.Count && i < pageNum * pageSize; i++)
        {
            tripsOnPage.Add(trips[i]);
        }

        var page = new {  pageNum, pageSize, allPages = numOfPages, trips = tripsOnPage};

        return Ok(page);
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        //bad request if client does not exist
        var doesClientExist = _context.Clients.Any(c => c.IdClient == idClient);
        
        if (!doesClientExist)
            return BadRequest("Client does not exist.");
        
        //bad request if client is assigned to trips
        var doesClientHaveTrips = _context.ClientTrips.Any(c => c.IdClient == idClient);
        
        if (doesClientHaveTrips)
            return BadRequest("Client cannot be assigned to any trips.");

        //remove client
        var client = await _context.Clients.FirstAsync(c => c.IdClient == idClient);
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        
        return Ok("Deleted client with id " + idClient + ".");
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, AddClientToTrip obj)
    {
        //bad request if client does not exist
        var doesClientExist = _context.Clients.Any(c => c.Pesel == obj.Pesel);

        if (!doesClientExist)
            return BadRequest("Client does not exist.");
        
        //bad request if client is already assigned to the trip
        var client = await _context.Clients.Where(c => c.Pesel == obj.Pesel).FirstAsync();
        var isClientAssignedToTrip = _context.ClientTrips.Any(c => c.IdClient == client.IdClient && c.IdTrip == idTrip);

        if (isClientAssignedToTrip)
            return BadRequest("Client is already assigned to that trip.");
        
        //bad request if trip does not exist
        var doesTripExist = _context.Trips.Any(t => t.IdTrip == idTrip);

        if (!doesTripExist)
            return BadRequest("Trip does not exist.");
        
        //bad request if trip was in the past
        var tripDate = await _context.Trips.Where(t => t.IdTrip == idTrip).Select(t => t.DateFrom).FirstAsync();

        if (tripDate <= DateTime.Today)
            return BadRequest("This trip was in the past.");

        var clientTrip = new ClientTrip()
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = obj.PaymentDate
        };

        await _context.ClientTrips.AddAsync(clientTrip);
        await _context.SaveChangesAsync();
        
        return Ok("Assigned client to the trip.");
    }
}