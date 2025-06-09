namespace Models;
public record Venue
 {
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Address {get;set;} = "";
    public string City {get;set;} = "";
    public int? Capacity {get;set;}
    public string Surface {get;set;} = "";
    public string Image {get;set;} = "";
 }