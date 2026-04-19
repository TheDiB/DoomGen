using DooMGen.Core.Map;
using System.Text;

namespace DooMGen.Core.Export
{
    public static class UdmfExporter
    {
        public static string Export(DoomMap map, bool zdoomMode)
        {
            var sb = new StringBuilder();

            // Toujours la première ligne
            sb.AppendLine("namespace = \"UDMF\";");
            sb.AppendLine();

            // ---------------------------------------------------------
            // THINGS
            // ---------------------------------------------------------
            foreach (var thing in map.Things)
            {
                sb.AppendLine("thing");
                sb.AppendLine("{");
                sb.AppendLine($"    x = {thing.X};");
                sb.AppendLine($"    y = {thing.Y};");
                sb.AppendLine($"    angle = {thing.Angle};");
                sb.AppendLine($"    type = {thing.Type};");
                sb.AppendLine($"    flags = {thing.Flags};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ---------------------------------------------------------
            // VERTICES
            // ---------------------------------------------------------
            foreach (var v in map.Vertices)
            {
                sb.AppendLine("vertex");
                sb.AppendLine("{");
                sb.AppendLine($"    x = {v.X};");
                sb.AppendLine($"    y = {v.Y};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ---------------------------------------------------------
            // LINEDEFS
            // ---------------------------------------------------------
            foreach (var l in map.Linedefs)
            {
                sb.AppendLine("linedef");
                sb.AppendLine("{");
                sb.AppendLine($"    v1 = {l.StartVertex};");
                sb.AppendLine($"    v2 = {l.EndVertex};");
                sb.AppendLine($"    sidefront = {l.FrontSidedef};");

                if (l.BackSidedef is int back)
                    sb.AppendLine($"    sideback = {back};");

                sb.AppendLine($"    flags = {l.Flags};");
                sb.AppendLine($"    special = {l.Special};");
                sb.AppendLine($"    arg0 = {l.Tag};");
                sb.AppendLine($"    arg1 = 0;");
                sb.AppendLine($"    arg2 = 0;");
                sb.AppendLine($"    arg3 = 0;");
                sb.AppendLine($"    arg4 = 0;");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ---------------------------------------------------------
            // SIDEDEFS
            // ---------------------------------------------------------
            foreach (var s in map.Sidedefs)
            {
                sb.AppendLine("sidedef");
                sb.AppendLine("{");
                sb.AppendLine($"    sector = {s.SectorId};");
                sb.AppendLine($"    texturetop = \"{s.UpperTexture}\";");
                sb.AppendLine($"    texturebottom = \"{s.LowerTexture}\";");
                sb.AppendLine($"    texturemiddle = \"{s.MiddleTexture}\";");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // ---------------------------------------------------------
            // SECTORS
            // ---------------------------------------------------------
            foreach (var sec in map.Sectors)
            {
                sb.AppendLine("sector");
                sb.AppendLine("{");
                sb.AppendLine($"    heightfloor = {sec.FloorHeight};");
                sb.AppendLine($"    heightceiling = {sec.CeilingHeight};");
                sb.AppendLine($"    texturefloor = \"{sec.FloorTexture}\";");
                sb.AppendLine($"    textureceiling = \"{sec.CeilingTexture}\";");
                sb.AppendLine($"    lightlevel = {sec.LightLevel};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}