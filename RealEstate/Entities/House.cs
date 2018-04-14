using System;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Entities
{
    public class House
    {
        // Yes, I know. No, I'm not motivated enough to change it.
        public ushort Id { get; set; }
        public SerializableVector3 Position { get; set; }
        public decimal Price { get; set; }

        public ulong? OwnerId { get; set; }
        public DateTime? LastPaid { get; set; }

        public House(ushort id, decimal price, Vector3 pos)
        {
            Id = id;
            Price = price;
            Position = new SerializableVector3(pos);
        }

        public House() { }
    }
}
