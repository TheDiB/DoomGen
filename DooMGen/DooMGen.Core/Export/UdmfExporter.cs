using DooMGen.Core.Map;
using System.Globalization;
using System.Text;

namespace DooMGen.Core.Export
{
    public static class UdmfExporter
    {
        private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

        public static string Export(DoomMap map, bool ZDoomMode)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace = \"" + (ZDoomMode ? "ZDoom" : "ZDBSP") + "\";"); // ou \"Doom\" selon port
            sb.AppendLine();

            foreach (var thing in map.Things)
            {
                sb.AppendLine("thing");
                sb.AppendLine("{");
                sb.AppendLine($"    x = {thing.X.ToString(Ci)};");
                sb.AppendLine($"    y = {thing.Y.ToString(Ci)};");
                sb.AppendLine($"    angle = {thing.Angle};");
                sb.AppendLine($"    type = {thing.Type};");
                sb.AppendLine($"    flags = {thing.Flags};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            foreach (var vertex in map.Vertices)
            {
                sb.AppendLine("vertex");
                sb.AppendLine("{");
                sb.AppendLine($"    x = {vertex.X.ToString(Ci)};");
                sb.AppendLine($"    y = {vertex.Y.ToString(Ci)};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            foreach (var sector in map.Sectors)
            {
                sb.AppendLine("sector");
                sb.AppendLine("{");
                sb.AppendLine($"    heightfloor = {sector.FloorHeight.ToString(Ci)};");
                sb.AppendLine($"    heightceiling = {sector.CeilingHeight.ToString(Ci)};");
                sb.AppendLine($"    texturefloor = \"{sector.FloorTexture}\";");
                sb.AppendLine($"    textureceiling = \"{sector.CeilingTexture}\";");
                sb.AppendLine($"    lightlevel = {sector.LightLevel};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            foreach (var linedef in map.Linedefs)
            {
                sb.AppendLine("linedef");
                sb.AppendLine("{");
                sb.AppendLine($"    v1 = {linedef.StartVertex};");
                sb.AppendLine($"    v2 = {linedef.EndVertex};");
                sb.AppendLine($"    sidefront = {linedef.FrontSidedef};");
                if (linedef.BackSidedef is int back)
                    sb.AppendLine($"    sideback = {back};");
                sb.AppendLine($"    flags = {linedef.Flags};");
                sb.AppendLine($"    special = {linedef.Special};");
                sb.AppendLine($"    arg0 = {linedef.Tag};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            foreach (var sidedef in map.Sidedefs)
            {
                sb.AppendLine("sidedef");
                sb.AppendLine("{");
                sb.AppendLine($"    sector = {sidedef.SectorId};");
                sb.AppendLine($"    texturetop = \"{sidedef.UpperTexture}\";");
                sb.AppendLine($"    texturebottom = \"{sidedef.LowerTexture}\";");
                sb.AppendLine($"    texturemiddle = \"{sidedef.MiddleTexture}\";");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
