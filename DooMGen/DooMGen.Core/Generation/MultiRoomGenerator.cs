using DooMGen.Core.Map;

namespace DooMGen.Core.Generation
{
    public class MultiRoomGenerator
    {
        private readonly Random _rng;

        public MultiRoomGenerator(int seed)
        {
            _rng = new Random(seed);
        }

        // ---------------------------------------------------------
        // ROOM GENERATION
        // ---------------------------------------------------------

        public List<Room> GenerateRooms(int count)
        {
            var rooms = new List<Room>();
            int attempts = 0;

            while (rooms.Count < count && attempts < count * 20)
            {
                attempts++;

                var room = new Room
                {
                    Id = rooms.Count,
                    Width = _rng.Next(128, 512),
                    Height = _rng.Next(128, 512),
                    X = _rng.Next(0, 2048),
                    Y = _rng.Next(0, 2048)
                };

                if (!rooms.Any(r => Overlaps(r, room)))
                    rooms.Add(room);
            }

            return rooms;
        }

        private bool Overlaps(Room a, Room b)
        {
            return !(b.X + b.Width <= a.X ||
                     b.X >= a.X + a.Width ||
                     b.Y + b.Height <= a.Y ||
                     b.Y >= a.Y + a.Height);
        }

        // ---------------------------------------------------------
        // CORRIDOR PAIRING (LOGIQUE)
        // ---------------------------------------------------------

        public List<(Room A, Room B)> GenerateCorridors(List<Room> rooms)
        {
            var corridors = new List<(Room, Room)>();
            var sorted = rooms.OrderBy(r => r.CenterX).ToList();

            for (int i = 0; i < sorted.Count - 1; i++)
                corridors.Add((sorted[i], sorted[i + 1]));

            return corridors;
        }

        // ---------------------------------------------------------
        // MAP BUILDING
        // ---------------------------------------------------------

        public DoomMap BuildMap(List<Room> rooms, List<(Room A, Room B)> corridors, string mapName)
        {
            var map = new DoomMap { Name = mapName };

            // 1. Un seul secteur pour toute la map
            var sector = new Sector
            {
                Id = 0,
                FloorHeight = 0,
                CeilingHeight = 128,
                FloorTexture = "FLOOR0_1",
                CeilingTexture = "CEIL1_1",
                LightLevel = 160
            };
            map.Sectors.Add(sector);

            // 2. Build rooms (toutes dans le même secteur)
            foreach (var room in rooms)
            {
                room.SectorId = sector.Id;
                BuildRoom(map, room, sector.Id);
            }

            // 3. Build corridors (dans le même secteur)
            foreach (var (A, B) in corridors)
                ConnectRooms(map, A, B, sector.Id);

            // 4. Player start dans la première room (safe)
            var first = rooms.First();
            var (sx, sy) = SafeSpawn(first);

            map.Things.Add(new Thing
            {
                Id = 0,
                X = sx,
                Y = sy,
                Type = 1,
                Angle = 0,
                Flags = 7
            });

            return map;
        }

        private (double X, double Y) SafeSpawn(Room room)
        {
            const int margin = 64;
            double x = room.X + margin;
            double y = room.Y + margin;
            return (x, y);
        }

        private void BuildRoom(DoomMap map, Room room, int sectorId)
        {
            int v0 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X, room.Y));

            int v1 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X + room.Width, room.Y));

            int v2 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X + room.Width, room.Y + room.Height));

            int v3 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X, room.Y + room.Height));

            AddLine(map, v0, v3, sectorId);
            AddLine(map, v3, v2, sectorId);
            AddLine(map, v2, v1, sectorId);
            AddLine(map, v1, v0, sectorId);
        }

        // ---------------------------------------------------------
        // CORRIDOR GEOMETRY (L-SHAPE, MÊME SECTEUR)
        // ---------------------------------------------------------

        private void ConnectRooms(DoomMap map, Room a, Room b, int sectorId)
        {
            int ax = (int)a.CenterX;
            int ay = (int)a.CenterY;

            int bx = (int)b.CenterX;
            int by = (int)b.CenterY;

            // 1) Segment horizontal
            int hX = Math.Min(ax, bx);
            int hW = Math.Abs(bx - ax);
            BuildCorridor(map, hX, ay - 32, hW, 64, sectorId);

            // 2) Segment vertical
            int vY = Math.Min(ay, by);
            int vH = Math.Abs(by - ay);
            BuildCorridor(map, bx - 32, vY, 64, vH, sectorId);
        }

        private void BuildCorridor(DoomMap map, int x, int y, int width, int height, int sectorId)
        {
            int v0 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x, y));

            int v1 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x + width, y));

            int v2 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x + width, y + height));

            int v3 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x, y + height));

            AddLine(map, v0, v3, sectorId);
            AddLine(map, v3, v2, sectorId);
            AddLine(map, v2, v1, sectorId);
            AddLine(map, v1, v0, sectorId);
        }

        // ---------------------------------------------------------
        // LINE BUILDER
        // ---------------------------------------------------------

        private void AddLine(DoomMap map, int vStart, int vEnd, int sectorId)
        {
            int sidedefIndex = map.Sidedefs.Count;

            map.Sidedefs.Add(new Sidedef
            {
                Id = sidedefIndex,
                SectorId = sectorId,
                UpperTexture = "-",
                LowerTexture = "-",
                MiddleTexture = "STARTAN2"
            });

            map.Linedefs.Add(new Linedef
            {
                Id = map.Linedefs.Count,
                StartVertex = vStart,
                EndVertex = vEnd,
                FrontSidedef = sidedefIndex,
                Flags = 0,
                Special = 0,
                Tag = 0
            });
        }
    }
}