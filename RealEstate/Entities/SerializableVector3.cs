﻿using UnityEngine;

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

        public override bool Equals(object obj)
        {
            if (obj is SerializableVector3 other && X == other.X && Y == other.Y && Z == other.Z)
                return true;

            if (obj is Vector3 other3 && X == other3.x && Y == other3.y && Z == other3.z)
                return true;

            return false;
        }

        public Vector3 GetVector3() => new Vector3(X, Y, Z);
    }
}
