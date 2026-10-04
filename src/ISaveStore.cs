namespace EngineGDI
{
    // Donde se guardan los datos. GameManager solo conoce esta interfaz,
    // asi se puede cambiar el archivo por otra cosa (o por un fake en pruebas) sin tocarlo.
    public interface ISaveStore
    {
        SaveData Load();
        void Save(SaveData data);
    }
}
