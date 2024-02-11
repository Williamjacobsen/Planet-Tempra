using System;

public static class Inventory
{
    public static string PickUpCollider { get; set; } 
    public static Tuple<string, int>[] items = new Tuple<string, int>[8]; // string: name of plant/seed, int: number of plants/seeds
}