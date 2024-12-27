namespace Models.DTO;
public record PlayerDTO 
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string FirstName {get;set;} = "";
    public string LastName {get;set;} = "";
    public int? Age {get;set;}
    public Birth? Birth {get;set;}
    public string Nationality {get;set;} = "";
    public double? Height {get;set;}
    public int? Number {get;set;}
    public string? Position {get;set;} = "";
    public string Photo {get;set;} = "";
    public TeamDTO? Team {get;set;}
}