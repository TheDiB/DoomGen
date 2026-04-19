using DooMGen.Core.Map;
using DooMGen.Core.WAD;
using System.Text;

namespace DooMGen.Core.Export
{
    public static class WadMapExporter
    {
        public static void ExportToWad(DoomMap map, string filePath, bool ZDoomMode, string mapName)
        {
            var wad = new WadWriter();

            // MAP01 (vide)
            wad.AddLump("MAP01", Array.Empty<byte>());

            // TEXTMAP
            var udmf = UdmfExporter.Export(map, ZDoomMode);
            var utf8NoBom = new UTF8Encoding(false);
            wad.AddLump("TEXTMAP", utf8NoBom.GetBytes(udmf));

            // ENDMAP
            wad.AddLump("ENDMAP", Array.Empty<byte>());

            // MAPINFO
            var mapinfo = MapInfoExporter.Export(map, ZDoomMode, mapName);
            wad.AddLump(ZDoomMode ? "ZMAPINFO" : "MAPINFO", Encoding.UTF8.GetBytes(mapinfo));

            wad.Save(filePath);
        }
    }
}
