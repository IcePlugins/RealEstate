using System;

namespace ExtraConcentratedJuice.RealEstate.Entities
{
    public class House
    {
        public ushort Id { get; set; }
        public decimal Price { get; set; }

        public ulong? OwnerId { get; set; }
        public DateTime? LastPaid { get; set; }

        public House(ushort id, decimal price)
        {
            Id = id;
            Price = price;
        }

        public House() { }
    }
}
