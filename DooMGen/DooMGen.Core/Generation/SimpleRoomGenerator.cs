using DooMGen.Core.Map;

namespace DooMGen.Core.Generation
{
    public static class SimpleRoomGenerator
    {
        public static DoomMap GenerateSimpleRoom()
        {
            var map = new DoomMap { Name = "MAP01" };

            // 1. Secteur
            var sector = new Sector
            {
                Id = map.Sectors.Count,
                FloorHeight = 0,
                CeilingHeight = 128,
                FloorTexture = "FLOOR0_1",
                CeilingTexture = "CEIL1_1",
                LightLevel = 160
            };
            map.Sectors.Add(sector);

            // 2. Vertices (ordre : bas-gauche, bas-droite, haut-droite, haut-gauche)
            int v0 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(0, 0));

            int v1 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(256, 0));

            int v2 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(256, 256));

            int v3 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(0, 256));

            // 3. Lignes (boucle fermée, dans l’ordre)
            AddLine(map, v0, v3, sector.Id);
            AddLine(map, v3, v2, sector.Id);
            AddLine(map, v2, v1, sector.Id);
            AddLine(map, v1, v0, sector.Id);

            // 4. Player 1 Start au centre du secteur
            map.Things.Add(new Thing
            {
                Id = 0,
                X = 128,
                Y = 128,
                Type = 1,
                Angle = 0,
                Flags = 7
            });

            return map;
        }

        private static void AddLine(DoomMap map, int vStart, int vEnd, int sectorId)
        {
            // 1. Créer le sidedef
            var sidedef = new Sidedef
            {
                Id = map.Sidedefs.Count,
                SectorId = sectorId,
                UpperTexture = "-",
                LowerTexture = "-",
                MiddleTexture = "STARTAN2"
            };
            map.Sidedefs.Add(sidedef);

            // 2. Créer le linedef qui pointe vers CE sidedef
            map.Linedefs.Add(new Linedef
            {
                Id = map.Linedefs.Count,
                StartVertex = vStart,
                EndVertex = vEnd,
                FrontSidedef = sidedef.Id,
                Flags = 0,
                Special = 0,
                Tag = 0
            });
        }
    }
}
