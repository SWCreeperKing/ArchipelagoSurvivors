using HarmonyLib;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Stage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class StageDataPatch
{
    [HarmonyPatch(typeof(DataManager), "Initialize"), HarmonyPostfix]
    public static void LoadE(DataManager __instance)
    {
        var fullStageData
            = JsonConvert.DeserializeObject<List<RealStageData>>(__instance.AllStages[StageType.FOREST].ToString()); 
        // foreach (var stageData in __instance.AllStages[StageType.FOREST].ToObject<IEnumerable<StageData>>())
        foreach (var stageData in fullStageData)
        {
            Core.Log.Msg(
                $"Forest min: [{stageData.minute}]: [{string.Join(", ", stageData.enemies.Select(t => $"{t}"))}], [{string.Join(", ", stageData.bosses.Select(t => $"{t}"))}]"
            );
        }

        PrintData(fullStageData.Take(15).ToArray());
        PrintData(fullStageData.Skip(15).ToArray());
    }

    public static void PrintData(RealStageData[] data)
    {
        Core.Log.Msg($"minutes: [{string.Join(",", data.Select(d => d.minute))}]");
        Core.Log.Msg($"enemies: [{string.Join(",", data.SelectMany(d => d.enemies).ToHashSet())}]");
        Core.Log.Msg($"bosses: [{string.Join(",", data.SelectMany(d => d.bosses).ToHashSet())}]\n");
    }

    [HarmonyPatch(typeof(DataManager), "Initialize"), HarmonyPostfix]
    public static void Load(DataManager __instance)
    {
        // Directory.CreateDirectory("Stages");
        // Directory.CreateDirectory("Enemies");
        //
        // foreach (var (stage, arr) in __instance.AllStages)
        // {
        //     File.WriteAllText($"Stages/{stage}.json", arr.ToString());
        //     Core.Log.Msg($"Stage [{stage}] [{arr[0].Value<string>("stageName")}]");
        // }
        //
        // foreach (var (enemy, arr) in __instance.AllEnemies)
        // {
        //     File.WriteAllText($"Enemies/{enemy}.json", arr.ToString());
        // }
        // Core.Log.Msg($"Enemies [{__instance.AllEnemies.Count}]");

        Core.Log.Msg("========= DATA WRITTEN =========");
    }

    [HarmonyPatch(typeof(DataManager), "BuildConvertedDlcData"), HarmonyPostfix]
    public static void LoadDlc(DataManagerSettings settings)
    {
        // if (settings is null) return;
        //
        // if (settings.StageDataJsonAsset is not null)
        // {
        //     var dict = JObject.Parse(settings.StageDataJsonAsset.text).ToObject<Dictionary<StageType, JArray>>();
        //     if (dict is not null)
        //     {
        //         foreach (var (stage, arr) in dict)
        //         {
        //             File.WriteAllText($"Stages/{stage}.json", arr.ToString());
        //             Core.Log.Msg($"Stage [{stage}] [{arr[0]["stageName"]}]");
        //         }
        //     }
        // }
        //
        // if (settings.EnemyDataJsonAsset is not null)
        // {
        //     var dict = JObject.Parse(settings.EnemyDataJsonAsset.text).ToObject<Dictionary<EnemyType, JArray>>();
        //     if (dict is not null)
        //     {
        //         foreach (var (enemy, arr) in dict) { File.WriteAllText($"Enemies/{enemy}.json", arr.ToString()); }
        //         Core.Log.Msg($"Enemies [{dict.Count}]");
        //     }
        // }

        Core.Log.Msg("========= DATA WRITTEN FOR DLC =========");
    }
}

public struct RealStageData
{
    public int minute = 0;
    public List<EnemyType> enemies = [];
    public List<EnemyType> bosses = [];

    public RealStageData()
    {
    }
}