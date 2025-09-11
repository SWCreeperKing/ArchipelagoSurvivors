using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;

namespace ArchipelagoSurvivors.Patches;

public static class EnemyCounterPatch
{
    public static Dictionary<EnemyType, int> KillCounter = new();
    
    [HarmonyPatch(typeof(EnemyController), "Die"), HarmonyPostfix]
    public static void Count(EnemyController __instance)
    {
        if (!KillCounter.ContainsKey(__instance._enemyType)) KillCounter[__instance._enemyType] = 0;
        
        Core.Log.Msg($" == ENEMY DIED <{__instance._enemyType}> [{KillCounter[__instance._enemyType] += 1}] == ");
    }
}