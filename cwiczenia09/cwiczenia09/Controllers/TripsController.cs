using cwiczenia09.Data;
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
        //zwraca blad jak klient ma przypisana jakas wycieczke
        
        return Ok();
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip()
    {
        //zwraca blad jak klient o podanym peselu istnieje
        //zwraca blad jak klient o podanym peselu jest juz przypisany do tej wycieczki
        //zwraca blad jak wycieczka nie istnieje lub juz sie odbyla
        //PaymentDate null tylko dla klientow ktorzy nie zaplacili jeszcze
        //RegisteredAt ma sie zgadzac w tabeli clientTrip
        
        return Ok();
    }
}