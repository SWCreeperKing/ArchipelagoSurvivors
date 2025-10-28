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

public static class EnemyCounterPatch
{
    // public static Dictionary<EnemyType, int> KillCounter = new();
    private static List<int> Warnlist = [];
    private static List<EnemyType> Warnlist2 = [];
    public static List<EnemyType> EnemyTypes;
    public static Dictionary<EnemyType, EnemyType> EnemyVariantListings = [];

    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance)
    {
        if (!EnemysanityEnabled) return;

        var enemyType = __instance.EnemyType;
        // Log.Msg("a");
        if (EnemyVariantListings.TryGetValue(enemyType, out var potentialType)) enemyType = potentialType;
        // Log.Msg("b");
        if (!EnemyTypeToName.TryGetValue(enemyType, out var value))
        {
            if (EnemyTypes.Contains(enemyType)) return;
            EnemyTypes.Add(enemyType);
            Log.Error($"New enemy encounter: ({__instance._defaultName}) [{enemyType}]");
            return;
        }

        // Log.Msg($"c: [{EnemyVariantListings.ContainsKey(ANGEL1)}] [{EnemyStages.ContainsKey(ANGEL1)}] || [{__instance.EnemyType}] => [{enemyType}] || [{EnemyVariantListings.ContainsKey(__instance.EnemyType)}]");

        if (!EnemyStages.ContainsKey(enemyType))
        {
            if (Warnlist2.Contains(__instance.EnemyType)) return;
            Warnlist2.Add(__instance.EnemyType);
            Log.Error($"Enemy: [{__instance.EnemyType}], variant of: [{enemyType}] does not show up in Enemysanity, please report");
            return;
        }
        
        var stage = GM.Core.Stage.StageType;
        if (!EnemyStages[enemyType].Contains(stage)) return;
        // Log.Msg("d");
        if (EnemyHurryStages.ContainsKey(enemyType) && IsHurryLocked && EnemyHurryStages[enemyType].Contains(stage))
        {
            // Log.Msg("d.a");
            var hash = HashCode.Combine(enemyType, stage);
            // Log.Msg("d.b");
            if (!Warnlist.Contains(hash))
            {
                Log.Warning(
                    $"Not sending enemy kill check [Kill {EnemyTypeToName[enemyType]}] as hurry is required for it to be in logic");
                Warnlist.Add(hash);
            }

            return;
        }

        // Log.Msg("e");
        AddLocationToQueue($"Kill {value}");

        if (enemyType != DIRECTER || APSurvivorClient.GoalRequirement != KillTheDirector) return;
        Client?.Goal();
    }
}