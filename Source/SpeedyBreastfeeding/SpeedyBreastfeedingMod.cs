using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace SpeedyBreastfeeding;

public class SpeedyBreastfeedingMod : Mod
{
    public static SpeedyBreastfeedingSettings Settings;

    public SpeedyBreastfeedingMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<SpeedyBreastfeedingSettings>();
        Harmony harmony = new Harmony("DanZinagri.SpeedyBreastfeeding");
        harmony.PatchAll();
    }

    // Vanilla's ticks-per-full-meal constant, shared by breastfeeding and bottle feeding.
    public const float VanillaFeedTicks = 5000f;

    public static bool IsFeedTicksConstant(CodeInstruction instruction) =>
        instruction.opcode == System.Reflection.Emit.OpCodes.Ldc_R4 && instruction.operand is float value && value == VanillaFeedTicks;

    // Called from the patched IL in place of the 5000f constant, so they read the current settings.
    public static float BreastfeedTicks() => VanillaFeedTicks / Settings.BreastfeedMultiplier;
    public static float BabyfoodTicks() => VanillaFeedTicks / Settings.BabyfoodMultiplier;

    public static void DebugLog(string message)
    {
        if (Settings.DebugLogging)
        {
            Log.Message("[SpeedyBreastfeeding] " + message);
        }
    }

    public override string SettingsCategory() => "SpeedyBreastfeedingTitle".Translate();

    public override void WriteSettings()
    {
        base.WriteSettings();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Listing_Standard listing = new();
        listing.Begin(inRect);
        listing.Label("SpeedyBreastfeeding.Scale".Translate() + ": " + Settings.BreastfeedMultiplier);
        //we're setting the minimum to anything but 0.
        Settings.BreastfeedMultiplier = (int) listing.Slider(Settings.BreastfeedMultiplier, 1f, 10f);
        listing.GapLine();
        listing.Label("SpeedyBreastfeeding.Scale2".Translate() + ": " + Settings.BabyfoodMultiplier);
        Settings.BabyfoodMultiplier = (int)listing.Slider(Settings.BabyfoodMultiplier, 1f, 10f);
        listing.GapLine();
        listing.CheckboxLabeled("SpeedyBreastfeeding.DebugLogging".Translate(), ref Settings.DebugLogging, "SpeedyBreastfeeding.DebugLoggingDesc".Translate());
        listing.End();
    }
}

public class SpeedyBreastfeedingSettings : ModSettings
{
    public int BreastfeedMultiplier = 2;
    public int BabyfoodMultiplier = 2;
    public bool DebugLogging = false;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref BreastfeedMultiplier, "BreastfeedMultiplier", 2);
        Scribe_Values.Look(ref BabyfoodMultiplier, "BabyfoodMultiplier", 2);
        Scribe_Values.Look(ref DebugLogging, "DebugLogging", false);
    }
}