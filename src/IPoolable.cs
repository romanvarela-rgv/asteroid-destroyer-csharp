using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EngineGDI
{
    public interface IPoolable
    {
        bool Active { get; }
        void ResetObject(); //Reinicia el estado del objeto 
    }
}