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

                if (house == null && RealEstate.instance.Configuration.Instance.disableBuildingIfNotInHome)
                {
                    __instance.player.equipment.dequip();
                    UnturnedChat.Say(player, RealEstate.instance.Translate("cant_build_outside"), Color.red);
                    return false;
                }

                if (!HouseManager.HouseCheck(house, player))
                {
                    __instance.player.equipment.dequip();
                    UnturnedChat.Say(player, RealEstate.instance.Translate("cant_place_structures"), Color.red);
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
