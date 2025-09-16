using System.Text;
using HarmonyLib;
using Il2CppDreamteck.Splines.Primitives;
using Il2CppTMPro;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Enemies;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.InformationTransformer;
using Random = System.Random;

namespace ArchipelagoSurvivors.Patches;

public static class MainMenuPatch
{
    public static GameObject StartButton;
    public static GameObject BestiaryButton;

    [HarmonyPatch(typeof(MainMenuPage), "Start"), HarmonyPostfix]
    public static void HideButtons(MainMenuPage __instance)
    {
        var container = __instance.GetChild(2);
        StartButton = container.GetChild(0);
        BestiaryButton = container.GetChild(5);

        if (Random.Shared.Next(100) == 42)
        {
            StartButton.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FISH";
            Log.Msg("FISH");
        }

        container.GetChild(1).AddComponent<Invisinator>(); // quick start button
        container.GetChild(12).AddComponent<Invisinator>(); // adventure button
        __instance.gameObject.AddComponent<APGui>();
    }

    [HarmonyPatch(typeof(EnemyItemUI), "SetData"), HarmonyPrefix]
    public static void BeforeBestiarySetData(EnemyItemUI __instance, EnemyType type, int count, EnemyData dat,
        BestiaryPage page, ref bool hasKilled)
    {
        hasKilled = true;
    }

    // private static StringBuilder sb = new();
    // private static StringBuilder sb2 = new();

    [HarmonyPatch(typeof(EnemyItemUI), "SetData"), HarmonyPostfix]
    public static void BestiarySetData(EnemyItemUI __instance, EnemyType type, int count, EnemyData dat,
        BestiaryPage page, bool hasKilled)
    {
        // var enemySpawnList = __instance._data.bPlaces._items.Select(st => StageTypeToName[st]).ToList();
        //
        // int i;
        // while ((i = enemySpawnList.FindLastIndex(s => s == "Mad Forest")) > 0)
        // {
        //     enemySpawnList.RemoveAt(i);
        // }
        
        // sb.Append($"\"{__instance._Name.text}\": [{string.Join(", ", enemySpawnList.Select(s => $"\"{s}\""))}],\n");
        // sb2.Append($"[{type}] = \"{__instance._Name.text.Trim()}\",\n");
        if (Client is null) return;
        var killed = Client.MissingLocations.All(kv => kv.Value.LocationName != $"Kill {EnemyTypeToName[__instance._type]}");
        __instance._Name.text = $"[{(killed ? "Killed" : "Unkilled")}] {__instance._Name.text}";
        __instance.gameObject.SetActive(!killed);

        // if (type == EnemyType.EME_GATEBOSS_LIVINGANGUISH)
        // {
        //     Log.Msg(sb.ToString());
        //     File.WriteAllText("Enemy List.txt", sb.ToString());
        //     // File.WriteAllText("Enemy Name List.txt", sb2.ToString());
        // }
    }
}