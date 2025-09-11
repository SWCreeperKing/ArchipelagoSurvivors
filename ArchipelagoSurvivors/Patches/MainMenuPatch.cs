using HarmonyLib;
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
        // GM.Core.Player;
        //     WeaponType
        // GM.Core.Player.die;

        var container = __instance.GetChild(2);
        // StartButton = container.GetChild(0);
        container.GetChild(1).AddComponent<Invisinator>(); // quick start button
        container.GetChild(12).AddComponent<Invisinator>(); // adventure button
        __instance.gameObject.AddComponent<APGui>();
        // __instance.gameObject.AddComponent<Testing>();
        // __instance.GetParent().GetChild(11).AddComponent<Testing2>();
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
        Log.Msg($"[{type}] [{__instance._Name.text}]");
        __instance._Name.text = $"[{Random.Shared.Next(1000):###,###}/1,000] {__instance._Name.text}";
    }
}

// [RegisterTypeInIl2Cpp]
// public class Testing : MonoBehaviour
// {
//     public void Update()
//     {
//         if (GM.Core is null) return;
//         var player = GM.Core.Player;
//         Log.Msg("has core");
//         if (player is null) return; 
//         Log.Msg($"[{player}] [{player.Magnet}] [{player.MaxAccessoryCount}] + [{player._maxAccessoryBonus}]");
//     }
// } 

// [RegisterTypeInIl2Cpp]
// public class Testing2 : MonoBehaviour
// {
//     public void Start()
//     {
//         Log.Msg("Start");
//     }
//
//     public void Awake()
//     {
//         Log.Msg("Awake");
//     }
//     
//     public void OnEnable()
//     {
//         Log.Msg("Enable");
//         var enemyList = this.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChildren().Select(child => child.GetComponent<EnemyItemUI>()).ToArray();
//         
//         foreach (var enemy in enemyList)
//         {
//             Log.Msg($"{enemy._type}, {enemy.name} = {enemy._Name.text}");
//         }
//     }
// }