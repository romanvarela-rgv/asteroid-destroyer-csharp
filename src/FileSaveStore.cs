using System;
using System.Collections.Generic;
using System.IO;

namespace EngineGDI
{
    // Guarda los datos en un archivo de texto con lineas "clave=valor"
    // en %AppData%\AsteroidDestroyer\save.txt
    public class FileSaveStore : ISaveStore
    {
        private readonly string path;

        public FileSaveStore()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AsteroidDestroyer", "save.txt"))
        {
        }

        public FileSaveStore(string path)
        {
            this.path = path;
        }

        public SaveData Load()
        {
            var data = new SaveData();
            if (!File.Exists(path)) return data;

            try
            {
                var values = new Dictionary<string, string>();
                foreach (string line in File.ReadAllLines(path))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0)
                        values[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
                }

                data.HighScore    = ReadInt(values, "highscore", 0);
                data.SoundEnabled = ReadInt(values, "sound", 1) != 0;
                data.Wave         = ReadInt(values, "wave", 0);
                data.Score        = ReadInt(values, "score", 0);
                data.Lives        = ReadInt(values, "lives", 0);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Si no se puede leer, se arranca de cero en vez de cerrar el juego
                Console.WriteLine("No se pudo leer el guardado: " + ex.Message);
                return new SaveData();
            }

            return data;
        }

        public void Save(SaveData data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllLines(path, new[]
                {
                    "highscore=" + data.HighScore,
                    "sound=" + (data.SoundEnabled ? 1 : 0),
                    "wave=" + data.Wave,
                    "score=" + data.Score,
                    "lives=" + data.Lives
                });
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine("No se pudo guardar: " + ex.Message);
            }
        }

        // Un valor corrupto o que falta no rompe la carga: se usa el valor por defecto
        private static int ReadInt(Dictionary<string, string> values, string key, int fallback)
        {
            string raw;
            int result;
            if (values.TryGetValue(key, out raw) && int.TryParse(raw, out result))
                return result;
            return fallback;
        }
    }
}
