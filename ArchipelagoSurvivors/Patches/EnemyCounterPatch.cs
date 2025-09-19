using HarmonyLib;
using Il2CppVampireSurvivors.Data;
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
    public static List<EnemyType> EnemyTypes = [
        BOSS_XLPINCER, // BOSS_XLCRAB's pincer
        
        // bullets
        BULLET_SM1, BULLET_2, BULLET_3
    ];
    public static Dictionary<EnemyType, EnemyType> EnemyVariantListings = [];

    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance)
    {
        if (!EnemysanityEnabled) return;
     // Log.Msg($"Enemy died: [{__instance.EnemyType}]");
        var enemyType = __instance.EnemyType;
        if (EnemyVariantListings.TryGetValue(enemyType, out var potentialType)) enemyType = potentialType;
        if (!EnemyTypeToName.TryGetValue(enemyType, out var value))
        {
            if (EnemyTypes.Contains(enemyType)) return;
            EnemyTypes.Add(enemyType);
            Log.Error($"New enemy encounter: ({__instance._defaultName}) [{enemyType}]");
            return;
        }
        // Log.Msg($"Killed: [{value}]");
        AddLocationToQueue($"Kill {value}");
        
        if (enemyType != DIRECTER || APSurvivorClient.GoalRequirement != KillTheDirector) return;
        Client?.Goal();

        // KillCounter.TryAdd(__instance._enemyType, 0);

        // Core.Log.Msg($" == ENEMY DIED <{__instance._enemyType}> [{++KillCounter[__instance._enemyType]}] == ");
    }
}