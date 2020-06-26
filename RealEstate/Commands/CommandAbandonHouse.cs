using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandAbandonHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "abandonhouse";

        public string Help => "Lets the player give up ownership of their house.";

        public string Syntax => "/abandonhouse";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.abandonhouse" };

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

            if (h.OwnerId != player.CSteamID.m_SteamID)
            {
                RealEstate.instance.TellPlayer(player, "cannot_abandon", Color.red);
                return;
            }

            RealEstate.manager.SetHouseOwner(h.Id, h.Position, null);
            RealEstate.instance.TellPlayer(player, "abandoned", Palette.SERVER);
        }
    }
}