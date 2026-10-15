using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace SpeedyBreastfeeding
{
    [HarmonyPatch(typeof(ChildcareUtility), nameof(ChildcareUtility.SuckleFromLactatingPawn))]
    public static class Patch_SuckleFromLactatingPawn
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (SpeedyBreastfeedingMod.IsFeedTicksConstant(instruction))
                {
                    // Swap the constant for a call so slider changes apply without a restart.
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(SpeedyBreastfeedingMod), nameof(SpeedyBreastfeedingMod.BreastfeedTicks));
                }

                yield return instruction;
            }
        }
    }
}
