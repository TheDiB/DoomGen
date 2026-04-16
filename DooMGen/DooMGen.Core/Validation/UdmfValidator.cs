using DooMGen.Core.WAD;

namespace DooMGen.Core.Validation
{
    public class UdmfValidator
    {
        public record ValidationResult(bool Success, List<string> Errors, List<string> Warnings);

        public static ValidationResult Validate(string wadPath, bool ZDoomMode)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            var lumps = WadReader.ReadDirectory(wadPath);

            // 1. Trouver MAPxx
            var mapLump = lumps.FirstOrDefault(l => l.Name.StartsWith("MAP"));
            if (mapLump == null)
                errors.Add("Aucun lump MAPxx trouvé.");

            // 2. TEXTMAP
            var textmap = lumps.FirstOrDefault(l => l.Name == "TEXTMAP");
            if (textmap == null)
                errors.Add("Lump TEXTMAP manquant.");

            // 3. ENDMAP
            var endmap = lumps.FirstOrDefault(l => l.Name == "ENDMAP");
            if (endmap == null)
                errors.Add("Lump ENDMAP manquant.");

            // 4. Ordre TEXTMAP → ENDMAP
            if (textmap != null && endmap != null)
            {
                int indexTextmap = lumps.IndexOf(textmap);
                int indexEndmap = lumps.IndexOf(endmap);

                if (indexEndmap != indexTextmap + 1)
                    errors.Add("ENDMAP doit suivre immédiatement TEXTMAP.");
            }

            // 5. Namespace UDMF
            if (textmap != null)
            {
                var text = WadReader.ReadLumpText(wadPath, textmap);

                if (!text.Contains("namespace", StringComparison.OrdinalIgnoreCase))
                    errors.Add("Le lump TEXTMAP ne contient pas de namespace.");

                if (!text.Contains("namespace = \"Doom\"") &&
                    !text.Contains("namespace = \"ZDoom\"") && ZDoomMode)
                {
                    warnings.Add("Namespace UDMF inhabituel. Recommandé : \"Doom\" ou \"ZDoom\".");
                }

                if (text.Contains("namespace = \"ZDoom\"") && !ZDoomMode)
                {
                    warnings.Add("Namespace ZDoom inattendu. Recommandé : \"Doom\" ou \"ZDBSP\".");
                }
            }

            // 6. ZMAPINFO
            var zmapinfo = lumps.FirstOrDefault(l => l.Name == "ZMAPINFO");
            if (zmapinfo == null && ZDoomMode)
                warnings.Add("ZMAPINFO manquant (la map utilisera les paramètres par défaut).");

            return new ValidationResult(errors.Count == 0, errors, warnings);
        }
    }
}
