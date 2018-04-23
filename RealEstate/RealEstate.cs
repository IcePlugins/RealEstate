using fr34kyn01535.Uconomy;
using Harmony;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using UnityEngine;
using ExtraConcentratedJuice.RealEstate.Overrides;
using ExtraConcentratedJuice.RealEstate.Entities;

namespace ExtraConcentratedJuice.RealEstate
{
    public class RealEstate : RocketPlugin<RealEstateConfiguration>
    {
        public static RealEstate instance;
        public static HouseManager manager;
        private Timer timer;

        protected override void Load()
        {
            instance = this;

            if (Configuration.Instance.feePaymentTimeInMinutes > 0)
            {
                timer = new Timer(1000 * Configuration.Instance.paymentCheckIntervalInSeconds);
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
            }

            U.Events.OnPlayerConnected += OnConnected;

            manager = new HouseManager(this);

            HarmonyInstance harmony = HarmonyInstance.Create("pw.cirno.extraconcentratedjuice");

            var orig = typeof(UseableStructure).GetMethod("simulate", BindingFlags.Instance | BindingFlags.Public);
            var pre = typeof(SimulateStructureOverride).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
            var post = typeof(SimulateStructureOverride).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(orig, new HarmonyMethod(pre), new HarmonyMethod(post));

            var orig2 = typeof(UseableBarricade).GetMethod("simulate", BindingFlags.Instance | BindingFlags.Public);
            var pre2 = typeof(SimulateBarricadeOverride).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
            var post2 = typeof(SimulateBarricadeOverride).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(orig2, new HarmonyMethod(pre2), new HarmonyMethod(post2));
        }

        protected override void Unload()
        {
            if (timer != null)
            {
                timer.Elapsed -= OnTimerElapsed;
                timer.Stop();
            }

            U.Events.OnPlayerConnected -= OnConnected;
        }

        private void OnTimerElapsed(object s, ElapsedEventArgs t)
        {
            foreach (House x in Configuration.Instance.homes)
            {
                if (x.LastPaid == null || x.OwnerId == null)
                    continue;

                UnturnedPlayer player = PlayerTool.getPlayer(new Steamworks.CSteamID(x.OwnerId.Value)) == null ? null :
                    UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(x.OwnerId.Value));

                if ((DateTime.Now - x.LastPaid.Value).TotalMinutes > Configuration.Instance.feePaymentTimeInMinutes)
                {
                    if (Uconomy.Instance.Database.GetBalance(x.OwnerId.ToString()) < x.Price)
                    {
                        if (player != null)
                            UnturnedChat.Say(player, Translate("eviction_notice"), Color.red);

                        manager.SetHouseOwner(x.Id, x.Position, null);
                        continue;
                    }

                    if (player != null)
                        UnturnedChat.Say(player, Translate("upkeep_payment", Configuration.Instance.currencySymbol, x.Price));

                    Uconomy.Instance.Database.IncreaseBalance(x.OwnerId.ToString(), -x.Price);
                    x.LastPaid = DateTime.Now;
                    Configuration.Save();
                }
            }
        }

        private void OnConnected(UnturnedPlayer p)
        {
            DisplayName n = Configuration.Instance.displayNames.FirstOrDefault(x => x.Id == p.CSteamID.m_SteamID);

            if (n != null)
            {
                if (p.DisplayName != n.Name)
                {
                    n.Name = p.DisplayName;
                    Configuration.Save();
                }
            }
            else
            {
                Configuration.Instance.displayNames.Add(new DisplayName(p.CSteamID.m_SteamID, p.DisplayName));
                Configuration.Save();
            }
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
                { "cant_build_outside", "Building outside of a house that you own has been disabled." }
            };
    }
}
