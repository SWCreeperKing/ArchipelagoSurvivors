using HarmonyLib;
using Il2CppNewtonsoft.Json;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppZenject;
using MelonLoader;
using Microsoft.VisualBasic;
using UnityEngine;
using static AutoUnexcludeEquippedWeapons.Core;
using static Il2CppVampireSurvivors.Signals.GameplaySignals;
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
        var weaponType = signalType.Name switch
        {
            "WeaponAddedToCharacterSignal" => new WeaponAddedToCharacterSignal(signal.Pointer).Weapon,
            "AccessoryAddedToCharacterSignal" => new AccessoryAddedToCharacterSignal(signal.Pointer).Accessory,
            _ => WeaponType.VOID
        };

        if (weaponType == 0) return;
        if (!GM.Core.LevelUpFactory.BanishedWeapons.Contains(weaponType)) return;

        Log.Msg($"Unsealing/Unbanishing [{signalType.Name.Replace("AddedToCharacterSignal", "")}] [{weaponType}]");
        
        GM.Core.LevelUpFactory.ExcludedWeapons.Remove(weaponType);
        GM.Core.LevelUpFactory.BanishedWeapons.Remove(weaponType);
    }
}