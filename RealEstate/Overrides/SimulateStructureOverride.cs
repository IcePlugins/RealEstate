using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Reflection;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Overrides
{
    internal class SimulateStructureOverride
    {
        internal static bool Prefix(UseableStructure __instance, uint simulation, bool inputSteady)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(__instance.player);

            if (player.HasPermission("realestate.bypass"))
                return true;

            if (GetValue<bool>(__instance, "isUsing", BindingFlags.NonPublic | BindingFlags.Instance))
            {
                House house = RealEstate.manager.HouseFromPosition(GetValue<Vector3>(__instance, "point", BindingFlags.NonPublic | BindingFlags.Instance));

                if (house != null && (house.OwnerId == null || house.OwnerId != null && house.OwnerId.Value != player.CSteamID.m_SteamID))
                {
                    __instance.player.equipment.dequip();
                    UnturnedChat.Say(player, "You cannot place structures in a house that does not belong to you.", Color.red);
                    return false;
                }
            }

            return true;
        }

        internal static void PostFix()
        {
            // lol
        }

        private static T GetValue<T>(UseableStructure instance, string name, BindingFlags flags) 
            => (T)typeof(UseableStructure).GetField(name, flags).GetValue(instance);
    }
}
