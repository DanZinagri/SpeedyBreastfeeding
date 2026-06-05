using HarmonyLib;
using RimWorld;
using SpeedyBreastfeeding;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

[HarmonyPatch]
public static class Patch_FeedBabyFoodFromInventory
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (var method in AccessTools.GetDeclaredMethods(typeof(JobDriver_BottleFeedBaby)))
        {
            if (method.Name.Contains("FeedBabyFoodFromInventory") &&
                method.Name.Contains("b__"))
            {
                Log.Message("[SpeedyBreastfeeding] Checking baby food method: " + method.Name);
                yield return method;
            }
        }
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        bool found = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 &&
                instruction.operand is float value &&
                value == 5000f)
            {
                found = true;
                Log.Message("[SpeedyBreastfeeding] Replacing 5000f in " + __originalMethod.Name);
                instruction.operand = 5000f / SpeedyBreastfeedingMod.Settings.BabyfoodMultiplier;
            }

            yield return instruction;
        }

        if (!found)
        {
            Log.Message("[SpeedyBreastfeeding] No 5000f found in " + __originalMethod.Name);
        }
    }
}