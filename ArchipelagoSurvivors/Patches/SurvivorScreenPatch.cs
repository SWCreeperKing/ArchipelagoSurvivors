using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.UI;
using UnityEngine;
using UnityEngine.UI;
using static ArchipelagoSurvivors.APSurvivorClient;
using static ArchipelagoSurvivors.Patches.PlayerPatch;
using Il2CppGeneric = Il2CppSystem.Collections.Generic;

namespace ArchipelagoSurvivors.Patches;

public static class SurvivorScreenPatch
{
    public static List<CharacterType> AllowedCharacters = [];
    public static List<StageType> AllowedStages = [];

    public static TickBoxController EggController;
    public static TickBoxController HyperController;
    public static TickBoxController HurryController;

    public static TickBoxController ArcaneController;

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
            ui.GetChild(3).GetComponent<Image>().enabled = !CharactersBeaten.Contains(character);
        }
    }

    [HarmonyPatch(typeof(StageSelectPage), "Start"), HarmonyPostfix]
    public static void OverrideMapStart(StageSelectPage __instance)
    {
        var infoBox = __instance.GetChild(0).GetChild(2).GetChild(0).GetChildren();
        HyperController = infoBox[2];
        HyperController.VariableTracker = "hyper";
        HurryController = infoBox[3];
        HurryController.VariableTracker = "hurry";
        ArcaneController = infoBox[4];
        ArcaneController.VariableTracker = "arcanas";
    }

    [HarmonyPatch(typeof(StageSelectPage), "Update"), HarmonyPostfix]
    public static void OverrideMap(StageSelectPage __instance)
    {
        HyperController.Update();
        HurryController.Update();
        ArcaneController.Update();
        
        foreach (var stage in __instance.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponentsInChildren<StageItemUI>())
        {
            stage.gameObject.SetActive(AllowedStages.Contains(stage.Type));
            stage.GetChild(5).SetActive(!StagesBeaten.Contains(stage.Type));
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