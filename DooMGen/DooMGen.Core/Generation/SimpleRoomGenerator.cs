using DooMGen.Core.Map;

namespace DooMGen.Core.Generation
{
    public static class SimpleRoomGenerator
    {
        public static DoomMap GenerateSimpleRoom()
        {
            var map = new DoomMap { Name = "MAP01" };

            // 4 sommets d’un rectangle
            map.Vertices.AddRange(new[]
            {
            new Vertex(0, 0),
            new Vertex(256, 0),
            new Vertex(256, 256),
            new Vertex(0, 256)
        });

            // 1 secteur
            var sector = new Sector { Id = 0 };
            map.Sectors.Add(sector);

            // 4 sidedefs (un par mur)
            for (int i = 0; i < 4; i++)
            {
                map.Sidedefs.Add(new Sidedef
                {
                    Id = i,
                    SectorId = sector.Id
                });
            }

            // 4 linedefs (boucle)
            map.Linedefs.AddRange(new[]
            {
            new Linedef { Id = 0, StartVertex = 0, EndVertex = 1, FrontSidedef = 0 },
            new Linedef { Id = 1, StartVertex = 1, EndVertex = 2, FrontSidedef = 1 },
            new Linedef { Id = 2, StartVertex = 2, EndVertex = 3, FrontSidedef = 2 },
            new Linedef { Id = 3, StartVertex = 3, EndVertex = 0, FrontSidedef = 3 },
        });

            // Player 1 au centre
            map.Things.Add(new Thing
            {
                Id = 0,
                X = 128,
                Y = 128,
                Angle = 0,
                Type = 1
            });

            return map;
        }
    }
}
