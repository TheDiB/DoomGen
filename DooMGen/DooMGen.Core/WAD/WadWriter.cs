using System.Text;

namespace DooMGen.Core.WAD
{
    public class WadWriter
    {
        private readonly List<(string Name, byte[] Data)> _lumps = new();

        public void AddLump(string name, byte[] data)
        {
            if (name.Length > 8)
                throw new ArgumentException("Le nom d'un lump ne peut pas dépasser 8 caractères.");

            _lumps.Add((name, data));
        }

        public void Save(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs);

            // Header placeholder
            bw.Write(Encoding.ASCII.GetBytes("PWAD")); // 4 bytes
            bw.Write(_lumps.Count);                    // 4 bytes
            bw.Write(0);                               // 4 bytes (offset directory, à remplir plus tard)

            // Écriture des lumps
            var lumpOffsets = new List<int>();
            var lumpSizes = new List<int>();

            foreach (var lump in _lumps)
            {
                lumpOffsets.Add((int)fs.Position);
                lumpSizes.Add(lump.Data.Length);

                bw.Write(lump.Data);
            }

            // Offset du directory
            int directoryOffset = (int)fs.Position;

            // Directory
            for (int i = 0; i < _lumps.Count; i++)
            {
                bw.Write(lumpOffsets[i]); // offset
                bw.Write(lumpSizes[i]);   // size

                var nameBytes = Encoding.ASCII.GetBytes(_lumps[i].Name);
                var padded = new byte[8];
                Array.Copy(nameBytes, padded, nameBytes.Length);

                bw.Write(padded); // nom du lump (8 bytes)
            }

            // Mise à jour du header
            fs.Seek(8, SeekOrigin.Begin);
            bw.Write(directoryOffset);
        }
    }
}
