using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Stage;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.PlayerPatch;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class StageEndPatch
{
    // [HarmonyPatch(typeof(StageEventManager), "PlayCycleComplete"), HarmonyPostfix]
    [HarmonyPatch(typeof(StageEventManager), "TriggerEvent"), HarmonyPostfix]
    public static void Win(Event stageDataEvent)
    {
        var rawEvent = Enum.Parse<StageEventType>(stageDataEvent.eventType);
        if (rawEvent is not StageEventType.CYCLE_COMPLETE) return;

        if (Client is null) return;
        if (IsHurryLocked)
        {
            Log.Msg(
                ConsoleColor.Yellow,
                "You do not have hurry unlocked so `Beat with [character]` and `[stage] beaten` checks are locked"
            );
            return;
        }

        Log.Msg($"Beat Stage: [{StageTypeToName[GM.Core.Stage.StageType]}]");

        var type = GM.Core.Player.CharacterType;
        if (!CharactersBeaten.Contains(type))
        {
            if (CharacterTypeToName.TryGetValue(type, out var value))
            {
                Log.Msg("beat with check");
                AddLocationToQueue($"Beat with {value}");
                CharactersBeaten.Add(type);
                Client.SendToStorage(
                    "characters_completed",
                    CharactersBeaten.Select(ct => CharacterTypeToName[ct]).ToArray()
                );
            }
        }

        if (!StagesBeaten.Contains(GM.Core.Stage.StageType))
        {
            Log.Msg("beaten check");
            AddLocationToQueue($"{StageTypeToName[GM.Core.Stage.StageType]} Beaten");
            StagesBeaten.Add(GM.Core.Stage.StageType);
            Client.SendToStorage("levels_completed", StagesBeaten.Select(st => StageTypeToName[st]).ToArray());
            if (StagesBeaten.Count != StagesToBeat.Length ||
                Client.HasGoaled || APSurvivorClient.GoalRequirement != GoalRequirement.StageHunt) return;
            Client.Goal();
        }
    }
}