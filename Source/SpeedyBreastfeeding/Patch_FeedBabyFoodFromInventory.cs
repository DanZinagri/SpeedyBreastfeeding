using HarmonyLib;
using RimWorld;
using SpeedyBreastfeeding;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

// The bottle-feed rate lives in a compiler-generated lambda (FeedingToil.tickIntervalAction) inside
// JobDriver_BottleFeedBaby.FeedBabyFoodFromInventory, so it can't be targeted by name. We scan that method's
// lambdas and patch only the one whose IL actually contains the 5000f constant.
[HarmonyPatch]
public static class Patch_FeedBabyFoodFromInventory
{
    private static List<MethodBase> targets;

    static bool Prepare()
    {
        targets = AccessTools.GetDeclaredMethods(typeof(JobDriver_BottleFeedBaby))
            .Where(m => m.Name.Contains("FeedBabyFoodFromInventory") && m.Name.Contains("b__"))
            .Where(m => PatchProcessor.GetOriginalInstructions(m).Any(SpeedyBreastfeedingMod.IsFeedTicksConstant))
            .Cast<MethodBase>()
            .ToList();

        if (targets.Count == 0)
        {
            Log.Warning("[SpeedyBreastfeeding] Could not find the baby food feeding rate in JobDriver_BottleFeedBaby; baby food speed setting will have no effect.");
            return false;
        }

        foreach (MethodBase method in targets)
        {
            SpeedyBreastfeedingMod.DebugLog("Patching baby food method: " + method.Name);
        }
        return true;
    }

    static IEnumerable<MethodBase> TargetMethods() => targets;

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (SpeedyBreastfeedingMod.IsFeedTicksConstant(instruction))
            {
                // Swap the constant for a call so slider changes apply without a restart.
                instruction.opcode = OpCodes.Call;
                instruction.operand = AccessTools.Method(typeof(SpeedyBreastfeedingMod), nameof(SpeedyBreastfeedingMod.BabyfoodTicks));
            }

            yield return instruction;
        }
    }
}
