using HarmonyLib;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;

namespace ArchipelagoSurvivors.Patches;

[PatchAll]
public static class StageDataPatch
{
    public static Dictionary<StageType, RealStageData[]> FullStageData = new();
    public static Dictionary<EnemyType, RealEnemyData> FullEnemyData = new();
    public static Dictionary<CharacterType, RealCharacterData> FullCharacterData = new();

    [HarmonyPatch(typeof(DataManager), "Initialize"), HarmonyPostfix]
    public static void LoadE(DataManager __instance)
    {
        if (Core.Debug == 0) return;
        try
        {
            foreach (var kv in __instance.AllStages)
            {
                FullStageData.Add(kv.Key, JsonConvert.DeserializeObject<RealStageData[]>(kv.Value.ToString()));
            }

            foreach (var kv in __instance.AllEnemies)
            {
                var enemy = JsonConvert.DeserializeObject<RealEnemyData>(kv.Value[0].ToString());
                if (FullEnemyData.TryGetValue(kv.Key, out _)) continue;
                FullEnemyData.Add(kv.Key, enemy);
            }

            foreach (var kv in __instance.AllCharacters)
            {
                var character = JsonConvert.DeserializeObject<RealCharacterData>(kv.Value[0].ToString());
                FullCharacterData.Add(kv.Key, character);
            }
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    [HarmonyPatch(typeof(DataManager), "BuildConvertedDlcData"), HarmonyPostfix]
    public static void LoadDlc(DataManagerSettings settings, DlcType dlcType)
    {
        if (Core.Debug == 0) return;
        try
        {
            if (settings is null) return;

            if (settings.StageDataJsonAsset is not null)
            {
                foreach (var kv in JsonConvert
                                  .DeserializeObject<
                                       Dictionary<StageType, RealStageData[]>>(settings.StageDataJsonAsset.text)
                                  .Where(kv
                                       => kv.Key is not (StageType.LEGION_TEST or StageType.BEELZEBUB_TEST
                                           or StageType.DOPPLEGANGER_TEST
                                           or StageType.TP_CASTLE_TEST)
                                   ))
                {
                    if (kv.Key is StageType.DEVILROOM) kv.Value[0].stageName = "Room 1665";
                    kv.Value[0].DlcType = dlcType;

                    FullStageData.Add(kv.Key, kv.Value);
                }
            }

            if (settings.EnemyDataJsonAsset is not null)
            {
                foreach (var kv in JsonConvert.DeserializeObject<Dictionary<EnemyType, RealEnemyData[]>>(
                             settings.EnemyDataJsonAsset.text
                         ))
                {
                    if (FullEnemyData.ContainsKey(kv.Key)) continue;
                    kv.Value[0].DlcType = dlcType;
                    
                    FullEnemyData.Add(kv.Key, kv.Value[0]);
                }
            }

            if (settings.CharacterDataJsonAsset is not null)
            {
                foreach (var kv in JsonConvert.DeserializeObject<Dictionary<CharacterType, RealCharacterData[]>>(
                             settings.CharacterDataJsonAsset.text
                         ))
                {
                    if (FullCharacterData.ContainsKey(kv.Key)) continue;
                    kv.Value[0].DlcType = dlcType;
                    
                    FullCharacterData.Add(kv.Key, kv.Value[0]);
                }
            }
        }
        catch (Exception e) { Core.Log.Error(e); }
    }
}

public class RealStageData
{
    public string stageName = "";
    public int minute = 0;
    public List<EnemyType> enemies = [];
    public List<EnemyType> bosses = [];
    public DlcType? DlcType = null;

    public string StageName => stageName;
    public int Minute => minute;
    public List<EnemyType> Enemies => enemies;
    public List<EnemyType> Bosses => bosses;
}

public class RealEnemyData
{
    public bool bInclude = false;
    public string[] bVariants = [];
    public string bName = "";
    public DlcType? DlcType = null;

    public bool BestiaryInclude => bInclude;
    public string[] Variants => bVariants;
    public string Name => bName;
}

public class RealCharacterData
{
    public string prefix = "";
    public string charName = "";
    public string surname = "";
    public DlcType? DlcType = null;

    public string Name => $"{ValidNamePart(prefix)}{ValidNamePart(charName)}{surname}".Trim();

    private string ValidNamePart(string namePart) => namePart is "" ? "" : $"{namePart} ";
}