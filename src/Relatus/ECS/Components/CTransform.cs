using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class CTransform : IComponent
    {
        public Vector3 Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 RotationOffset { get; set; }
        public Vector3 Translation { get; set; }

        public CTransform()
        {
            Rotation = 0;
            RotationOffset = Vector2.Zero;
            Translation = Vector3.Zero;
            Scale = new Vector3(1);
        }

        public CTransform(Vector3 scale, float rotation, Vector2 rotationOffset, Vector3 translation)
        {
            Scale = scale;
            Rotation = rotation;
            RotationOffset = rotationOffset;
            Translation = translation;
        }
    }
}
