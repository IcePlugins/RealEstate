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
            if (args.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax, Color.red);
                return;
            }

            if (!Decimal.TryParse(args[0], out decimal price))
            {
                UnturnedChat.Say(caller, Syntax, Color.red);
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;

            LevelObject h = RealEstate.manager.LevelObjectFromPosition(player.Position);

            if (h == null)
            {
                UnturnedChat.Say(caller, RealEstate.instance.Translate("house_not_found"), Color.red);
                return;
            }

            if (RealEstate.manager.Homes.Any(x => x.Id == h.asset.id))
            {
                RealEstate.manager.SetHousePrice(h.id, price);
                UnturnedChat.Say(caller, RealEstate.instance.Translate("house_updated", RealEstate.instance.Configuration.Instance.currencySymbol, price));
            }
            else
            {
                RealEstate.manager.AddHouse(new House(h.asset.id, price));
                UnturnedChat.Say(caller, RealEstate.instance.Translate("house_added", RealEstate.instance.Configuration.Instance.currencySymbol, price));
            }
        }
    }
}