using ArchipelagoSurvivors.Patches;
using Il2CppVampireSurvivors.Data;
using MelonLoader;
using static ArchipelagoSurvivors.InformationTransformer;
using static ArchipelagoSurvivors.Patches.EnemyCounterPatch;

[assembly: MelonInfo(typeof(ArchipelagoSurvivors.Core), "ArchipelagoSurvivors", "1.0.0", "SW_CreeperKing", null)]
[assembly: MelonGame("poncle", "Vampire Survivors")]

namespace ArchipelagoSurvivors
{
    public class Core : MelonMod
    {
        public static MelonLogger.Instance Log;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;

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
                                                .Select(ss => Enum.Parse<EnemyType>(ss, true))
                                                .ToArray());
                                     })
                                    .ToDictionary(t => t.Item1, t => t.Item2);

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

            HarmonyInstance.PatchAll(typeof(SurvivorScreenPatch));
            HarmonyInstance.PatchAll(typeof(ChestPickupPatch));
            HarmonyInstance.PatchAll(typeof(EnemyCounterPatch));
            HarmonyInstance.PatchAll(typeof(MainMenuPatch));
            HarmonyInstance.PatchAll(typeof(PlayerPatch));

            LoggerInstance.Msg("Initialized.");
        }

        public override void OnUpdate() { APSurvivorClient.Update(); }
    }
}