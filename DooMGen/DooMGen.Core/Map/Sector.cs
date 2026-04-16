namespace DooMGen.Core.Map
{
    public class Sector
    {
        public int Id { get; set; }
        public double FloorHeight { get; set; } = 0;
        public double CeilingHeight { get; set; } = 128;
        public string FloorTexture { get; set; } = "FLOOR0_1";
        public string CeilingTexture { get; set; } = "CEIL1_1";
        public int LightLevel { get; set; } = 160;
    }
}
