using HarmonyLib;
using Il2CppTMPro;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Enemies;
using Il2CppVampireSurvivors.UI;
using UnityEngine;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.Patches.EnemyCounterPatch;
using Random = System.Random;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class MainMenuPatch
{
    public static GameObject StartButton;
    public static GameObject BestiaryButton;
    public static EnemyType[] Unknown;
    public static Dictionary<EnemyType, string> Names = [];

    [HarmonyPatch(typeof(MainMenuPage), "Start"), HarmonyPostfix]
    public static void HideButtons(MainMenuPage __instance)
    {
        var container = __instance.GetChild(5);
        StartButton = container.GetChild(0);
        BestiaryButton = container.GetChild(6);
        
        if (Random.Shared.Next(100) == 42)
        {
            StartButton.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FISH";
            Log.Msg("FISH");
        }
        
        container.GetChild(1).AddComponent<Invisinator>(); // quick start button
        container.GetChild(2).AddComponent<Invisinator>(); // online button
        container.GetChild(13).AddComponent<Invisinator>(); // adventure button
        __instance.gameObject.AddComponent<APGui>();
    }

    [HarmonyPatch(typeof(EnemyItemUI), "SetData"), HarmonyPrefix]
    public static void BeforeBestiarySetData(EnemyItemUI __instance, EnemyType type, int count, EnemyData dat,
        BestiaryPage page, ref bool hasKilled)
    {
        hasKilled = true;
    }

    [HarmonyPatch(typeof(EnemyItemUI), "SetData"), HarmonyPostfix]
    public static void BestiarySetData(EnemyItemUI __instance, EnemyType type, int count, EnemyData dat,
        BestiaryPage page, bool hasKilled)
    {
        try
        {;
            var enemyName = __instance._type.GetName(out var enemyType);
            if (enemyName is "") return;
            __instance.gameObject.SetActive(false);

            if (Unknown is not null && Unknown.Contains(enemyType)) Names[enemyType] = __instance._Name.text;
            if (Client is null) return;
            
            if (!EnemysanityEnabled) return;
            var killed = !Client.MissingLocations.Contains($"Kill {enemyName}");
            __instance._Name.text = $"[Unkilled] {__instance._Name.text}";
            __instance.gameObject.SetActive(!killed);
        }
        catch(Exception e)
        {
            Log.Msg(e);
        }
    }
}