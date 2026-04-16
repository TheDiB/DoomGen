using System.Text;

namespace DooMGen.Core.WAD
{
    public class WadReader
    {
        public record LumpInfo(string Name, int Offset, int Size);

        public static List<LumpInfo> ReadDirectory(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            // Header
            var magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if (magic != "PWAD" && magic != "IWAD")
                throw new Exception("Fichier WAD invalide : header incorrect.");

            int lumpCount = br.ReadInt32();
            int dirOffset = br.ReadInt32();

            fs.Seek(dirOffset, SeekOrigin.Begin);

            var lumps = new List<LumpInfo>();

            for (int i = 0; i < lumpCount; i++)
            {
                int offset = br.ReadInt32();
                int size = br.ReadInt32();
                string name = Encoding.ASCII.GetString(br.ReadBytes(8)).TrimEnd('\0');

                lumps.Add(new LumpInfo(name, offset, size));
            }

            return lumps;
        }

        public static string ReadLumpText(string filePath, LumpInfo lump)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            fs.Seek(lump.Offset, SeekOrigin.Begin);
            var data = br.ReadBytes(lump.Size);

            return Encoding.UTF8.GetString(data);
        }
    }
}
