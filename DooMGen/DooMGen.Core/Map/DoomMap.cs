namespace DooMGen.Core.Map
{
    public class DoomMap
    {
        public string Name { get; set; } = "MAP01";
        public List<Vertex> Vertices { get; } = new();
        public List<Sector> Sectors { get; } = new();
        public List<Sidedef> Sidedefs { get; } = new();
        public List<Linedef> Linedefs { get; } = new();
        public List<Thing> Things { get; } = new();
    }
}
