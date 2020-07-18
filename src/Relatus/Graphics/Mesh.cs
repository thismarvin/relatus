using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class Mesh
    {
        public Vector3[] Vertices { get; private set; }
        public short[] Indices { get; private set; }

        public Mesh(Vector3[] vertices, short[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
