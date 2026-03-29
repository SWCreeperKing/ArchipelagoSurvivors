using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Stage;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Stages;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.Patches.PlayerPatch;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class StageEndPatch
{
    [HarmonyPatch(typeof(StageEventManager), "TriggerEvent"), HarmonyPostfix]
    public static void Win(Event stageDataEvent)
    {
        try
        {
            var rawEvent = Enum.Parse<StageEventType>(stageDataEvent.eventType);
            if (rawEvent is not StageEventType.CYCLE_COMPLETE) return;

            BeatStageAndCharacter();
        }
        catch (Exception e) { Log.Error(e); }
    }

    [HarmonyPatch(typeof(DirecterManager), "StartPhase4"), HarmonyPrefix]
    public static void DirectorPhase4() => BeatStageAndCharacter(true);

    [HarmonyPatch(typeof(DirecterManager), "StartPhase5"), HarmonyPrefix]
    public static void DirectorPhase5() => BeatStageAndCharacter(true);

    public static void BeatStageAndCharacter(bool isDirector = false)
    {
        if (Client is null) return;
        if (IsHurryLocked)
        {
            Log.Msg(ConsoleColor.Yellow,
                "You do not have hurry unlocked so `Beat with [character]` and `[stage] beaten` checks are locked");
            return;
        }

        BeatStage();
        BeatCharacter();
        if (!isDirector) return;
        Client.SendLocation("Kill The Directer");
        Client.TryGoal(GoalRequirement.KillTheDirector);
    }

    public static void BeatCharacter()
    {
        var type = GM.Core.Player.CharacterType;
        if (CharactersBeaten.Contains(type)) return;
        if (!CharacterTypeToName.TryGetValue(type, out var value)) return;

        Log.Msg("beat with check");
        AddLocationToQueue($"Beat with {value}");
        CharactersBeaten.Add(type);
        Client?.SendToStorage("characters_completed",
            CharactersBeaten.Select(ct => CharacterTypeToName[ct]).ToArray());
    }

    public static void BeatStage()
    {
        Log.Msg($"Beat Stage: [{StageTypeToName[GM.Core.Stage.StageType]}]");

        if (StagesBeaten.Contains(GM.Core.Stage.StageType)) return;
        Log.Msg("beaten check");
        AddLocationToQueue($"{StageTypeToName[GM.Core.Stage.StageType]} Beaten");
        StagesBeaten.Add(GM.Core.Stage.StageType);
        Client!.SendToStorage("levels_completed", StagesBeaten.Select(st => StageTypeToName[st]).ToArray());

        if (StagesBeaten.Count != StagesToBeat.Length || Client.HasGoaled) return;
        Client.TryGoal(GoalRequirement.StageHunt);
    }
}