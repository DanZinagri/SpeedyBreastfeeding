using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime;
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
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float value && value == 5000f)
                {
                    instruction.operand = 5000f / SpeedyBreastfeedingMod.Settings.BreastfeedMultiplier; // 2x faster feeding
                }

                yield return instruction;
            }
        }
    }
}