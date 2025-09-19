using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.UI;
using UnityEngine;
using UnityEngine.UI;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.PlayerPatch;
using Il2CppGeneric = Il2CppSystem.Collections.Generic;
using Object = UnityEngine.Object;

namespace ArchipelagoSurvivors.Patches;

public static class SurvivorScreenPatch
{
    public static List<CharacterType> AllowedCharacters = [];
    public static List<StageType> AllowedStages = [];

    public static TickBoxController EggController;
    public static TickBoxController HyperController;
    public static TickBoxController HurryController;

    public static TickBoxController ArcaneController;

    private static GameObject StageSelectButton;
    private static int LastBeaten = -1;

    [HarmonyPatch(typeof(CharacterSelectionPage), "Start"), HarmonyPostfix]
    public static void OverrideCharacterStart(CharacterSelectionPage __instance)
    {
        EggController = __instance.GetChild(5).GetChild(0);
        EggController.VariableTracker = "eggs";
    }

    [HarmonyPatch(typeof(CharacterSelectionPage), "Update"), HarmonyPostfix]
    public static void OverrideCharacter(CharacterSelectionPage __instance)
    {
        if (ChestPickupPatch.ChestsOpened != 0)
        {
            ChestPickupPatch.ChestsOpened = 0;
        }

        if (LastMinuteCheck != -1)
        {
            LastMinuteCheck = -1;
        }

        EggController.Update();

        foreach (var (character, ui) in __instance._characterItemUIs)
        {
            ui.gameObject.SetActive(AllowedCharacters.Contains(character));
            ui._LockIcon.enabled = !IsHurryLocked && !CharactersBeaten.Contains(character);
        }
    }

    [HarmonyPatch(typeof(StageSelectPage), "Start"), HarmonyPostfix]
    public static void OverrideMapStart(StageSelectPage __instance)
    {
        LastBeaten = -1;
        var infoBox = __instance.GetChild(0).GetChild(2).GetChild(0).GetChildren();
        HyperController = infoBox[2];
        HyperController.VariableTracker = "hyper";
        HurryController = infoBox[3];
        HurryController.VariableTracker = "hurry";
        ArcaneController = infoBox[4];
        ArcaneController.VariableTracker = "arcanas";
        StageSelectButton = __instance.GetChild(2);
    }

    [HarmonyPatch(typeof(StageSelectPage), "Update"), HarmonyPostfix]
    public static void OverrideMap(StageSelectPage __instance)
    {
        HyperController.Update();
        HurryController.Update();
        ArcaneController.Update();

        var missingText = Client!.MissingLocations.Select(loc => Client!.DataLookup.Locations[loc]).ToArray();
        foreach (var stage in __instance.GetChild(0)
                                        .GetChild(1)
                                        .GetChild(0)
                                        .GetChild(0)
                                        .GetComponentsInChildren<StageItemUI>())
        {
            var hasBeaten = !StagesBeaten.Contains(stage.Type);
            var stageName = StageTypeToName[stage.Type];

            stage.gameObject.SetActive(AllowedStages.Contains(stage.Type));
            stage._Exclamation.SetActive(!IsHurryLocked && hasBeaten);
            
            if (stage.Type == StageType.MACHINE)
            {
                if (APSurvivorClient.GoalRequirement == GoalRequirement.KillTheDirector)
                {
                    stage.DescriptionText.text = $"Stages beaten requirement to open:\n[{StagesBeaten.Count}] of [{StagesToBeatForDirector}]";
                }
            }
            else
            {
                var chestChecksMissing = missingText.Where(s => !s.Contains("Beaten") && s.Contains(stageName))
                                                    .Select(s => s.Replace($" on {stageName}", ""))
                                                    .ToArray();
                
                var enemyChecksMissing = missingText.Where(s => s.Contains("Kill"))
                                                    .Select(s => EnemyNameToType[s[5..]])
                                                    .Where(et => EnemyStages[et].Contains(stage.Type))
                                                    .Select(et => EnemyTypeToName[et])
                                                    .ToArray();
                
                if (chestChecksMissing.Any())
                {
                    stage.DescriptionText.text = $"Chest Checks Remaining:\n{string.Join(", ", chestChecksMissing)}";
                }
                else if (enemyChecksMissing.Any())
                {
                    stage.DescriptionText.text = $"Enemy Checks Remaining:\n{string.Join(", ", enemyChecksMissing)}";
                }
                else
                {
                    stage.DescriptionText.text = "All Chest and Enemy Checks Got!";
                }
            }
            
            if (stage.Type != StageType.MACHINE ||
                APSurvivorClient.GoalRequirement != GoalRequirement.KillTheDirector) continue;

            if (__instance._selectedStage == stage)
            {
                StageSelectButton.SetActive(StagesBeaten.Count >= StagesToBeatForDirector);
            }
        }
    }
}

public class TickBoxController
{
    public string VariableTracker;

    public bool Enabled
        => VariableTracker switch
        {
            "hurry" => !IsHurryLocked,
            "hyper" => !IsHyperLocked,
            "arcanas" => !IsArcanasLocked,
            "eggs" => !IsEggesLocked,
            _ => false
        };

    private TickBoxUI Box = null;
    private GameObject Gobj = null;

    public TickBoxController(GameObject gobj)
    {
        Gobj = gobj;
        Box = Gobj.GetComponent<TickBoxUI>();
    }

    public void Update()
    {
        if (!Enabled && Box.isOn)
        {
            Box.isOn = false;
        }

        if (Enabled != Gobj.activeSelf)
        {
            Gobj.SetActive(Enabled);
        }
    }

    public static implicit operator TickBoxController(GameObject gobj) => new(gobj);
}