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
        listing.End();
    }
}

public class SpeedyBreastfeedingSettings : ModSettings
{
    public int BreastfeedMultiplier = 2;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref BreastfeedMultiplier, "BreastfeedMultiplier", 2);
    }
}