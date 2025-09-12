using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Items;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.InformationTransformer;

namespace ArchipelagoSurvivors.Patches;

public static class ChestPickupPatch
{
    public static int ChestsOpened = 0;
    public static Dictionary<StageType, int> StageTreasureTracker = [];

    [HarmonyPatch(typeof(TreasureChest), "TrackItemPickup"), HarmonyPrefix]
    public static void TrackItem(bool trackRunPickup, TreasureChest __instance)
    {
        var stageType = GM.Core.Stage.StageType;
        StageTreasureTracker.TryAdd(stageType, 0);
        ChestsOpened++;

        if (StageTreasureTracker[stageType] >= ChestsOpened || StageTreasureTracker[stageType] >= ChestCheckAmount) return;
        StageTreasureTracker[stageType]++;
        AddLocationToQueue($"Open Chest #{ChestsOpened} on {StageTypeToName[stageType]}");
    }
}