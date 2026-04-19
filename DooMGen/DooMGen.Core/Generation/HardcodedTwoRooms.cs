using DooMGen.Core.Map;

namespace DooMGen.Core.Generation
{
    public static class HardcodedTwoRooms
    {
        public static DoomMap Build(string mapName)
        {
            var map = new DoomMap { Name = mapName };

            // --- 1) Secteurs ---
            int sectorRoomA = AddSector(map);
            int sectorCorridor = AddSector(map);
            int sectorRoomB = AddSector(map);

            // --- 2) Vertices (partagés) ---
            // Room A
            int vA0 = AddVertex(map, 0, 0);   // (0,0)
            int vA1 = AddVertex(map, 512, 0);   // (512,0)
            int vA2 = AddVertex(map, 512, 512);   // (512,512)
            int vA3 = AddVertex(map, 0, 512);   // (0,512)

            // Ouverture A → corridor (partagée)
            int vO1 = AddVertex(map, 256, 512);   // (256,512) = bas A + haut corridor
                                                  // vA2 (512,512) sert aussi de coin haut droit corridor

            // Corridor
            int vC2 = AddVertex(map, 512, 640);   // (512,640)
            int vC3 = AddVertex(map, 256, 640);   // (256,640)

            // Room B
            int vB0 = AddVertex(map, 0, 640);   // (0,640)
                                                // vC3 (256,640) = milieu haut B
                                                // vC2 (512,640) = coin haut droit B
            int vB3 = AddVertex(map, 512, 1152);  // (512,1152)
            int vB4 = AddVertex(map, 0, 1152);  // (0,1152)

            // --- 3) Room A : polygone fermé ---
            // Loop: A0 -> A1 -> A2 -> O1 -> A3 -> A0
            AddWall(map, vA0, vA1, sectorRoomA);      // haut
            AddWall(map, vA1, vA2, sectorRoomA);      // droite
                                                      // bas : A2 -> O1 est partagé avec corridor (double‑sided)
                                                      // bas : O1 -> A3 est one‑sided
            AddWall(map, vO1, vA3, sectorRoomA);      // bas gauche
            AddWall(map, vA3, vA0, sectorRoomA);      // gauche

            // --- 4) Corridor : polygone fermé ---
            // Loop: O1 -> A2 -> C2 -> C3 -> O1
            // O1 -> A2 : partagé avec Room A (double‑sided)
            AddWall(map, vA2, vC2, sectorCorridor);   // droite
            AddWall(map, vC2, vC3, sectorCorridor);   // bas
            AddWall(map, vC3, vO1, sectorCorridor);   // gauche

            // --- 5) Room B : polygone fermé ---
            // Loop: B0 -> C3 -> C2 -> B3 -> B4 -> B0
            AddWall(map, vB0, vC3, sectorRoomB);      // haut gauche
                                                      // C3 -> C2 : partagé avec corridor (double‑sided)
            AddWall(map, vC2, vB3, sectorRoomB);      // droite
            AddWall(map, vB3, vB4, sectorRoomB);      // bas
            AddWall(map, vB4, vB0, sectorRoomB);      // gauche

            // --- 6) Ouvertures double‑sided ---

            // Room A <-> Corridor : segment O1 -> A2
            AddTwoSidedWall(map, vO1, vA2, sectorRoomA, sectorCorridor);

            // Corridor <-> Room B : segment C3 -> C2
            AddTwoSidedWall(map, vC3, vC2, sectorCorridor, sectorRoomB);

            // --- 7) Player Start dans Room A ---
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

        // ---------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------

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

        private static void AddTwoSidedWall(DoomMap map, int vStart, int vEnd, int frontSector, int backSector)
        {
            int front = map.Sidedefs.Count;
            map.Sidedefs.Add(new Sidedef
            {
                Id = front,
                SectorId = frontSector,
                MiddleTexture = "-",
                UpperTexture = "-",
                LowerTexture = "-"
            });

            int back = map.Sidedefs.Count;
            map.Sidedefs.Add(new Sidedef
            {
                Id = back,
                SectorId = backSector,
                MiddleTexture = "-",
                UpperTexture = "-",
                LowerTexture = "-"
            });

            map.Linedefs.Add(new Linedef
            {
                Id = map.Linedefs.Count,
                StartVertex = vStart,
                EndVertex = vEnd,
                FrontSidedef = front,
                BackSidedef = back
            });
        }
    }
}
