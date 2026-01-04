namespace Models;

public record Player
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? Age { get; set; }
    public int? Number { get; set; }
    public string? Position { get; set; } = "";
    public string Photo { get; set; } = "";
    public string Nationality { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
}