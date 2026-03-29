using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class EnemyCounterPatch
{
    private static List<int> Warnlist = [];
    private static List<EnemyType> Warnlist2 = [];

    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance) => EnemyDied(__instance.EnemyType);
    
    [HarmonyPatch(typeof(EnemyBigFuzz), "Die"), HarmonyPostfix]
    public static void CountBigFuzz(EnemyController __instance) => EnemyDied(EnemyType.BOSS_FB_BIGFUZZ);

    public static void EnemyDied(EnemyType type)
    {
        try
        {
            if (!EnemysanityEnabled) return;

            var enemyName = type.GetName(out var enemyType);
            if (enemyName is "") return;

            if (!EnemyStages.ContainsKey(enemyType))
            {
                if (Warnlist2.Contains(enemyType)) return;
                Warnlist2.Add(enemyType);
                Log.Error(
                    $"Enemy: [{type}], variant of: [{enemyType}] does not show up in Enemysanity, please report"
                );
                return;
            }

            var stage = GM.Core.Stage.StageType;
            if (!EnemyStages[enemyType].Contains(stage))
            {
                Log.Warning($"Enemy [{enemyType}] killed on [{stage}]???, not listed, report please");
                return;
            }
            
            if (EnemyHurryStages.ContainsKey(enemyType) && IsHurryLocked && EnemyHurryStages[enemyType].Contains(stage))
            {
                var hash = HashCode.Combine(enemyType, stage);
                if (Warnlist.Contains(hash)) return;
                Log.Warning(
                    $"Not sending enemy kill check [Kill {EnemyTypeToName[enemyType]}] as hurry is required for it to be in logic"
                );
                Warnlist.Add(hash);
                return;
            }

            AddLocationToQueue($"Kill {enemyName}");
            
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}