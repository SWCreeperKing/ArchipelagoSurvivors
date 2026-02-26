using ArchipelagoSurvivors.Patches;
using CreepyUtil.Archipelago.ApClient;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppVampireSurvivors.Data;
using MelonLoader;
using UnityEngine;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.EnemyCounterPatch;

[assembly: MelonInfo(typeof(ArchipelagoSurvivors.Core), "ArchipelagoSurvivors", "0.0.1", "SW_CreeperKing", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: MelonOptionalDependencies("SurvivorModMenu")] // https://github.com/takacomic/SurvivorModMenu

namespace ArchipelagoSurvivors;

public class Core : MelonMod
{
    public static MelonLogger.Instance Log;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        ParseEnemyVariantTypes();
        ParseCSV();
        ParseEnemyStageTypes();
        ParseEnemyHurryStageTypes();
        ParseBlacklistedEnemyTypes();

        Log.Msg("Converting [EnemyVariantTypes] to [EnemyVariantListings]");
        foreach (var (enemy, variants) in EnemyVariantTypes)
        {
            foreach (var variant in variants)
            {
                if (enemy == variant) continue;
                EnemyVariantListings[variant] = enemy;
            }
        }

        // foreach (var (enemy, stages) in EnemyStages)
        // {
        //     foreach (var stage in stages)
        //     {
        //         if (!StagesEnemies.ContainsKey(stage)) StagesEnemies[stage] = [];
        //         StagesEnemies[stage].Add(enemy);
        //     }
        // }

        StageNameToType = new Dictionary<string, StageType>();

        foreach (var kv in StageTypeToName)
        {
            StageNameToType[kv.Value] = kv.Key;
        }
        

        var classesToPatch = MelonAssembly.Assembly.GetTypes()
                                          .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                          .ToArray();

        Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch)
        {
            HarmonyInstance.PatchAll(patch);

            Log.Msg($"Loaded: [{patch.Name}]");
        }
        
        LoggerInstance.Msg("Initialized.");
    }

    public override void OnUpdate() => APSurvivorClient.Update();

    public void ParseCSV()
    {
        if (!File.Exists("Enemies - Sheet1.csv")) return;
        Log.Msg("Parsing CSV");
        var text = File.ReadAllText("Enemies - Sheet1.csv");
        var lines = text
                   .Replace("Capella Magna", "Cappella Magna")
                   .Replace("Bone Zone", "The Bone Zone")
                   .Replace("\r", "")
                   .Split('\n')
                   .Skip(1);
        var cells = lines.Select(line => line.Split(',')).ToArray();
        var notvalid = false;

        var enemyStageProcessList =
            cells.Where(arr =>
                  {
                      var has = EnemyNameToType.ContainsKey(arr[0]);
                      if (!has) Log.Error($"CSV NAME: [{arr[0]}] DOES NOT EXIST");
                      return has;
                  })
                 .Select(arr => new EnemySheetData(EnemyNameToType[arr[0]], arr[1],
                      StageNameToType[arr[2]], arr[3] == "Yes"))
                 .Where(data =>
                  {
                      if (data.Frequency is null) return false;
                      if (double.TryParse(data.Frequency, out var numbFreq))
                      {
                          return numbFreq > 10;
                      }

                      if (data.Frequency.Contains("Arcana") ||
                          data.Frequency.Contains("Obscure Event") ||
                          data.Frequency.Contains("Trap") ||
                          data.Frequency.Contains("Room Based") ||
                          data.Frequency.Contains("Ignores Walls"))
                      {
                          return false;
                      }

                      if (CharacterNameToType.ContainsKey(data.Frequency) ||
                          data.Frequency.StartsWith("Event") ||
                          data.Frequency.StartsWith("Boss") || data.Frequency.StartsWith("N/A"))
                      {
                          return true;
                      }

                      notvalid = true;
                      Log.Msg($"Unchecked enemystageprocesslist: [{data.Frequency}]");
                      return false;
                  })
                 .ToArray();

        Dictionary<EnemyType, HashSet<StageType>> enemyListOut = new();
        Dictionary<EnemyType, HashSet<StageType>> enemyHurryOnlyListOut = new();

        foreach (var enemyStageData in enemyStageProcessList)
        {
            if (!enemyListOut.ContainsKey(enemyStageData.EnemyType))
            {
                enemyListOut.Add(enemyStageData.EnemyType, []);
            }

            enemyListOut[enemyStageData.EnemyType].Add(enemyStageData.StageType);
        }

        foreach (var enemyStageData in enemyStageProcessList)
        {
            if (!enemyStageData.RequireHurry) continue;

            if (!enemyHurryOnlyListOut.ContainsKey(enemyStageData.EnemyType))
            {
                enemyHurryOnlyListOut.Add(enemyStageData.EnemyType, []);
            }

            enemyHurryOnlyListOut[enemyStageData.EnemyType].Add(enemyStageData.StageType);
        }

        foreach (var enemyStageData in enemyStageProcessList)
        {
            if (enemyStageData.RequireHurry) continue;
            if (!enemyHurryOnlyListOut.TryGetValue(enemyStageData.EnemyType, out var hashset)) continue;
            if (!hashset.Contains(enemyStageData.StageType)) continue;
            hashset.Remove(enemyStageData.StageType);
        }

        enemyHurryOnlyListOut =
            enemyHurryOnlyListOut.Where(kv => kv.Value.Any()).ToDictionary(kv => kv.Key, kv => kv.Value);
        
        Log.Msg("CSV parsed, writing out parsed data");

        File.WriteAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy List.txt",
            string.Join('\n',
                enemyListOut.Select(kv
                    => $"{EnemyTypeToName[kv.Key]}: {string.Join(',', kv.Value.Select(stage => StageTypeToName[stage]))}")));

        File.WriteAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy Hurry List.txt",
            string.Join('\n',
                enemyHurryOnlyListOut.Select(kv
                    => $"{EnemyTypeToName[kv.Key]}: {string.Join(',', kv.Value.Select(stage => StageTypeToName[stage]))}")));

        File.WriteAllText("EnemyList.py",
            $"enemy_map = {{\n{string.Join(",\n", enemyListOut.Select(kv => $"\t\"{EnemyTypeToName[kv.Key]}\": [{string.Join(',', kv.Value.Select(stage => $"\"{StageTypeToName[stage]}\""))}]"))}\n}}");

        File.WriteAllText("EnemyHurryList.py",
            $"enemy_hurry_map = {{\n{string.Join(",\n", enemyHurryOnlyListOut.Select(kv => $"\t\"{EnemyTypeToName[kv.Key]}\": [{string.Join(',', kv.Value.Select(stage => $"\"{StageTypeToName[stage]}\""))}]"))}\n}}");

        if (notvalid) return;
        File.Delete("Enemies - Sheet1.csv");
    }

    public void ParseEnemyStageTypes()
    {
        Log.Msg("Reading and parsing [EnemyStages] list");
        EnemyStages = File.ReadAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy List.txt")
                          .Replace("\r", "")
                          .Replace("\"", "")
                          .Replace("[", "")
                          .Replace("]", "")
                          .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s =>
                           {
                               var split = s.Split(": ");
                               return (EnemyNameToType[split[0].Trim()],
                                   split[1]
                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => StageNameToType[t.Trim()])
                                      .ToArray());
                           })
                          .ToDictionary(t => t.Item1, t => t.Item2);
        Log.Msg($"Parsed [EnemyStages] list, Element Count: [{EnemyStages.Count}]");
    }
    public void ParseEnemyHurryStageTypes()
    {
        Log.Msg("Reading and parsing [EnemyHurryStages] list");
        EnemyHurryStages = File.ReadAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy Hurry List.txt")
                          .Replace("\r", "")
                          .Replace("\"", "")
                          .Replace("[", "")
                          .Replace("]", "")
                          .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s =>
                           {
                               var split = s.Split(": ");
                               return (EnemyNameToType[split[0].Trim()],
                                   split[1]
                                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => StageNameToType[t.Trim()])
                                      .ToArray());
                           })
                          .ToDictionary(t => t.Item1, t => t.Item2);
        Log.Msg($"Parsed [EnemyHurryStages] list, Element Count: [{EnemyHurryStages.Count}]");
    }

    public void ParseEnemyVariantTypes()
    {
        Log.Msg("Reading and parsing [EnemyVariantTypes] list");
        EnemyVariantTypes = File.ReadAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy Variants.txt")
                                .Replace("\r", "")
                                .Replace("\"", "")
                                .Replace("[", "")
                                .Replace("]", "")
                                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s =>
                                 {
                                     var split = s.Split(": ");
                                     return (EnemyNameToType[split[0].Trim()],
                                         split[1]
                                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(ss => Enum.Parse<EnemyType>(ss.Trim(), true))
                                            .ToArray());
                                 })
                                .ToDictionary(t => t.Item1, t => t.Item2);
        
        Log.Msg($"Parsed [EnemyVariantTypes] list, Element Count: [{EnemyNameToType.Count}]");
    }

    public void ParseBlacklistedEnemyTypes()
    {
        Log.Msg("Reading and parsing [BlacklistedEnemyTypes] list");
        EnemyTypes = File.ReadAllText("Mods/SW_CreeperKing.ArchipelagoSurvivors/Enemy Blacklist.txt")
                         .Replace("\r", "")
                         .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => Enum.Parse<EnemyType>(s.Trim()))
                         .ToList();
        
        Log.Msg($"Parsed [BlacklistedEnemyTypes] list, Element Count: [{EnemyTypes.Count}]");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                
                break;
        }
    }
}

public readonly struct EnemySheetData(EnemyType enemyType, string frequency, StageType stageType, bool requireHurry)
{
    public readonly EnemyType EnemyType = enemyType;
    public readonly string Frequency = frequency;
    public readonly StageType StageType = stageType;
    public readonly bool RequireHurry = requireHurry;
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;