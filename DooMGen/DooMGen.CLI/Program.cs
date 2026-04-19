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
DateTime generationStart = DateTime.Now;

bool bZDoom = appConfig.ContainsKey("zdoom");

var outputDir = "Output";
if (appConfig.ContainsKey("output"))
    outputDir = appConfig["output"];


var mapName = "generated_map";
if (appConfig.ContainsKey("wadname"))
    mapName = appConfig["wadname"];

Console.WriteLine($"Using {(bZDoom ? "ZDoom" : "Vanilla")} generation mode");

Directory.CreateDirectory(outputDir);

DoomMap theMap = new DoomMap();

Console.WriteLine($"Starting generation for map '{mapName}' ...");

int seed = (appConfig.ContainsKey("seed") && int.TryParse(appConfig.GetValueOrDefault("seed"), out int parsedSeed)) ? parsedSeed : Environment.TickCount;
Console.WriteLine($"Seed used : {seed}");
int roomCount = (appConfig.ContainsKey("rooms") && int.TryParse(appConfig.GetValueOrDefault("rooms"), out int parsedRoomCount)) ? parsedRoomCount : 0;
Console.WriteLine($"Number of rooms : {(roomCount == 0 ? 1 : roomCount)}");

if (roomCount == 0)
{
    Console.WriteLine($"Using HardcodedTwoRooms generator");
    //theMap = SimpleRoomGenerator.GenerateSimpleRoom(mapName);
    //theMap = HardcodedTwoRooms.Build(mapName);
    theMap = OneSectorRooms.Build(mapName);
}
else
{
    Console.WriteLine($"Using MultiRoomGenerator generator");
    var gen = new MultiRoomGenerator(seed);

    // 1. Génération des rooms
    var rooms = gen.GenerateRooms(roomCount);
    Console.WriteLine($"Rooms generated : {rooms.Count}");

    // 2. Génération des corridors logiques
    var corridors = gen.GenerateCorridors(rooms);
    Console.WriteLine($"Corridors generated : {corridors.Count}");

    // 3. Construction de la map UDMF
    theMap = gen.BuildMap(rooms, corridors, mapName);
}

// 4. Export WAD
var wadPath = Path.Combine(outputDir, $"{theMap.Name}.wad");
WadMapExporter.ExportToWad(theMap, wadPath, bZDoom, mapName);

var result = UdmfValidator.Validate(wadPath, bZDoom);

Console.WriteLine(result.Success ? "Validation OK" : "Validation failed");

foreach (var e in result.Errors)
    Console.WriteLine("ERROR : " + e);

foreach (var w in result.Warnings)
    Console.WriteLine("WARNING : " + w);

DateTime generationEnd = DateTime.Now;
TimeSpan generationDuration = generationEnd - generationStart;

Console.WriteLine(result.Success ? $"WAD generation ended ({generationDuration.TotalSeconds:F2} s)  : {wadPath}" : "Échec de la génération du WAD");
return 0;

static void PrintUsage()
{
    Console.WriteLine("Usage: DooMGen.CLI [zdoom] [seed=<valeur>] [rooms=<valeur>]");
    Console.WriteLine("  zdoom: Générer un WAD compatible ZDoom");
    Console.WriteLine("  seed: (optionnel) Seed pour la génération (défaut : seed aléatoire)");
    Console.WriteLine("  rooms: (optionnel) Nombre de rooms à générer (défaut : 6)");
    Console.WriteLine("  output: (optionnel) Répertoire de sortie pour le WAD généré (défaut : Output)");
}