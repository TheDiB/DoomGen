namespace DooMGen.Core.Map
{
    public class Linedef
    {
        public int Id { get; set; }
        public int StartVertex { get; set; }
        public int EndVertex { get; set; }
        public int FrontSidedef { get; set; }
        public int? BackSidedef { get; set; }
        public int Flags { get; set; } = 0;
        public int Special { get; set; } = 0;
        public int Tag { get; set; } = 0;
    }
}
