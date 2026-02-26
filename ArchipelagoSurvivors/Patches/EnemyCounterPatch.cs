using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.GoalRequirement;
using static ArchipelagoSurvivors.InformationTransformer;
using static Il2CppVampireSurvivors.Data.EnemyType;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class EnemyCounterPatch
{
    private static List<int> Warnlist = [];
    private static List<EnemyType> Warnlist2 = [];
    public static List<EnemyType> EnemyTypes;
    public static Dictionary<EnemyType, EnemyType> EnemyVariantListings = [];

    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance)
    {
        if (!EnemysanityEnabled) return;

        var enemyType = __instance.EnemyType;
        if (EnemyVariantListings.TryGetValue(enemyType, out var potentialType)) enemyType = potentialType;
        if (!EnemyTypeToName.TryGetValue(enemyType, out var value))
        {
            if (EnemyTypes.Contains(enemyType)) return;
            EnemyTypes.Add(enemyType);
            Log.Error($"New enemy encounter: ({__instance._defaultName}) [{enemyType}]");
            return;
        }

        if (!EnemyStages.ContainsKey(enemyType))
        {
            if (Warnlist2.Contains(__instance.EnemyType)) return;
            Warnlist2.Add(__instance.EnemyType);
            Log.Error($"Enemy: [{__instance.EnemyType}], variant of: [{enemyType}] does not show up in Enemysanity, please report");
            return;
        }
        
        var stage = GM.Core.Stage.StageType;
        if (!EnemyStages[enemyType].Contains(stage)) return;
        if (EnemyHurryStages.ContainsKey(enemyType) && IsHurryLocked && EnemyHurryStages[enemyType].Contains(stage))
        {
            var hash = HashCode.Combine(enemyType, stage);
            if (Warnlist.Contains(hash)) return;
            Log.Warning(
                $"Not sending enemy kill check [Kill {EnemyTypeToName[enemyType]}] as hurry is required for it to be in logic");
            Warnlist.Add(hash);
            return;
        }

        AddLocationToQueue($"Kill {value}");

        if (enemyType != DIRECTER || APSurvivorClient.GoalRequirement != KillTheDirector) return;
        Client?.Goal();
    }
}