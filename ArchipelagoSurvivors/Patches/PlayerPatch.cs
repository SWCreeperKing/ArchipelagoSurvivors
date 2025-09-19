using HarmonyLib;
using Il2CppSystem.Text;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Saves;
using Il2CppVampireSurvivors.Objects;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.GoalRequirement;
using static ArchipelagoSurvivors.InformationTransformer;
using CharacterController = Il2CppVampireSurvivors.Objects.Characters.CharacterController;

namespace ArchipelagoSurvivors.Patches;

public class PlayerPatch
{
    public static string[] DeathlinkMessages =
    [
        "Couldn't handle the pressure",
        "Didn't garlic enough",
        "Victory was in another coffin",
        "Ran out of Tirajisú",
        "Ran out of floor chicken",
        "Fell into death's hands",
    ];

    public static List<StageType> StagesBeaten = [];
    public static List<CharacterType> CharactersBeaten = [];
    public static int LastMinuteCheck = -1;
    public static bool DeathIsQueued;
    public static bool Warning;

    [HarmonyPatch(typeof(CharacterController), "OnUpdate"), HarmonyPostfix]
    public static void OnUpdate(CharacterController __instance)
    {
        var currentMinute = GM.Core.Stage.CurrentMinute;

        if (LastMinuteCheck == -1)
        {
            LastMinuteCheck = 0;
            Log.Msg($"stage loop time: [{GM.Core.Stage._maxStageDataMinute}]");
        }

        if (LastMinuteCheck <= currentMinute && GM.Core.Stage._maxStageDataMinute > currentMinute)
        {
            LastMinuteCheck = currentMinute;
            return;
        }

        if (Client is null) return;
        if (IsHurryLocked)
        {
            if (Warning) return;
            Warning = true;
            Log.Msg(ConsoleColor.Yellow,
                "You do not have hurry unlocked so `Beat with [character]` and `[stage] beaten` checks are locked");
            return;
        }

        if (!CharactersBeaten.Contains(__instance.CharacterType))
        {
            if (CharacterTypeToName.TryGetValue(__instance.CharacterType, out var value))
            {
                Log.Msg("beat with check");
                AddLocationToQueue($"Beat with {value}");
                CharactersBeaten.Add(__instance.CharacterType);
                Client.SendToStorage("characters_completed",
                    CharactersBeaten.Select(ct => CharacterTypeToName[ct]).ToArray());
            }
        }

        if (!StagesBeaten.Contains(GM.Core.Stage.StageType))
        {
            Log.Msg("beaten check");
            AddLocationToQueue($"{StageTypeToName[GM.Core.Stage.StageType]} Beaten");
            StagesBeaten.Add(GM.Core.Stage.StageType);
            Client.SendToStorage("levels_completed", StagesBeaten.Select(st => StageTypeToName[st]).ToArray());
            if (StagesBeaten.Count != StagesToBeat.Length ||
                Client.HasGoaled || APSurvivorClient.GoalRequirement != StageHunt) return;
            Client.Goal();
        }
    }

    [HarmonyPatch(typeof(CharacterController), "InitCharacter"),
     HarmonyPostfix]
    public static void Init(CharacterController __instance,
        CharacterType characterType, int playerIndex, bool dontGetCharacterDataForCurrentLevel)
    {
        DeathIsQueued = false;
        Warning = false;
    }

    [HarmonyPatch(typeof(CharacterController), "OnDeath"), HarmonyPostfix]
    public static void OnDeath(CharacterController __instance)
    {
        if (DeathIsQueued || DeathlinkCooldown > 0)
        {
            DeathIsQueued = false;
            return;
        }

        if (!DeathLink) return;
        DeathlinkCooldown = 4;
        Client?.SendDeathLink(DeathlinkMessages[Random.Shared.Next(DeathlinkMessages.Length)]);
    }
}