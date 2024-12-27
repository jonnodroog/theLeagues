using System;

namespace Models;

public record PlayerDo
{
    public required List<Player> Players {get;set;} = new();
}
