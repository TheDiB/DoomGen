namespace DooMGen.Core.Map
{
    public class Sidedef
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public string UpperTexture { get; set; } = "-";
        public string LowerTexture { get; set; } = "-";
        public string MiddleTexture { get; set; } = "STARTAN2";
    }
}
