using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public struct CBoxCollider : IComponent
    {
 
    }

    internal static class CBoxColliderHelper
    {
        public static GeometryData ShapeData { get; private set; }

        static CBoxColliderHelper()
        {
            ShapeData = GeometryManager.GetShapeData(ShapeType.Square);
        }       
    }
}
