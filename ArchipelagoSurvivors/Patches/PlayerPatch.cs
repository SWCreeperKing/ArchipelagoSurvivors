using HarmonyLib;
using Il2CppSystem.Text;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Saves;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.InformationTransformer;
using CharacterController = Il2CppVampireSurvivors.Objects.Characters.CharacterController;

namespace ArchipelagoSurvivors.Patches;

public class PlayerPatch
{
    public static List<StageType> StagesBeaten = [];
    public static List<CharacterType> CharactersBeaten = [];
    
    [HarmonyPatch(typeof(CharacterController), "OnUpdate"), HarmonyPostfix]
    public static void OnUpdate(CharacterController __instance)
    {
        // Log.Msg($"[{GM.Core.Stage.CurrentMinute}] [{GM.Core.Stage._maxStageDataMinute}]");

        if (GM.Core.Stage.CurrentMinute < GM.Core.Stage._maxStageDataMinute) return;
        if (Client is null) return;
        
        if (!CharactersBeaten.Contains(__instance.CharacterType))
        {
            AddLocationToQueue($"Beat with {CharacterTypeToName[__instance.CharacterType]}");
            CharactersBeaten.Add(__instance.CharacterType);
        }

        if (!StagesBeaten.Contains(GM.Core.Stage.StageType))
        {
            AddLocationToQueue($"{GM.Core.Stage.StageType} Beaten");
            StagesBeaten.Add(GM.Core.Stage.StageType);
            Client.SendToStorage("levels_completed", StagesBeaten.Select(st => StageTypeToName[st]).ToArray());
            if (StagesBeaten.Count != StagesToBeat.Length || Client.HasGoaled) return;
            Client.Goal();
        }
    }
    
    // [HarmonyPatch(typeof(CharacterController), "InitCharacter"),
    //  HarmonyPostfix]
    // public static void Init(CharacterController __instance,
    //     CharacterType characterType, int playerIndex, bool dontGetCharacterDataForCurrentLevel)
    // {
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
        // var banishedWeapons = GM.Core.LevelUpFactory.BanishedWeapons;
    // }

    // [HarmonyPatch(typeof(LevelUpFactory), "BanishedSealedWeapons"), HarmonyPrefix]
    // public static void Test1(LevelUpFactory __instance)
    // {
    //     Log.Msg($"Adding sealed weapons to banish list [{__instance.BanishedWeapons.Count}], no u");
    // }
    //
    // [HarmonyPatch(typeof(LevelUpFactory), "BanishedSealedWeapons"), HarmonyPostfix]
    // public static void Test2(LevelUpFactory __instance)
    // {
    //     Log.Msg($"Added sealed weapons to banish list [{__instance.BanishedWeapons.Count}]");
    //     __instance.BanishedWeapons.Clear();
    //     __instance.ExcludedWeapons.Clear();
    //
    //     foreach (var weapon in Enum.GetValues<WeaponType>())
    //     {
    //         __instance.BanishedWeapons.AddLast(weapon);
    //         __instance.ExcludedWeapons.AddLast(weapon);
    //     }
    // }
}