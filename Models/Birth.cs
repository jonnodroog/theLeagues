namespace Models;

public record Birth
{
    public int Id {get;set;}
    public DateTime? Date {get;set;}
    public string Place {get;set;} = "";
    public string Country {get;set;} = "";
}