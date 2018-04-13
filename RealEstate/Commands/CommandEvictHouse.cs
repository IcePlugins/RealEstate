using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
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
                UnturnedChat.Say(caller, RealEstate.instance.Translate("registered_house_not_found"), Color.red);
                return;
            }

            RealEstate.manager.SetHouseOwner(h.Id, null);
            UnturnedChat.Say(caller, RealEstate.instance.Translate("eviction"));
        }
    }
}