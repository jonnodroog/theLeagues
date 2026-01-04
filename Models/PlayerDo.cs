using System;

namespace Models;

public record PlayerDo
{
    public required Player Player {get;set;} = new();
}
