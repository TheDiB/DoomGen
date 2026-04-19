using DooMGen.Core.Map;
using System.Text;

namespace DooMGen.Core.Export
{
    public static class MapInfoExporter
    {
        public static string Export(DoomMap map, bool zdoomMode, string mapName)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"map {map.Name} \"Generated Map\"");
            sb.AppendLine("{");
            sb.AppendLine("    levelnum = 1");
            sb.AppendLine("    sky1 = \"SKY1\"" + (zdoomMode ? ", 0" : ""));
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
