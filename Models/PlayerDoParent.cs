using System;

namespace Models;

public record PlayerDoParent
{
    public required List<PlayerDo> Response {get;set;} = new();
} 
