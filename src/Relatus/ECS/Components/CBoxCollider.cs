using Microsoft.Xna.Framework;
using Relatus.Core;
using Relatus.Graphics;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    struct CBoxCollider : IComponent
    {
 
    }

    static class CBoxColliderHelper
    {
        public static ShapeData ShapeData { get; private set; }

        static CBoxColliderHelper()
        {
            ShapeData = GeometryManager.GetShapeData(ShapeType.Square);
        }       
    }
}
