using HarmonyLib;
using Il2CppNewtonsoft.Json;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Signals;
using Il2CppZenject;
using MelonLoader;
using Microsoft.VisualBasic;
using UnityEngine;
using static AutoUnexcludeEquippedWeapons.Core;
using Action = Il2CppSystem.Action;

[assembly:
    MelonInfo(typeof(AutoUnexcludeEquippedWeapons.Core), "AutoUnexcludeEquippedWeapons", "1.0.0", "SW_CreeperKing",
        null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]

namespace AutoUnexcludeEquippedWeapons;

public class Core : MelonMod
{
    public static MelonLogger.Instance Log;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonyInstance.PatchAll(typeof(WeaponPatch));

        LoggerInstance.Msg("Initialized.");
    }
}

public static class WeaponPatch
{
    [HarmonyPatch(typeof(SignalBus), "InternalFire"), HarmonyPrefix]
    public static void MonitorAllSignals(Il2CppSystem.Type signalType, Il2CppSystem.Object signal)
    {
        var signalClass = signalType.Name switch
        {
            "WeaponAddedToCharacterSignal" => "Weapon",
            "AccessoryAddedToCharacterSignal" => "Accessory",
            _ => ""
        };
        
        if (signalClass == "") return;
        
        var realWeaponType = (GameplaySignals.WeaponAddedToCharacterSignal)signal;
        Log.Msg($"weapon: [{realWeaponType.Weapon}]");
        
        var weaponType = FindAndParse(JsonUtility.ToJson(signal), signalClass);
        GM.Core.LevelUpFactory.ExcludedWeapons.Remove(weaponType);
        GM.Core.LevelUpFactory.BanishedWeapons.Remove(weaponType);
    }

    public static WeaponType FindAndParse(string json, string text)
    {
        var weaponIndex = json.IndexOf(text, StringComparison.Ordinal) + text.Length + 2;
        var weaponEnd = json.IndexOf('"', weaponIndex) - 1;
        return (WeaponType)int.Parse(json[weaponIndex..weaponEnd]);
    }
}