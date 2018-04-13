using ExtraConcentratedJuice.RealEstate.Entities;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Commands
{
    public class CommandBuyHouse : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "buyhouse";

        public string Help => "Buys the house that the caller is standing in.";

        public string Syntax => "/buyhouse";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "realestate.buyhouse" };

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

            int max = RealEstate.manager.GetPlayerMax(player);

            if (RealEstate.manager.CountPlayerHouses(player.CSteamID.m_SteamID) >= max)
            {
                UnturnedChat.Say(caller, RealEstate.instance.Translate("max_reached", max), Color.red);
                return;
            }

            if (h.OwnerId != null)
                UnturnedChat.Say(caller, RealEstate.instance.Translate("owner_exists"));
            else
            {
                if (Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < h.Price)
                {
                    UnturnedChat.Say(caller, RealEstate.instance.Translate("cannot_afford", RealEstate.instance.Configuration.Instance.currencySymbol, h.Price), Color.red);
                    return;
                }

                Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -h.Price);
                UnturnedChat.Say(caller, RealEstate.instance.Translate("house_purchased", RealEstate.instance.Configuration.Instance.currencySymbol, h.Price));
                RealEstate.manager.SetHouseOwner(h.Id, player.CSteamID.m_SteamID);
            }
        }
    }
}