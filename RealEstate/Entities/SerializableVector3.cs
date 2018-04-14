using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Entities
{
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3() { }

        public SerializableVector3(Vector3 p)
        {
            X = p.x;
            Y = p.y;
            Z = p.z;
        }

        public Vector3 GetVector3() => new Vector3(X, Y, Z);
    }
}
