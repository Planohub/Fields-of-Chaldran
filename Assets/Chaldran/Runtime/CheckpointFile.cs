using System;
using System.IO;

namespace Chaldran
{
    public static class CheckpointFile
    {
        public static bool TryWrite(string path, string contents)
        {
            string temporary = path + ".tmp";
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(temporary, contents);
                if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
                else File.Move(temporary, path);
                return true;
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                return false; // Keep the previous checkpoint if writing failed.
            }
            finally
            {
                try { if (File.Exists(temporary)) File.Delete(temporary); }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException) { }
            }
        }

        public static string TryRead(string path)
        {
            try
            {
                if (!File.Exists(path) || new FileInfo(path).Length > 65536) return null;
                return File.ReadAllText(path);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException) { return null; }
        }
    }
}
