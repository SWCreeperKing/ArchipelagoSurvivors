using ArchipelagoSurvivors.Patches;
using MelonLoader;

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
			
			HarmonyInstance.PatchAll(typeof(SurvivorScreenPatch));
			HarmonyInstance.PatchAll(typeof(ChestPickupPatch));
			HarmonyInstance.PatchAll(typeof(EnemyCounterPatch));
			HarmonyInstance.PatchAll(typeof(MainMenuPatch));
			HarmonyInstance.PatchAll(typeof(PlayerPatch));
			
			LoggerInstance.Msg("Initialized.");
		}
	}
}