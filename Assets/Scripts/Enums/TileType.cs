public enum TileType
{
    Empty = 0,
    [Ground(true)] Border = 1,
    [Ground(true)] Filled = 2,
    Trace = 3
}
