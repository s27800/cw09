namespace cwiczenia09.Models.DTOs;

public class TripGetDTO
{
    public string Name { get; set; }
    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }
    public IEnumerable<CountryGetDTO> Countries { get; set; } = new List<CountryGetDTO>();
    public IEnumerable<ClientGetDTO> Clients { get; set; } = new List<ClientGetDTO>();
}