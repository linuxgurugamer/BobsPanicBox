using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;


namespace BobsPanicBox
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class BPB_Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Bob's Panic Box"; } }
        public override string DisplaySection { get { return "Bob's Panic Box"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = true;


        [GameParameters.CustomParameterUI("Active at launch")]
        public bool activeAtLaunch = false;

        [GameParameters.CustomParameterUI("Active in VAB")]
        public bool activeInVAB = false;

        [GameParameters.CustomParameterUI("Active in SPH")]
        public bool activeInSPH = false;

        [GameParameters.CustomParameterUI("Allow change in flight",
            toolTip = "Not implemented")]
        public bool allowChangeInFlight = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }

    public class BPB_Options2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Abort Parameters"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Bob's Panic Box"; } }
        public override string DisplaySection { get { return "Bob's Panic Box"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }




        [GameParameters.CustomParameterUI("Explosion Detection active at launch",
            toolTip = "Abort if an explosion is detected on the vessel")]
        public bool explosionDetection = false;

        [GameParameters.CustomParameterUI("Neg-Vel Detection active at launch",
            toolTip = "Abort if the max negative velocity is exceeded")]
        public bool negativeVelDetection = false;

        [GameParameters.CustomIntParameterUI("Default Negative Velocity", minValue = -100, maxValue = -1)]
        public int defaultNegVel = -40;

        [GameParameters.CustomParameterUI("High G Detection active at launch",
            toolTip = "Abort if the max G is exceeded")]
        public bool highGDetection = false;

        [GameParameters.CustomIntParameterUI("Default max G", minValue = 1, maxValue = 10)]
        public int defaultG = 6;

        [GameParameters.CustomParameterUI("Max AoA limit detection",
            toolTip = "Abort if the Max AoA is exceeded")]
        public bool exceedingAoA = false;

        [GameParameters.CustomIntParameterUI("Max AoA", minValue = 1, maxValue = 180)]
        public int maxAoA = 20;

        [GameParameters.CustomIntParameterUI("Default disable after (secs)", minValue = 1, maxValue = 600,
            toolTip = "Disable Bob's Panic Box after this many minutes of flight")]
        public int disableAfter = 600;

        [GameParameters.CustomIntParameterUI("Action after timeout", minValue = 0, maxValue = 10,
            toolTip = "Trigger this action after the timeout, 0 = none")]
        public int actionAfterTimeout = 0;


        [GameParameters.CustomIntParameterUI("Post-Abort Action (PAA)", minValue = 0, maxValue = 10,
       toolTip = "Trigger this action after an abort, 0 = none")]
        public int postAbortAction = 0;

        [GameParameters.CustomIntParameterUI("Post-Abort PAA Delay (secs)", minValue = 1, maxValue = 60,
           toolTip = "Delay after an abort before triggering the Post-Abort Action")]
        public int postAbortDelay = 10;

        [GameParameters.CustomParameterUI("Wait for safe chute deployment speed",
            toolTip = "Only if there are chutes in the PAA action")]
        public bool delayPostAbortUntilSafe = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }


    public class BPB_UI_Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Control Options"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Bob's Panic Box"; } }
        public override string DisplaySection { get { return "Bob's Panic Box"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Window via Part Action Menu")]
        public bool pamWindow = false;

        [GameParameters.CustomParameterUI("Toolbar Button")]
        public bool useToolbar = false;

        
        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }

    }
}