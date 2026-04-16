using DooMGen.Core.Export;
using DooMGen.Core.Generation;
using DooMGen.Core.Validation;

Console.WriteLine($"Starting DooMGen CLI...");

bool bZDoom = args.Contains("-zdoom");

Console.WriteLine($"Using {(bZDoom ? "ZDoom" : "Vanilla")} generation mode");

var outputDir = "Output";
Directory.CreateDirectory(outputDir);

var map = SimpleRoomGenerator.GenerateSimpleRoom();

var wadPath = Path.Combine(outputDir, $"{map.Name}.wad");
WadMapExporter.ExportToWad(map, wadPath, bZDoom);

var result = UdmfValidator.Validate(wadPath, bZDoom);

Console.WriteLine(result.Success ? "Validation OK" : "Validation échouée");

foreach (var e in result.Errors)
    Console.WriteLine("ERREUR : " + e);

foreach (var w in result.Warnings)
    Console.WriteLine("AVERTISSEMENT : " + w);


Console.WriteLine(result.Success ? $"WAD généré : {wadPath}" : "Échec de la génération du WAD");