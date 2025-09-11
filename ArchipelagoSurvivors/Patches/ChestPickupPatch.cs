using HarmonyLib;
using Il2CppVampireSurvivors.Objects.Items;

namespace ArchipelagoSurvivors.Patches;

public static class ChestPickupPatch
{
    public static int ChestsOpened = 0;

    [HarmonyPatch(typeof(TreasureChest), "TrackItemPickup"), HarmonyPrefix]
    public static void TrackItem(bool trackRunPickup, TreasureChest __instance)
    {
        Core.Log.Msg($" == PICKUP TRACKED == -> ({ChestsOpened += 1})");
    }
}