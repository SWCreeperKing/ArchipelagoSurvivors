using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.GoalRequirement;
using static ArchipelagoSurvivors.InformationTransformer;

namespace ArchipelagoSurvivors.Patches;

public static class EnemyCounterPatch
{
    // public static Dictionary<EnemyType, int> KillCounter = new();
    public static List<EnemyType> EnemyTypes = [];

    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance)
    {
        if (!EnemysanityEnabled) return;
     // Log.Msg($"Enemy died: [{__instance.EnemyType}]");
        var enemyType = __instance.EnemyType;
        if (EnemyVariantTypes.TryGetValue(enemyType, out var potentialType)) enemyType = potentialType;
        if (!EnemyTypeToName.TryGetValue(enemyType, out var value))
        {
            if (EnemyTypes.Contains(__instance.EnemyType)) return;
            EnemyTypes.Add(__instance.EnemyType);
            Log.Msg($"New enemy encounter: ({__instance._defaultName}) [{__instance._enemyType}]");
            return;
        }
        // Log.Msg($"Killed: [{value}]");
        AddLocationToQueue($"Kill {value}");
        
        if (enemyType != EnemyType.DIRECTER || APSurvivorClient.GoalRequirement != KillTheDirector) return;
        Client?.Goal();

        // KillCounter.TryAdd(__instance._enemyType, 0);

        // Core.Log.Msg($" == ENEMY DIED <{__instance._enemyType}> [{++KillCounter[__instance._enemyType]}] == ");
    }
}