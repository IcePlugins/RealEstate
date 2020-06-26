using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
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
                RealEstate.instance.TellPlayer(player, "registered_house_not_found", Color.red);
                return;
            }

            if (h.OwnerId != null)
                RealEstate.instance.TellPlayer(player, "check_house_owned", Palette.SERVER, RealEstate.manager.GetName(h.OwnerId.Value), RealEstate.instance.Configuration.Instance.currencySymbol, h.Price);
            else
                RealEstate.instance.TellPlayer(player, "check_house_unowned", Palette.SERVER, RealEstate.instance.Configuration.Instance.currencySymbol, h.Price);
        }
    }
}