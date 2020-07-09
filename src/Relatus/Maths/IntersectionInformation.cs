using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{
    public struct IntersectionInformation
    {
        public bool Valid { get; private set; }
        public float T { get; private set; }
        public float U { get; private set; }
        public Vector2 Convergence { get; private set; }
        public bool Intersected { get; private set; }

        public IntersectionInformation(float t, float u, Vector2 convergence)
        {
            Valid = true;
            T = t;
            U = u;
            Convergence = convergence;
            Intersected = 0 <= T && T <= 1 && 0 <= U && U <= 1;
        }
    }
}
