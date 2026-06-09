using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineGDI;
using System.Reflection;

namespace Asteroid_Destroyer
{
    [TestClass]
    public class UnitTest1
    {
        // Verifica que la suma de dos vectores devuelve el resultado correcto en X e Y.
        
        [TestMethod]
        public void Vector2f_Suma_DevuelveResultadoCorrecto()
        {
            var a = new Vector2f(1, 2);
            var b = new Vector2f(3, 4);
            var resultado = a + b;
            Assert.AreEqual(4f, resultado.X, 0.0001f);
            Assert.AreEqual(6f, resultado.Y, 0.0001f);
        }

        // Verifica que dos cajas que se solapan son detectadas como colisión AABB
        
        [TestMethod]
        public void Collision_CheckAABB_DetectaSolapamientoYSeparacion()
        {
            var t1 = new Transform(0, 0);
            t1.Size = new Vector2f(100, 100);

            var tSolapa = new Transform(50, 50);
            tSolapa.Size = new Vector2f(100, 100);

            var tSeparado = new Transform(200, 0);
            tSeparado.Size = new Vector2f(100, 100);

            Assert.IsTrue(Collision.CheckAABB(t1, tSolapa));
            Assert.IsFalse(Collision.CheckAABB(t1, tSeparado));
        }

        // Verifica el ciclo completo del Object Pool
        
        [TestMethod]
        public void ObjectPool_GetYRelease_GestionaListasCorrectamente()
        {
            var pool = new PoolObject<TestPoolable>(3);

            Assert.AreEqual(3, pool.AvailableObjects.Count);
            Assert.AreEqual(0, pool.ActiveObjects.Count);

            var obj = pool.GetObject();

            Assert.AreEqual(2, pool.AvailableObjects.Count);
            Assert.AreEqual(1, pool.ActiveObjects.Count);

            pool.RealeaseObject(obj);

            Assert.AreEqual(3, pool.AvailableObjects.Count);
            Assert.AreEqual(0, pool.ActiveObjects.Count);
        }
    }

    // Clase mínima para satisfacer la restricción genérica de PoolObject<T>
    public class TestPoolable : IPoolable
    {
        public bool Active { get; private set; } = true;
        public void ResetObject() { }
    }
}
