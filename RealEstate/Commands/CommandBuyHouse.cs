using ExtraConcentratedJuice.RealEstate.Entities;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
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
                RealEstate.instance.TellPlayer(player, "registered_house_not_found", Color.red);
                return;
            }

            int max = RealEstate.manager.GetPlayerMax(player);

            if (RealEstate.manager.CountPlayerHouses(player.CSteamID.m_SteamID) >= max)
            {
                RealEstate.instance.TellPlayer(player, "max_reached", Color.red, max);
                return;
            }

            if (h.OwnerId != null)
                RealEstate.instance.TellPlayer(player, "owner_exists", Palette.SERVER);
            else
            {
                if (Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < h.Price)
                {
                    RealEstate.instance.TellPlayer(player, "cannot_afford", Color.red, RealEstate.instance.Configuration.Instance.currencySymbol, h.Price);
                    return;
                }

                Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -h.Price);
                RealEstate.instance.TellPlayer(player, "house_purchased", Palette.SERVER, RealEstate.instance.Configuration.Instance.currencySymbol, h.Price);
                RealEstate.manager.SetHouseOwner(h.Id, h.Position, player.CSteamID.m_SteamID);
            }
        }
    }
}