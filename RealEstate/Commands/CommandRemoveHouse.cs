using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandRemoveHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "removehouse";

        public string Help => "Removes a house from the catalog.";

        public string Syntax => "/removehouse";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.removehouse" };

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

            RealEstate.manager.RemoveHouse(h);
            RealEstate.instance.TellPlayer(player, "removed", Palette.SERVER);
        }
    }
}