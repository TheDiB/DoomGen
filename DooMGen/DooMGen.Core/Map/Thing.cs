namespace DooMGen.Core.Map
{
    public class Thing
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Angle { get; set; } = 0;
        public int Type { get; set; } = 1; // Player 1 start
        public int Flags { get; set; } = 7; // Easy/Med/Hard
    }
}
