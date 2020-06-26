using fr34kyn01535.Uconomy;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections;
using System.Linq;
using Rocket.API.Collections;
using UnityEngine;
using ExtraConcentratedJuice.RealEstate.Entities;
using HarmonyLib;
using Logger = Rocket.Core.Logging.Logger;

namespace ExtraConcentratedJuice.RealEstate
{
    public class RealEstate : RocketPlugin<RealEstateConfiguration>
    {
        public static RealEstate instance;
        public static HouseManager manager;

        protected override void Load()
        {
            instance = this;

            if (Configuration.Instance.feePaymentTimeInMinutes > 0) 
                instance.StartCoroutine(PaymentCoroutine());

            U.Events.OnPlayerConnected += OnConnected;

            manager = new HouseManager(this);

            Harmony harmony = new Harmony("pw.cirno.extraconcentratedjuice");
            harmony.PatchAll(Assembly);
            
            Logger.Log("RealEstate by ExtraGayJuice loaded");
            Logger.Log("Visit https://iceplugins.xyz/RealEstate for help");
        }

        protected override void Unload()
        {
            instance.StopCoroutine(PaymentCoroutine());
            U.Events.OnPlayerConnected -= OnConnected;
        }

        private IEnumerator PaymentCoroutine()
        {
            while (true)
            {
                foreach (House x in Configuration.Instance.homes)
                {
                    if (x.LastPaid == null || x.OwnerId == null)
                        continue;

                    UnturnedPlayer player = PlayerTool.getPlayer(new Steamworks.CSteamID(x.OwnerId.Value)) == null
                        ? null
                        : UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(x.OwnerId.Value));

                    if (!((DateTime.Now - x.LastPaid.Value).TotalMinutes > Configuration.Instance.feePaymentTimeInMinutes)) 
                        continue;
                    
                    if (Uconomy.Instance.Database.GetBalance(x.OwnerId.ToString()) < x.Price)
                    {
                        if (player != null)
                            instance.TellPlayer(player, "eviction_notice", Color.red);

                        manager.SetHouseOwner(x.Id, x.Position, null);
                        continue;
                    }

                    if (player != null)
                        instance.TellPlayer(player, "upkeep_payment", Palette.SERVER,
                            Configuration.Instance.currencySymbol, x.Price);

                    Uconomy.Instance.Database.IncreaseBalance(x.OwnerId.ToString(), -x.Price);
                    x.LastPaid = DateTime.Now;
                    Configuration.Save();
                }

                yield return new WaitForSeconds(instance.Configuration.Instance.paymentCheckIntervalInSeconds);
            }
        }

        private void OnConnected(UnturnedPlayer p)
        {
            DisplayName n = Configuration.Instance.displayNames.FirstOrDefault(x => x.Id == p.CSteamID.m_SteamID);

            if (n != null)
            {
                if (p.DisplayName == n.Name)
                    return;
                
                n.Name = p.DisplayName;
                Configuration.Save();
            }
            else
            {
                Configuration.Instance.displayNames.Add(new DisplayName(p.CSteamID.m_SteamID, p.DisplayName));
                Configuration.Save();
            }
        }

        public void TellPlayer(UnturnedPlayer player, string translationKey, Color color, params object[] placeholders)
        {
            SteamPlayer realPlayer = player.Player.channel.owner;
            ChatManager.serverSendMessage(instance.Translate(translationKey, placeholders), color, toPlayer: realPlayer);
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                { "house_not_found", "You aren't within the bounds of a house." },
                { "registered_house_not_found", "You aren't within the bounds of a registered home." },
                { "eviction", "The owner of this house has been evicted." },
                { "removed", "This house has been removed from the catalog." },
                { "check_house_owned", "This house belongs to {0} and is valued at {1}{2}."},
                { "check_house_unowned", "This house currently does not belong to anybody and is valued at {0}{1}." },
                { "owner_exists", "This house already has an owner." },
                { "cannot_afford", "You cannot afford this house ({0}{1})." },
                { "house_purchased", "You've purchased this house for {0}{1}." },
                { "house_added", "You've added this house to the catalog for {0}{1}." },
                { "house_updated", "You've updated the price of this house to {0}{1}." },
                { "eviction_notice", "You have failed to pay one of your house's upkeeps and have been evicted." },
                { "upkeep_payment", "You've paid {0}{1} for the upkeep of one of your homes." },
                { "max_reached", "You have reached the maximum number of homes allocated to you. ({0})" },
                { "abandoned", "You have abandoned this house." },
                { "cannot_abandon", "This house doesn't even belong to you!" },
                { "cant_place_barricades", "You cannot place barricades in a house that does not belong to you." },
                { "cant_place_structures", "You cannot place structures in a house that does not belong to you." },
                { "cant_build_outside", "Building outside of a house that you own has been disabled." },
                { "command_add_syntax", "The correct syntax is /addhouse <price>" }
            };
    }
}
