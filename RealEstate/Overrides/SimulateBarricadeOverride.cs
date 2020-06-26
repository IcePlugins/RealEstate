using ExtraConcentratedJuice.RealEstate.Entities;
using HarmonyLib;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ExtraConcentratedJuice.RealEstate.Overrides
{
    [HarmonyPatch(typeof(UseableBarricade), "simulate")]
    internal class SimulateBarricadeOverride
    {
        [HarmonyPrefix]
        internal static bool Prefix(UseableBarricade __instance, uint simulation, bool inputSteady, bool ___isUsing, InteractableVehicle ___parentVehicle, Vector3 ___point)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(__instance.player);

            if (player.HasPermission("realestate.bypass"))
                return true;

            if (!___isUsing)
                return true;
            
            if (___parentVehicle != null && RealEstate.instance.Configuration.Instance.enableBuildingOnVehicles)
                return true;

            House house = RealEstate.manager.HouseFromPosition(___point);

            if (house == null && RealEstate.instance.Configuration.Instance.disableBuildingIfNotInHome)
            {
                __instance.player.equipment.dequip();
                RealEstate.instance.TellPlayer(player, RealEstate.instance.Translate("cant_build_outside"), Color.red);
                return false;
            }

            if (HouseManager.HouseCheck(house, player))
                return true;
                
            __instance.player.equipment.dequip();
            RealEstate.instance.TellPlayer(player, RealEstate.instance.Translate("cant_place_barricades"), Color.red);
            return false;

        }
    }
}
