using ArchipelagoSurvivors.Patches;
using Il2CppVampireSurvivors.Data;
using MelonLoader;

[assembly: MelonInfo(typeof(ArchipelagoSurvivors.Core), "ArchipelagoSurvivors", "0.3.0", "SW_CreeperKing", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
// [assembly: MelonOptionalDependencies("SurvivorModMenu")] // https://github.com/takacomic/SurvivorModMenu

namespace ArchipelagoSurvivors;

public class Core : MelonMod
{
    public static int Debug = 0;
    public static MelonLogger.Instance Log;
    public const string DataFolder = "Mods/SW_CreeperKing.ArchipelagoSurvivors/Data";

    public static Dictionary<string, EnemyType> EnemyNameToType;
    public static Dictionary<EnemyType, string> EnemyTypeToName;
    public static Dictionary<string, StageType> StageNameToType;
    public static Dictionary<StageType, string> StageTypeToName;
    public static Dictionary<string, CharacterType> CharacterNameToType;
    public static Dictionary<CharacterType, string> CharacterTypeToName;
    public static Dictionary<EnemyType, EnemyType[]> EnemyVariants;
    public static Dictionary<EnemyType, EnemyType> EnemyVariantListings;
    public static Dictionary<EnemyType, StageType[]> EnemyStages;
    public static Dictionary<EnemyType, StageType[]> EnemyHurryStages;
    public static EnemyType[] EnemyArcanaList;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        if (File.Exists("debug.txt")) Debug = int.Parse(File.ReadAllText("debug.txt"));

        if (Debug > 0)
        {
            if (!File.Exists($"{DataFolder}/new_unnamed_enemy_data.txt")) return;
            MainMenuPatch.Unknown = File.ReadAllLines($"{DataFolder}/new_unnamed_enemy_data.txt")
                                        .Select(s => Enum.Parse<EnemyType>(s, true)).ToArray();
        }

        var classesToPatch = MelonAssembly.Assembly.GetTypes()
                                          .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                          .ToArray();

        Log.Msg("Reading External Data");

        EnemyNameToType = ReadFile<EnemyType>("EnemyData");
        EnemyTypeToName = EnemyNameToType.ToDictionary(kv => kv.Value, kv => kv.Key);
        Log.Msg("Read Enemy Data");

        StageNameToType = ReadFile<StageType>("StageData");
        StageTypeToName = StageNameToType.ToDictionary(kv => kv.Value, kv => kv.Key);
        Log.Msg("Read Stage Data");

        CharacterNameToType = ReadFile<CharacterType>("CharData");
        CharacterTypeToName = CharacterNameToType.ToDictionary(kv => kv.Value, kv => kv.Key);
        Log.Msg("Read Character Data");

        EnemyVariants = ReadEnemyFile<EnemyType>("EnemyVariants");
        Log.Msg("Read Variant Data");

        EnemyVariantListings = File
                              .ReadAllLines($"{DataFolder}/EnemyVariantMap.txt").Select(s => s.Split(':'))
                              .ToDictionary(arr => Enum.Parse<EnemyType>(arr[0]), arr => Enum.Parse<EnemyType>(arr[1]));
        Log.Msg("Read Variant Map");

        EnemyStages = ReadEnemyMapFile("EnemyMap");
        Log.Msg("Read Play by Play Data");
        EnemyHurryStages = ReadEnemyMapFile("EnemyHurryMap");
        Log.Msg("Read Play by Play (Hurry) Data");

        EnemyArcanaList = File.ReadAllLines($"{DataFolder}/ArcanaEnemyList.txt").Select(s => Enum.Parse<EnemyType>(s)).ToArray();
        Log.Msg("Read Arcana Enemy");

        Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch)
        {
            HarmonyInstance.PatchAll(patch);

            Log.Msg($"Loaded: [{patch.Name}]");
        }

        LoggerInstance.Msg("Initialized.");

        return;

        Dictionary<string, T> ReadFile<T>(string fileName) where T : struct
            => File
              .ReadAllLines($"{DataFolder}/{fileName}.txt").Select(s => s.Split(':'))
              .ToDictionary(arr => arr[0], arr => Enum.Parse<T>(arr[1]));

        Dictionary<EnemyType, T[]> ReadEnemyFile<T>(string fileName) where T : struct
            => File
              .ReadAllLines($"{DataFolder}/{fileName}.txt").Select(s => s.Split(':'))
              .ToDictionary(
                   arr => Enum.Parse<EnemyType>(arr[0]),
                   arr => arr[1].Split('|').Select(Enum.Parse<T>).ToArray()
               );

        Dictionary<EnemyType, StageType[]> ReadEnemyMapFile(string fileName)
            => File.ReadAllLines($"{DataFolder}/{fileName}.txt").Select(s => s.Split(':'))
                   .ToDictionary(
                        arr => EnemyNameToType[arr[0]],
                        arr => arr[1].Split('|').Select(s => StageNameToType[s]).ToArray()
                    );
    }

    public override void OnUpdate() => APSurvivorClient.Update();

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (Debug == 0) return;
        switch (sceneName)
        {
            case "MainMenu":

                MakeCsv(
                    "stageTable", StageDataPatch.FullStageData.Select(kv =>
                        {
                            var stage = kv.Value[0];
                            return (string[])
                            [
                                stage.StageName, $"{kv.Key}",
                                stage.DlcType is not null ? $"{stage.DlcType!.Value}" : "",
                            ];
                        }
                    ).Prepend(["Stage", "Id", "Dlc"]).ToArray()
                );

                MakeCsv(
                    "stageDataEnemyTable", StageDataPatch.FullStageData.Where(kv
                        => kv.Value.Length > 1 && kv.Value.Any(data => data.Enemies.Any() || data.Bosses.Any())
                    ).SelectMany(kv => kv.Value.SelectMany(data
                            => data.Enemies.Select(e => (string[])[$"{e}", $"{kv.Key}", $"{data.minute}"])
                        )
                    ).Prepend(["Enemy Id", "Map Id", "Minute"]).ToArray()
                );

                MakeCsv(
                    "stageDataBossTable", StageDataPatch.FullStageData.Where(kv
                        => kv.Value.Length > 1 && kv.Value.Any(data => data.Enemies.Any() || data.Bosses.Any())
                    ).SelectMany(kv
                        => kv.Value.SelectMany(data
                            => data.Bosses.Select(e => (string[])[$"{e}", $"{kv.Key}", $"{data.minute}"])
                        )
                    ).Prepend(["Boss Id", "Map Id", "Minute"]).ToArray()
                );

                MakeCsv(
                    "enemyTable", StageDataPatch.FullEnemyData.Select(kv => (string[])
                        [
                            kv.Value.Name, $"{kv.Key}", kv.Value.BestiaryInclude ? "True" : "False",
                            kv.Value.DlcType is not null ? $"{kv.Value.DlcType!.Value}" : "",
                            $"\"{string.Join(", ", kv.Value.Variants)}\"",
                        ]
                    ).Prepend(["Name", "Id", "Bestiary Include", "Dlc", "Variants"]).ToArray()
                );

                MakeCsv(
                    "characterTable", StageDataPatch.FullCharacterData.Select(kv => (string[])
                        [
                            kv.Value.Name.Contains(',') ? $"\"{kv.Value.Name}\"" : kv.Value.Name, $"{kv.Key}",
                            kv.Value.DlcType is not null ? $"{kv.Value.DlcType!.Value}" : "",
                        ]
                    ).Prepend(["Name", "Id", "Dlc"]).ToArray()
                );

                break;
        }
    }

    public void MakeCsv(string name, string[][] table)
    {
        if (!Directory.Exists("Csvs")) Directory.CreateDirectory("Csvs");
        File.WriteAllLines(
            $"Csvs/{name}.csv", table.Select(arr => string.Join(",", arr))
        );
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;