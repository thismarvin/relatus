using Microsoft.Xna.Framework;
using Mimoso.Core;
using Mimoso.Graphics;
using Mimoso.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.ECS
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
