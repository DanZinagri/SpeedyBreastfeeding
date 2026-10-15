using HarmonyLib;
using Verse;

namespace SpeedyBreastfeeding
{
    // Lactation Expansion (Euterpe.MilkyTitfuck) prefixes ChildcareUtility.SuckleFromLactatingPawn and returns false, so the vanilla method we patch never runs.
    // Its replacement, LactationUtilities.Suckle, uses the same 5000f tick constant, so we apply the same logic if their mod is present.
    [StaticConstructorOnStartup]
    public static class Patch_LactationExpansionCompat
    {
        static Patch_LactationExpansionCompat()
        {
            var lactationUtilities = AccessTools.TypeByName("EuterpeMilkyTitfuck.LactationUtilities");
            if (lactationUtilities == null)
            {
                return; // Lactation Expansion not loaded; vanilla patch handles everything
            }

            var suckle = AccessTools.Method(lactationUtilities, "Suckle");
            if (suckle == null)
            {
                Log.Warning("[SpeedyBreastfeeding] Lactation Expansion detected but LactationUtilities.Suckle was not found; breastfeed speed setting will have no effect while it is loaded.");
                return;
            }

            new Harmony("DanZinagri.SpeedyBreastfeeding").Patch(suckle,
                transpiler: new HarmonyMethod(typeof(Patch_SuckleFromLactatingPawn), nameof(Patch_SuckleFromLactatingPawn.Transpiler)));
            SpeedyBreastfeedingMod.DebugLog("Lactation Expansion detected; applied breastfeed speed to LactationUtilities.Suckle.");
        }
    }
}
