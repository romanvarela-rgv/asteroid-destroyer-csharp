namespace EngineGDI
{
    // Desplazamiento de la "camara" para el screen shake.
    // Los objetos del mundo se dibujan a traves de Camera.Draw; el fondo y el HUD usan Engine.Draw y no se mueven.
    public static class Camera
    {
        public static Vector2f Offset = new Vector2f(0, 0);

        public static void Draw(string path, Transform transform)
        {
            transform.Position += Offset; // Transform es un struct: se modifica una copia
            Engine.Draw(path, transform);
        }
    }
}
