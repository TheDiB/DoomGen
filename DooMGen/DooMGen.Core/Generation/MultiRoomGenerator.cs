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
        // CORRIDOR PAIRING
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

        public DoomMap BuildMap(List<Room> rooms, List<(Room A, Room B)> corridors)
        {
            var map = new DoomMap { Name = "MAP01" };

            // 1. Build rooms
            foreach (var room in rooms)
                BuildRoom(map, room);

            // 2. Build corridors
            foreach (var (A, B) in corridors)
                ConnectRooms(map, A, B);

            // 3. Player start
            var first = rooms.First();
            map.Things.Add(new Thing
            {
                Id = 0,
                X = first.CenterX,
                Y = first.CenterY,
                Type = 1,
                Angle = 0,
                Flags = 7
            });

            return map;
        }

        private void BuildRoom(DoomMap map, Room room)
        {
            // 1. Sector
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

            // 2. Vertices
            int v0 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X, room.Y));

            int v1 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X + room.Width, room.Y));

            int v2 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X + room.Width, room.Y + room.Height));

            int v3 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(room.X, room.Y + room.Height));

            // 3. Linedefs (correct orientation)
            AddLine(map, v0, v3, sector.Id);
            AddLine(map, v3, v2, sector.Id);
            AddLine(map, v2, v1, sector.Id);
            AddLine(map, v1, v0, sector.Id);
        }

        // ---------------------------------------------------------
        // CORRIDOR GEOMETRY
        // ---------------------------------------------------------

        private void ConnectRooms(DoomMap map, Room a, Room b)
        {
            int x1 = (int)a.CenterX;
            int y1 = (int)a.CenterY;

            int x2 = (int)b.CenterX;
            int y2 = (int)b.CenterY;

            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
            {
                // Horizontal corridor
                int left = Math.Min(x1, x2);
                int right = Math.Max(x1, x2);

                BuildCorridor(map, left, y1 - 32, right - left, 64);
            }
            else
            {
                // Vertical corridor
                int top = Math.Min(y1, y2);
                int bottom = Math.Max(y1, y2);

                BuildCorridor(map, x1 - 32, top, 64, bottom - top);
            }
        }

        private void BuildCorridor(DoomMap map, int x, int y, int width, int height)
        {
            // 1. Sector
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

            // 2. Vertices
            int v0 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x, y));

            int v1 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x + width, y));

            int v2 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x + width, y + height));

            int v3 = map.Vertices.Count;
            map.Vertices.Add(new Vertex(x, y + height));

            // 3. Linedefs
            AddLine(map, v0, v3, sector.Id);
            AddLine(map, v3, v2, sector.Id);
            AddLine(map, v2, v1, sector.Id);
            AddLine(map, v1, v0, sector.Id);
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