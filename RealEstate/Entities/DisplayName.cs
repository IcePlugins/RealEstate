namespace ExtraConcentratedJuice.RealEstate.Entities
{
    public class DisplayName
    {
        // Can't serialize a dictionary to XML for some reason, better go with this I guess.
        public ulong Id { get; set; }
        public string Name { get; set; }

        public DisplayName(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public DisplayName() { }
    }
}
