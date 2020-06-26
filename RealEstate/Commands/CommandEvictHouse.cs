using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandEvictHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "evicthouse";

        public string Help => "Allows a player to reset the owner of the home that they are standing in.";

        public string Syntax => "/evicthouse";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.evicthouse" };

        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            House h = RealEstate.manager.HouseFromPosition(player.Position);

            if (h == null)
            {
                RealEstate.instance.TellPlayer(player, "registered_house_not_found", Color.red);
                return;
            }

            RealEstate.manager.SetHouseOwner(h.Id, h.Position, null);
            RealEstate.instance.TellPlayer(player, "eviction", Palette.SERVER);
        }
    }
}