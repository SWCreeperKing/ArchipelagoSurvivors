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
using static ArchipelagoSurvivors.Core;
using Random = System.Random;

namespace ArchipelagoSurvivors.Patches;

public static class MainMenuPatch
{
    public static GameObject StartButton;

    [HarmonyPatch(typeof(MainMenuPage), "Start"), HarmonyPostfix]
    public static void HideButtons(MainMenuPage __instance)
    {
        var container = __instance.GetChild(2);
        StartButton = container.GetChild(0);

        if (Random.Shared.Next(100) == 42)
        {
            StartButton.GetChild(0).GetComponent<TextMeshProUGUI>().text = "FISH";
            Log.Msg("FISH");
        }
        
        container.GetChild(1).AddComponent<Invisinator>(); // quick start button
        container.GetChild(12).AddComponent<Invisinator>(); // adventure button
        __instance.gameObject.AddComponent<APGui>();
    }
}