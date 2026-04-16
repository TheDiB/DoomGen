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

        public DoomMap BuildMap(List<Room> rooms, List<(Room A, Room B)> corridors)
        {
            var map = new DoomMap { Name = "MAP01" };

            int vertexIndex = 0;
            int sidedefIndex = 0;
            int linedefIndex = 0;
            int sectorIndex = 0;

            foreach (var room in rooms)
            {
                // 1. Créer un secteur
                var sector = new Sector { Id = sectorIndex++ };
                map.Sectors.Add(sector);

                // 2. Créer les 4 vertices
                int v0 = vertexIndex++;
                int v1 = vertexIndex++;
                int v2 = vertexIndex++;
                int v3 = vertexIndex++;

                map.Vertices.Add(new Vertex(room.X, room.Y));
                map.Vertices.Add(new Vertex(room.X + room.Width, room.Y));
                map.Vertices.Add(new Vertex(room.X + room.Width, room.Y + room.Height));
                map.Vertices.Add(new Vertex(room.X, room.Y + room.Height));

                // 3. Créer les 4 sidedefs
                int s0 = sidedefIndex++;
                int s1 = sidedefIndex++;
                int s2 = sidedefIndex++;
                int s3 = sidedefIndex++;

                map.Sidedefs.Add(new Sidedef { Id = s0, SectorId = sector.Id });
                map.Sidedefs.Add(new Sidedef { Id = s1, SectorId = sector.Id });
                map.Sidedefs.Add(new Sidedef { Id = s2, SectorId = sector.Id });
                map.Sidedefs.Add(new Sidedef { Id = s3, SectorId = sector.Id });

                // 4. Créer les 4 linedefs
                map.Linedefs.Add(new Linedef { Id = linedefIndex++, StartVertex = v0, EndVertex = v1, FrontSidedef = s0 });
                map.Linedefs.Add(new Linedef { Id = linedefIndex++, StartVertex = v1, EndVertex = v2, FrontSidedef = s1 });
                map.Linedefs.Add(new Linedef { Id = linedefIndex++, StartVertex = v2, EndVertex = v3, FrontSidedef = s2 });
                map.Linedefs.Add(new Linedef { Id = linedefIndex++, StartVertex = v3, EndVertex = v0, FrontSidedef = s3 });
            }

            // 5. Ajouter le joueur dans la première room
            var first = rooms.First();
            map.Things.Add(new Thing
            {
                Id = 0,
                X = first.CenterX,
                Y = first.CenterY,
                Type = 1
            });

            return map;
        }

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

        public List<(Room A, Room B)> GenerateCorridors(List<Room> rooms)
        {
            var corridors = new List<(Room, Room)>();

            var sorted = rooms.OrderBy(r => r.CenterX).ToList();

            for (int i = 0; i < sorted.Count - 1; i++)
                corridors.Add((sorted[i], sorted[i + 1]));

            return corridors;
        }

        private bool Overlaps(Room a, Room b)
        {
            return !(b.X + b.Width < a.X ||
                     b.X > a.X + a.Width ||
                     b.Y + b.Height < a.Y ||
                     b.Y > a.Y + a.Height);
        }
    }
}
