namespace DooMGen.Core.Map
{
    public class Room
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public double CenterX => X + Width / 2.0;
        public double CenterY => Y + Height / 2.0;

        public int SectorId { get; set; } // pour plus tard si tu veux plusieurs secteurs
    }
}
