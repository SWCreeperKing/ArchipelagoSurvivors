using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using ArchipelagoSurvivors.Patches;
using CreepyUtil.Archipelago;
using Il2CppNewtonsoft.Json;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using UnityEngine;
using static ArchipelagoSurvivors.Core;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.PlayerPatch;
using static ArchipelagoSurvivors.Patches.SurvivorScreenPatch;

namespace ArchipelagoSurvivors;

public enum GoalRequirement
{
    StageHunt,
    KillTheDirector
}

public static class APSurvivorClient
{
    private static List<long> ChecksToSend = [];
    public static ConcurrentQueue<long> ChecksToSendQueue = [];
    public static ApClient? Client;
    public static CharacterType StartingCharacter;
    public static StageType StartingStage;
    public static StageType[] StagesToBeat;
    public static bool IsHyperLocked = false;
    public static bool IsHurryLocked = false;
    public static bool IsArcanasLocked = false;
    public static bool IsEggesLocked = false;
    public static bool EnemysanityEnabled = false;
    public static long ChestCheckAmount;
    public static GoalRequirement GoalRequirement;
    public static double DeathlinkCooldown;

    private static double NextSend = 4;
    private static bool _Deathlink;

    public static bool DeathLink => _Deathlink;

    public static string[]? TryConnect(int port, string slot, string address, string password)
    {
        _Deathlink = false;
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
            ChestCheckAmount = (long)slotdata["chest_checks_per_stage"];

            AllowedStages.Add(StartingStage);
            AllowedCharacters.Add(StartingCharacter);

            CharactersBeaten = Client!.GetFromStorage<string[]>("characters_completed", def: [])
                                      .Select(s => CharacterNameToType[s])
                                      .ToList();
            StagesBeaten = Client!.GetFromStorage<string[]>("levels_completed", def: [])
                                  .Select(s => StageNameToType[s])
                                  .ToList();

            EnemysanityEnabled = slotdata.TryGetValue("enemysanity", out var enemysanity) && (bool)enemysanity;
            GoalRequirement = (GoalRequirement)(slotdata.TryGetValue("goal_requirement", out var goalrequirement)
                ? (long)goalrequirement
                : 0);

            foreach (var stage in StagesBeaten)
            {
                AddLocationToQueue($"{StageTypeToName[stage]} Beaten");
            }

            Client!.Session!.Socket.PacketReceived += jsonPacket =>
            {
                if (jsonPacket is not BouncedPacket packet) return;
                if (!packet.Tags.Contains("DeathLink")) return;
                if (GM.Core?.Player is null) return;

                var person = packet.Data.TryGetValue("source", out var source) ? source.ToString() : "Unknown";
                if (person == Client.PlayerName) return;

                Log.Msg(
                    $"Received Deathlink from [{person}] for \n[{(packet.Data.TryGetValue("source", out var cause) ? cause : "Unknown Reason")}]");

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

                DeathlinkCooldown = 4;
                DeathIsQueued = true;
                GM.Core.Player.Kill();
            };
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        Log.Msg("Connnected");
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
        if (DeathlinkCooldown > 0) DeathlinkCooldown -= Time.deltaTime;
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

            AllowedCharacters.AddRange(newItems.Where(s => s.StartsWith("Character Unlock: "))
                                               .Select(s =>
                                                    CharacterNameToType[s[18..]]));

            AllowedStages.AddRange(newItems.Where(s => s.StartsWith("Stage Unlock: "))
                                           .Select(s =>
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
        }

        while (!ChecksToSendQueue.IsEmpty)
        {
            ChecksToSendQueue.TryDequeue(out var location);
            ChecksToSend.Add(location);
        }
    }

    private static void SendChecks()
    {
        NextSend = 3;
        Client?.SendLocations(ChecksToSend.ToArray());
        ChecksToSend.Clear();
    }

    public static void AddLocationToQueue(string locationName)
    {
        if (Client is null) return;
        if (Client.MissingLocations.All(kv => kv.Value.LocationName != locationName)) return;
        var location = Client.MissingLocations.First(kv => kv.Value.LocationName == locationName).Key;
        if (ChecksToSendQueue.Contains(location)) return;
        ChecksToSendQueue.Enqueue(location);
        Log.Msg($"Send check: [{locationName}]");
    }

    public static void ToggleDeathlink()
    {
        if (Client?.Session is null) return;
        if (_Deathlink)
        {
            Client.Session.ConnectionInfo.UpdateConnectionOptions([]);
            _Deathlink = false;
        }
        else
        {
            Client.Session.ConnectionInfo.UpdateConnectionOptions(["DeathLink"]);
            _Deathlink = true;
        }
    }
}