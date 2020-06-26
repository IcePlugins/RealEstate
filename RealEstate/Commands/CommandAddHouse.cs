using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandAddHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addhouse";

        public string Help => "Allows the house that the caller is standing in to be sold at the defined price.";

        public string Syntax => "/addhouse <price>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.addhouse" };

        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            
            if (args.Length < 1)
            {
                RealEstate.instance.TellPlayer(player, "command_add_syntax", Color.red);
                return;
            }

            if (!Decimal.TryParse(args[0], out decimal price))
            {
                RealEstate.instance.TellPlayer(player, "command_add_syntax", Color.red);
                return;
            }

            LevelObject h = RealEstate.manager.LevelObjectFromPosition(player.Position);

            if (h == null)
            {
                RealEstate.instance.TellPlayer(player, "house_not_found", Color.red);
                return;
            }

            if (RealEstate.manager.Homes.Any(x => x.Id == h.asset.id && x.Position.GetVector3() == h.transform.position))
            {
                RealEstate.manager.SetHousePrice(h.id, new SerializableVector3(h.transform.position), price);
                RealEstate.instance.TellPlayer(player, "house_updated", Palette.SERVER, RealEstate.instance.Configuration.Instance.currencySymbol, price);
            }
            else
            {
                RealEstate.manager.AddHouse(new House(h.asset.id, price, h.transform.position));
                RealEstate.instance.TellPlayer(player, "house_added", Palette.SERVER, RealEstate.instance.Configuration.Instance.currencySymbol, price);
            }
        }
    }
}