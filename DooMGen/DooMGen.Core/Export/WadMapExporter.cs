using DooMGen.Core.Map;
using DooMGen.Core.WAD;
using System.Text;

namespace DooMGen.Core.Export
{
    public static class WadMapExporter
    {
        public static void ExportToWad(DoomMap map, string filePath, bool ZDoomMode)
        {
            var wad = new WadWriter();

            // MAP01 (vide)
            wad.AddLump(map.Name, Array.Empty<byte>());

            // TEXTMAP
            var udmf = UdmfExporter.Export(map, ZDoomMode);
            wad.AddLump("TEXTMAP", Encoding.UTF8.GetBytes(udmf));

            // ENDMAP
            wad.AddLump("ENDMAP", Array.Empty<byte>());

            // MAPINFO
            var mapinfo = MapInfoExporter.Export(map);
            wad.AddLump(ZDoomMode ? "ZMAPINFO" : "MAPINFO", Encoding.UTF8.GetBytes(mapinfo));

            wad.Save(filePath);
        }
    }
}
