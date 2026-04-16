namespace DooMGen.Core.Map
{
    public class Room
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double CenterX => X + Width / 2;
        public double CenterY => Y + Height / 2;
    }
}
