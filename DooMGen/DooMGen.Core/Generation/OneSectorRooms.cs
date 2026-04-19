using DooMGen.Core.Map;

namespace DooMGen.Core.Generation
{
    public static class OneSectorRooms
    {
        public static DoomMap Build(string mapName)
        {
            var map = new DoomMap { Name = mapName };

            // --- 1) Un seul secteur ---
            int sectorId = AddSector(map);

            // --- 2) Vertices du contour global ---
            int v0 = AddVertex(map, 0, 0);
            int v1 = AddVertex(map, 512, 0);
            int v2 = AddVertex(map, 768, 512);
            int v3 = AddVertex(map, 768, 1152);
            int v4 = AddVertex(map, 0, 1152);
            int v5 = AddVertex(map, 0, 512);

            // --- 3) Polygone fermé (dans l'ordre) ---
            AddWall(map, v0, v1, sectorId);
            AddWall(map, v1, v2, sectorId);
            AddWall(map, v2, v3, sectorId);
            AddWall(map, v3, v4, sectorId);
            AddWall(map, v4, v5, sectorId);
            AddWall(map, v5, v0, sectorId);

            // --- 4) Player Start ---
            map.Things.Add(new Thing
            {
                Id = 0,
                X = 256,
                Y = 256,
                Type = 1,
                Angle = 0,
                Flags = 7
            });

            return map;
        }

        private static int AddSector(DoomMap map)
        {
            int id = map.Sectors.Count;
            map.Sectors.Add(new Sector
            {
                Id = id,
                FloorHeight = 0,
                CeilingHeight = 128,
                FloorTexture = "FLOOR0_1",
                CeilingTexture = "CEIL1_1",
                LightLevel = 160
            });
            return id;
        }

        private static int AddVertex(DoomMap map, double x, double y)
        {
            int id = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x, y));
            return id;
        }

        private static void AddWall(DoomMap map, int vStart, int vEnd, int sectorId)
        {
            int sidedef = map.Sidedefs.Count;
            map.Sidedefs.Add(new Sidedef
            {
                Id = sidedef,
                SectorId = sectorId,
                MiddleTexture = "STARTAN2",
                UpperTexture = "-",
                LowerTexture = "-"
            });

            map.Linedefs.Add(new Linedef
            {
                Id = map.Linedefs.Count,
                StartVertex = vStart,
                EndVertex = vEnd,
                FrontSidedef = sidedef
            });
        }
    }
}