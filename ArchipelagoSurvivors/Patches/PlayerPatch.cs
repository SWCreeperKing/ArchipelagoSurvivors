using HarmonyLib;
using Il2CppSystem.Text;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Saves;
using static ArchipelagoSurvivors.Core;
using CharacterController = Il2CppVampireSurvivors.Objects.Characters.CharacterController;

namespace ArchipelagoSurvivors.Patches;

public class PlayerPatch
{
    [HarmonyPatch(typeof(CharacterController), "InitCharacter"),
     HarmonyPostfix]
    public static void Init(CharacterController __instance,
        CharacterType characterType, int playerIndex, bool dontGetCharacterDataForCurrentLevel)
    {
        // __instance.MaxWeaponCount = 1;
        // Log.Msg($"[{__instance.Magnet}] [{__instance.MaxAccessoryCount}] + [{__instance.MaxAccessoryBonus}]");

        // __instance.weaponSelection
        
        // StringBuilder sb = new();
        //
        // foreach (var weapon in PhaserSaveDataUtils.LoadSaveFiles().sealed)
        // {
        //     Log.Msg(weapon);
        // }
        //
        // Log.Msg(sb.ToString());

        // GM.Core.IsWeaponTypeAvailable();
        var banishedWeapons = GM.Core.LevelUpFactory.BanishedWeapons;
    }

    [HarmonyPatch(typeof(LevelUpFactory), "BanishedSealedWeapons"), HarmonyPrefix]
    public static void Test1(LevelUpFactory __instance)
    {
        Log.Msg($"Adding sealed weapons to banish list [{__instance.BanishedWeapons.Count}], no u");
    }
    
    [HarmonyPatch(typeof(LevelUpFactory), "BanishedSealedWeapons"), HarmonyPostfix]
    public static void Test2(LevelUpFactory __instance)
    {
        Log.Msg($"Added sealed weapons to banish list [{__instance.BanishedWeapons.Count}]");
        __instance.BanishedWeapons.Clear();
        __instance.ExcludedWeapons.Clear();

        foreach (var weapon in Enum.GetValues<WeaponType>())
        {
            __instance.BanishedWeapons.AddLast(weapon);
            __instance.ExcludedWeapons.AddLast(weapon);
        }
    }
}