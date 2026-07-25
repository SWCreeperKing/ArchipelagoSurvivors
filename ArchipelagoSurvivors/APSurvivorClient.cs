using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using UnityEngine;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.Patches.PlayerPatch;
using static ArchipelagoSurvivors.Patches.SurvivorScreenPatch;

namespace ArchipelagoSurvivors;

public enum GoalRequirement { StageHunt, KillTheDirector }

internal static class APSurvivorClient
{
    public const int DeathlinkCooldownTimer = 10;

    private static List<string> SentAlready = [];
    private static HashSet<string> ChecksToSend = [];
    public static ConcurrentQueue<string> ChecksToSendQueue = [];
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static CharacterType StartingCharacter;
    public static StageType StartingStage;
    public static StageType[] StagesToBeat;
    public static bool IsHyperLocked = false;
    public static bool IsHurryLocked = false;
    public static bool IsArcanasLocked = false;
    public static bool IsEggesLocked = false;
    public static bool EnemysanityEnabled = false;
    public static long ChestCheckAmount;
    public static double DeathlinkCooldown;
    public static long StagesToBeatForDirector = 0;

    private static double NextSend = 4;

    public static void Init()
    {
        Client.ItemHandlerInitialized += handler =>
        {
            handler.OnNewItemsReceived += (items, _) =>
            {
                if (!items.Any()) return;
                var updateCompatibilityTxt = false;
                var newItems = items
                              .Select(item => item?.ItemName!)
                              .ToArray();

                AllowedCharacters.AddRange(
                    GetCompatibilityConversion(
                        "character",
                        newItems.Where(s => s.StartsWith("Character Unlock: ")).Select(s => s[18..]).ToArray(),
                        CharacterNameToType, ref updateCompatibilityTxt
                    )
                );

                AllowedStages.AddRange(
                    GetCompatibilityConversion(
                        "stage",
                        newItems.Where(s => s.StartsWith("Stage Unlock: ")).Select(s => s[14..]).ToArray(),
                        StageNameToType, ref updateCompatibilityTxt
                    )
                );

                if (updateCompatibilityTxt)
                {
                    File.WriteAllLines(
                        $"{DataFolder}/Compatibility.txt",
                        CompatibilityConversions.Select(kv => $"{kv.Key} = {kv.Value}")
                    );
                }

                foreach (var gamemode in newItems.Where(s => s.StartsWith("Gamemode Unlock: "))
                                                 .Select(s => s[17..]))
                {
                    switch (gamemode)
                    {
                        case "Hurry": IsHurryLocked = false; break;
                        case "Hyper": IsHyperLocked = false; break;
                        case "Arcanas": IsArcanasLocked = false; break;
                        case "Eggs": IsEggesLocked = false; break;
                    }
                }
            };
        };
        
        Client.OnConnectionEvent += _ =>
        {
            try
            {
                var slotdata = Client.SlotData!;
                StartingStage = StageNameToType[(string)slotdata["starting_stage"]];
                StartingCharacter = CharacterNameToType[(string)slotdata["starting_character"]];

                StagesToBeat = ((string)slotdata["stages_to_beat"]).Split(',')
                                                                   .Select(s => s.Trim('\'', '[', ']', ' ', '"'))
                                                                   .Select(s => StageNameToType[s])
                                                                   .ToArray();

                IsHyperLocked = (bool)slotdata[slotdata.ContainsKey("is_hyper_locked") ? "is_hyper_locked" : "lock_hyper_behind_item"];
                IsHurryLocked = (bool)slotdata[slotdata.ContainsKey("is_hurry_locked") ? "is_hurry_locked" : "lock_hurry_behind_item"];
                IsArcanasLocked = (bool)slotdata[slotdata.ContainsKey("is_arcanas_locked") ? "is_arcanas_locked" : "lock_arcanas_behind_item"];
                IsEggesLocked = (long)slotdata["egg_inclusion"] != 2;
                ChestCheckAmount = (long)slotdata["chest_checks_per_stage"];

                AllowedStages.Add(StartingStage);
                AllowedCharacters.Add(StartingCharacter);

                CharactersBeaten = Client.GetFromStorage<string[]>("characters_completed", def: [])
                                          .Select(s => CharacterNameToType[s])
                                          .ToList();
                
                StagesBeaten = Client.GetFromStorage<string[]>("levels_completed", def: [])
                                      .Select(s => StageNameToType[s])
                                      .ToList();

                EnemysanityEnabled = slotdata.TryGetValue("enemysanity", out var enemysanity) && (bool)enemysanity;
                Client.SetGoalType((GoalRequirement)(slotdata.TryGetValue("goal_requirement", out var goalrequirement)
                    ? (long)goalrequirement : 0));

                StagesToBeatForDirector = slotdata.TryGetValue("ending_stage_count", out var goalstagerequirement)
                    ? (long)goalstagerequirement
                    : 0;

                Log.Msg($"""
                         StartingStage: [{StartingStage}]
                         StartingCharacter: [{StartingCharacter}]
                         StagesToBeat: [{StagesToBeat.Length}]
                         IsHyperLocked: [{IsHyperLocked}]
                         IsHurryLocked: [{IsHurryLocked}]
                         IsArcanasLocked: [{IsArcanasLocked}]
                         IsEggesLocked: [{IsEggesLocked}]
                         ChestCheckAmount: [{ChestCheckAmount}]
                         CharactersBeaten: [{CharactersBeaten.Count}]
                         StagesBeaten: [{StagesBeaten.Count}]
                         EnemysanityEnabled: [{EnemysanityEnabled}]
                         GoalRequirement: [{Client.GetGoalTypeAsEnum<GoalRequirement>()}]
                         StagesToBeatForDirector: [{StagesToBeatForDirector}]
                         """);

                if (StagesToBeat.Length > StagesBeaten.Count
                    && Client.GetGoalTypeAsEnum<GoalRequirement>() is GoalRequirement.StageHunt)
                {
                    Log.Msg(
                        $"Stages left to beat: \n - {string.Join("\n - ", StagesToBeat.Except(StagesBeaten).Select(t => StageTypeToName[t]))}");
                }

                foreach (var stage in StagesBeaten) { AddLocationToQueue($"{StageTypeToName[stage]} Beaten"); }

            }
            catch (Exception e) { Log.Error(e); }

            Log.Msg("Connected");
        };
        
        Client.OnDeathLinkPacketReceived += (group, source, cause) =>
        {
            if (GM.Core?.Player is null) return;

            if (source == Client?.PlayerName) return;

            Log.Msg($"Received Deathlink from [{source}] for \n[{cause}]");

            if (GM.Core.IsPaused)
            {
                Log.Msg("Deathlink was parried by pause (DON'T ABUSE)");
                return;
            }

            if (DeathlinkCooldown > 0)
            {
                Log.Msg("Deathlink on cooldown");
                return;
            }

            DeathlinkCooldown = DeathlinkCooldownTimer;
            DeathIsQueued = true;
            GM.Core.Player.Kill();
        };
        
        Client.ItemsSentNotification += str => Log.Msg(ConsoleColor.DarkGray, $"Check Sent: [{str}]");
    }

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        try
        {
            SentAlready.Clear();
            Log.Msg($"Attempting to connect [{address}]:[{port}] [{password}] [{slot}]");

            var connectError = Client.TryConnect(new LoginInfo(port, slot, address, password), "Vampire Survivors",
                ItemsHandlingFlags.AllItems, requestSlotData: true);

            if (connectError is not null && connectError.Length > 0)
            {
                Log.Msg("There was an Error");
                Disconnect();
                return connectError;
            }
        }
        catch (Exception e)
        {
            Log.Msg("There was an Error");
            Disconnect();
            return [e.Message, e.StackTrace!];
        }

