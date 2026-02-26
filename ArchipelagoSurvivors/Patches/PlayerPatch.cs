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
using static CreepyUtil.Archipelago.ArchipelagoTag;
using CharacterController = Il2CppVampireSurvivors.Objects.Characters.CharacterController;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
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

    [HarmonyPatch(typeof(CharacterController), "InitCharacter"),
     HarmonyPostfix]
    public static void Init(CharacterController __instance,
        CharacterType characterType, int playerIndex, bool dontGetCharacterDataForCurrentLevel)
    {
        DeathIsQueued = false;
    }

    [HarmonyPatch(typeof(CharacterController), "OnDeath"), HarmonyPostfix]
    public static void OnDeath(CharacterController __instance)
    {
        if (DeathIsQueued || DeathlinkCooldown > 0)
        {
            DeathIsQueued = false;
            return;
        }

        if (!Client!.Tags[DeathLink]) return;
        DeathlinkCooldown = 4;
        Client?.SendDeathLink(DeathlinkMessages[Random.Shared.Next(DeathlinkMessages.Length)]);
    }
}