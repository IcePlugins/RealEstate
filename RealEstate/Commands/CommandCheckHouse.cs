using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandCheckHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "checkhouse";

        public string Help => "Tells the caller the price and owner of the house that they are standing in.";

        public string Syntax => "/checkhouse";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.checkhouse" };

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

            if (h.OwnerId != null)
                UnturnedChat.Say(caller, RealEstate.instance.Translate("check_house_owned", RealEstate.manager.GetName(h.OwnerId.Value), RealEstate.instance.Configuration.Instance.currencySymbol, h.Price));
            else
                UnturnedChat.Say(caller, RealEstate.instance.Translate("check_house_unowned", RealEstate.instance.Configuration.Instance.currencySymbol, h.Price));
        }
    }
}