        return null;
    }

    public static void Disconnect()
    {
        Client.TryDisconnect();
        Log.Msg("Disconnected");
    }

    public static void Update()
    {
        if (Client is null) return;
        Client.UpdateConnection();
        if (!Client.IsConnected) return;

        NextSend -= Time.deltaTime;
        if (DeathlinkCooldown > 0) DeathlinkCooldown -= Time.deltaTime;
        if (ChecksToSend.Any() && NextSend <= 0) { SendChecks(); }

        Client.UpdateItemHandler();
        
        while (!ChecksToSendQueue.IsEmpty)
        {
            ChecksToSendQueue.TryDequeue(out var location);
            ChecksToSend.Add(location);
        }

        return;

    }

    private static T[] GetCompatibilityConversion<T>(string kind, string[] potentialItems, Dictionary<string, T> converter, ref bool updateCompatibilityTxt)
    {
        List<T> newItems = [];

        foreach (var item in potentialItems)
        {
            if (converter.TryGetValue(item, out var value1))
            {
                newItems.Add(value1);
                continue;
            }

            if (CompatibilityConversions.TryGetValue(item, out var value2) && value2 is not "")
            {
                if (converter.TryGetValue(value2, out var value3))
                {
                    newItems.Add(value3);
                    continue;
                }

                Log.Error($"Value [{value2}] is an incorrect {kind} name");
            }

            Log.Error(
                $"Value [{item}] is an incompatible {kind} name, goto [Vampire Survivors/Mods/SW_CreeperKing.ArchipelagoSurvivors/Data/Compatibility.txt] to fill out the correct name (restart the game to apply)");
            CompatibilityConversions[item] = "";
            updateCompatibilityTxt = true;
        }

        return newItems.ToArray();
    }

    private static void SendChecks()
    {
        NextSend = 3;
        var toSend = ChecksToSend.Where(check => !SentAlready.Contains(check)).ToArray();
        SentAlready.AddRange(toSend);
        if (toSend.Length > 0)
        {
            Client?.SendLocations(toSend);
            Log.Msg($"Checks Sent: [{string.Join(", ", toSend)}]");
        }
        ChecksToSend.Clear();
    }

    public static void AddLocationToQueue(string locationName)
    {
        if (Client is null) return;
        if (ChecksToSendQueue.Contains(locationName) || ChecksToSend.Contains(locationName)) return;
        ChecksToSendQueue.Enqueue(locationName);
    }
}