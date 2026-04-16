using DooMGen.Core.Export;
using DooMGen.Core.Generation;
using DooMGen.Core.Map;
using DooMGen.Core.Validation;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

Dictionary<string, string> appConfig = new Dictionary<string, string>();
foreach (string arg in args)
{
    if (!string.IsNullOrEmpty(arg))
    {
        string theArg = arg.Trim();
        string theKey = theArg.Split('=')[0];
        string theValue = theArg.Contains('=') ? theArg.Split('=')[1] : null;

        appConfig.Add(theKey, theValue);
    }
}

if (appConfig.ContainsKey("-h") || appConfig.ContainsKey("--help"))
{
    PrintUsage();
    return 0;
}

Console.WriteLine($"Starting DooMGen CLI...");

bool bZDoom = appConfig.ContainsKey("zdoom");

Console.WriteLine($"Using {(bZDoom ? "ZDoom" : "Vanilla")} generation mode");

var outputDir = "Output";
Directory.CreateDirectory(outputDir);

DoomMap theMap = new DoomMap();

int seed = (appConfig.ContainsKey("seed") && int.TryParse(appConfig.GetValueOrDefault("seed"), out int parsedSeed)) ? parsedSeed : Environment.TickCount;
Console.WriteLine($"Seed utilisé : {seed}");
int roomCount = (appConfig.ContainsKey("rooms") && int.TryParse(appConfig.GetValueOrDefault("rooms"), out int parsedRoomCount)) ? parsedRoomCount : 0;
Console.WriteLine($"Nombre de rooms : {(roomCount == 0 ? 1 : roomCount)}");

if (roomCount == 0)
{
    theMap = SimpleRoomGenerator.GenerateSimpleRoom();
}
else
{
    var gen = new MultiRoomGenerator(seed);

    // 1. Génération des rooms
    var rooms = gen.GenerateRooms(roomCount);
    Console.WriteLine($"Rooms générées : {rooms.Count}");

    // 2. Génération des corridors logiques
    var corridors = gen.GenerateCorridors(rooms);
    Console.WriteLine($"Corridors générés : {corridors.Count}");

    // 3. Construction de la map UDMF
    theMap = gen.BuildMap(rooms, corridors);
}

// 4. Export WAD
var wadPath = Path.Combine(outputDir, $"{theMap.Name}.wad");
WadMapExporter.ExportToWad(theMap, wadPath, bZDoom);

var result = UdmfValidator.Validate(wadPath, bZDoom);

Console.WriteLine(result.Success ? "Validation OK" : "Validation échouée");

foreach (var e in result.Errors)
    Console.WriteLine("ERREUR : " + e);

foreach (var w in result.Warnings)
    Console.WriteLine("AVERTISSEMENT : " + w);


Console.WriteLine(result.Success ? $"WAD généré : {wadPath}" : "Échec de la génération du WAD");
return 0;

static void PrintUsage()
{
    Console.WriteLine("Usage: DooMGen.CLI [zdoom] [seed=<valeur>] [rooms=<valeur>]");
    Console.WriteLine("  zdoom: Générer un WAD compatible ZDoom");
    Console.WriteLine("  seed: (optionnel) Seed pour la génération (défaut : seed aléatoire)");
    Console.WriteLine("  rooms: (optionnel) Nombre de rooms à générer (défaut : 6)");
}