using ExtraConcentratedJuice.RealEstate.Entities;
using Harmony;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate
{
    public class HouseManager
    {
        private RealEstate plugin;

        public List<House> Homes { get => plugin.Configuration.Instance.homes; }
        public List<DisplayName> Names { get => plugin.Configuration.Instance.displayNames; }

        public HouseManager(RealEstate plugin)
        {
            this.plugin = plugin;
        }

        public string GetName(ulong id) => Names.Any(x => x.Id == id) ? Names.First(x => x.Id == id).Name : "???";

        public void AddHouse(House house)
        {
            Homes.Add(house);
            plugin.Configuration.Save();
        }

        public void RemoveHouse(House house)
        {
            Homes.Remove(house);
            plugin.Configuration.Save();
        }

        public int CountPlayerHouses(ulong id) => Homes.Count(x => x.OwnerId == id);

        public int GetPlayerMax(UnturnedPlayer p)
        {
            Permission per = p.GetPermissions().FirstOrDefault(x => x.Name.StartsWith("realestate.maxhouses."));

            if (per != null)
                if (int.TryParse(per.Name.Split('.').Last(), out int max))
                    return max;

            return plugin.Configuration.Instance.defaultMaxHomes;
        }

        public void SetHousePrice(ushort id, decimal price)
        {
            House home = Homes.FirstOrDefault(x => x.Id == id);

            if (home != null)
            {
                home.Price = price;
                plugin.Configuration.Save();
            }
        }

        public void SetHouseOwner(ushort id, ulong? ownerId)
        {
            House home = Homes.FirstOrDefault(x => x.Id == id);

            if (home != null)
            {
                home.OwnerId = ownerId;

                if (ownerId == null && plugin.Configuration.Instance.destroyStructuresOnEviction)
                {
                    foreach (BarricadeRegion r in BarricadeManager.regions)
                        for (int i = r.drops.Count - 1; i >= 0; i--)
                        {
                            if (HouseFromPosition(r.drops.ElementAt(i).model.position) != home)
                                continue;

                            if (!BarricadeManager.tryGetInfo(r.drops.ElementAt(i).model, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region))
                                continue;

                            BarricadeManager.instance.channel.send("tellTakeBarricade", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                            {
                                x,
                                y,
                                plant,
                                index
                            });
                        }

                    foreach (StructureRegion r in StructureManager.regions)
                        for (int i = r.drops.Count - 1; i >= 0; i--)
                        {
                            if (HouseFromPosition(r.drops.ElementAt(i).model.position) != home)
                                continue;

                            if (!StructureManager.tryGetInfo(r.drops.ElementAt(i).model, out byte x, out byte y, out ushort plant, out StructureRegion region, out StructureDrop index))
                                continue;

                            StructureManager.instance.channel.send("tellTakeStructure", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                            {
                                x,
                                y,
                                plant,
                                index
                            });
                        }
                }

                home.LastPaid = DateTime.Now;
                plugin.Configuration.Save();
            }
        }

        public LevelObject LevelObjectFromPosition(Vector3 pos)
        {
            foreach (List<LevelObject> o in LevelObjects.objects)
            {
                LevelObject obj = o.Where(x => NullCheck(x)).FirstOrDefault(x => x.transform.GetComponent<Collider>().bounds.Contains(pos));

                if (obj != null)
                    return obj;
            }

            return null;
        }

        public House HouseFromPosition(Vector3 pos)
        {
            foreach (List<LevelObject> o in LevelObjects.objects)
            {
                LevelObject obj = o.Where(x => NullCheck(x)).FirstOrDefault(x => x.transform.GetComponent<Collider>().bounds.Contains(pos));

                if (obj != null)
                    return Homes.FirstOrDefault(x => x.Id == obj.asset.id);
            }

            return null;
        }

        private bool NullCheck(LevelObject o) => o.transform != null && o.transform.GetComponent<Collider>() != null;
    }
}
