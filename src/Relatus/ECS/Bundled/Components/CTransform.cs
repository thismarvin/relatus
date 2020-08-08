using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
{
    public class CTransform : IComponent
    {
        public Vector3 Translation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Rotation { get; set; }

        public CTransform()
        {
            Rotation = Vector3.Zero;
            Origin = Vector3.Zero;
            Translation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public CTransform(Vector3 translation, Vector3 scale, Vector3 origin, Vector3 rotation)
        {
            Translation = translation;
            Scale = scale;
            Origin = origin;
            Rotation = rotation;
        }
    }
}
