using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using Il2CppVampireSurvivors.Data;
using UnityEngine;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.PlayerPatch;
using static ArchipelagoSurvivors.Patches.SurvivorScreenPatch;

namespace ArchipelagoSurvivors;

public static class APSurvivorClient
{
    private static List<long> ChecksToSend = [];
    public static ConcurrentQueue<long> ChecksToSendQueue = [];
    public static ApClient? Client;
    private static double NextSend = 4;

    public static CharacterType StartingCharacter;
    public static StageType StartingStage;
    public static StageType[] StagesToBeat;
    public static bool IsHyperLocked = false;
    public static bool IsHurryLocked = false;
    public static bool IsArcanasLocked = false;
    public static bool IsEggesLocked = false;

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        try
        {
            Client = new ApClient();
            Log.Msg($"Attempting to connect [{address}]:[{port}] [{password}] [{slot}]");

            var connectError = Client.TryConnect(new LoginInfo(port, slot, address, password), 0x3AF4F1BC,
                "Vampire Survivors", ItemsHandlingFlags.AllItems, requestSlotData: true);

            if (connectError is not null && connectError.Length > 0)
            {
                Log.Msg("There was an Error");
                Disconnect();
                return connectError;
            }

            HasConnected();
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
        Client?.TryDisconnect();
        Client = null;
        Log.Msg("Disconnected");
    }

    public static void HasConnected()
    {
        try
        {
            var slotdata = Client?.SlotData!;
            StartingStage = StageNameToType[(string)slotdata["starting_stage"]];
            StartingCharacter = CharacterNameToType[(string)slotdata["starting_character"]];

            StagesToBeat = ((string)slotdata["stages_to_beat"]).Split(',')
                                                               .Select(s => s.Trim('\'', '[', ']', ' ', '"'))
                                                               .Select(s => StageNameToType[s])
                                                               .ToArray();

            IsHyperLocked = (bool)slotdata["is_hyper_locked"];
            IsHurryLocked = (bool)slotdata["is_hurry_locked"];
            IsArcanasLocked = (bool)slotdata["is_arcanas_locked"];
            IsEggesLocked = (long)slotdata["egg_inclusion"] != 2;
            
            AllowedStages.Add(StartingStage);
            AllowedCharacters.Add(StartingCharacter);

            StagesBeaten = Client!.GetFromStorage<string[]>("levels_completed", def: []).Select(s => StageNameToType[s]).ToList();
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
        // var startingLocation = (string)slotdata["starting_location"]!;
        // WinCondition = (long)slotdata["jobs_done"];
        // if (slotdata.TryGetValue("percentsanity", out var temp)) Percentsanity = (bool)temp;
        // if (slotdata.TryGetValue("objectsanity", out var temp1)) Objectsanity = (bool)temp1;
        //
        // if (slotdata.TryGetValue("goal_levels", out var temp3))
        // {
        //     Levels = ((string)temp3).Split(',').Select(s => s.Trim('\'', '[', ']', ' ', '"')).ToArray();
        // }
        //
        // if (slotdata.TryGetValue("goal_level_amount", out var temp4)) LevelCount = (long)temp4;
        // if (LevelCount == 0) LevelCount = Levels.Length;
        //
        // Goal = Levels.Any() && Levels[0] != "None" ? GoalType.LevelHunt : GoalType.McGuffinHunt;
        // // Plugin.Log.LogInfo($"[{Goal}] | [{string.Join(", ", Levels)}] | [{startingLocation}]");
        //
        // Allowed = [LevelUnlockDictionary[$"{startingLocation} Unlock"]];
        // Jobs = 0;

        Log.Msg("Connnected");
        // GoalLevelCheck(Client!.GetFromStorage<string[]>("levels_completed", def: [])!);
    }

    public static bool IsConnected()
    {
        return Client is not null && Client.IsConnected && Client.Session!.Socket.Connected;
    }

    public static void Update()
    {
        if (Client is null) return;
        Client.UpdateConnection();
        if (Client?.Session?.Socket is null || !Client.IsConnected) return;

        NextSend -= Time.deltaTime;
        if (ChecksToSend.Any() && NextSend <= 0)
        {
            SendChecks();
        }

        var rawNewItems = Client.GetOutstandingItems().ToArray();
        if (rawNewItems.Any())
        {
            var newItems = rawNewItems
                          .Select(item => item?.ItemName!)
                          .ToArray();
            
            AllowedCharacters.AddRange(newItems.Where(s => s.StartsWith("Character Unlock: ")).Select(s =>
                CharacterNameToType[s[18..]]));
            
            AllowedStages.AddRange(newItems.Where(s => s.StartsWith("Stage Unlock: ")).Select(s => 
                StageNameToType[s[14..]]));

            foreach (var gamemode in newItems.Where(s => s.StartsWith("Gamemode Unlock: ")).Select(s => s[17..]))
            {
                switch (gamemode)
                {
                    case "Hurry":
                        IsHurryLocked = false;
                        break;
                    case "Hyper":
                        IsHyperLocked = false;
                        break;
                    case "Arcanas":
                        IsArcanasLocked = false;
                        break;
                    case "Eggs":
                        IsEggesLocked = false;
                        break;
                }
            }
            
            // Jobs += newItems.Count(item => item == "A Job Well Done");
            // var newAllowed = newItems.Where(item => item.EndsWith(" Unlock"))
            //                          .Select(item => LevelUnlockDictionary[item])
            //                          .ToArray();
            //
            // if (newAllowed.Any())
            // {
            //     Allowed.AddRange(newAllowed);
            //     Plugin.Log.LogInfo($"Unlocked: [{string.Join(", ", newAllowed)}]");
            //     UpdateAvailableLevelGoal();
            // }
        }

        while (!ChecksToSendQueue.IsEmpty)
        {
            ChecksToSendQueue.TryDequeue(out var location);
            ChecksToSend.Add(location);
        }

        // if (Jobs < WinCondition) return;
        // Client.Goal();
    }

    private static void SendChecks()
    {
        NextSend = 3;
        Client?.SendLocations(ChecksToSend.ToArray());
        // Log.Msg($"Sent [{ChecksToSend.Count}] checks");
        ChecksToSend.Clear();
    }

    public static void AddLocationToQueue(string locationName)
    {
        // Log.Msg($"Try send check: [{locationName}]");
        if (Client is null) return;
        if (Client.MissingLocations.All(kv => kv.Value.LocationName != locationName)) return;
        var location = Client.MissingLocations.First(kv => kv.Value.LocationName == locationName).Key;
        if (ChecksToSendQueue.Contains(location)) return;
        ChecksToSendQueue.Enqueue(location);
        // Log.Msg($"Send check: [{locationName}]");
    }
}