using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EngineGDI
{
    public class PoolObject<T> where T : class, new()
    {
        //// La restricción 'where T : class, new()' obliga a que sea una clase y tenga constructor vacío
        private List<T> availableObjects = new List<T>();
        private List<T> activeObjects = new List<T>();

        public List<T> ActiveObjects => activeObjects;
        public List<T> AvailableObjects => availableObjects;

        public PoolObject(int initiaSize = 100)
        {
            for (int i = 0; i < initiaSize; i++)
            {
                availableObjects.Add(new T()); //Instancia el tipo generico
            }
        }

        public T GetObject()
        {
            T obj;

            if (availableObjects.Count > 0)
            {
                obj = availableObjects[0];
                availableObjects.RemoveAt(0);
            }
            else
            {
                obj = new T();
            }
            activeObjects.Add(obj);
            return obj;
        }
        public void RealeaseObject(T obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
            }
            if (!availableObjects.Contains(obj))
            {
                availableObjects.Add(obj);
            }
          
        }
        

    }
    
}