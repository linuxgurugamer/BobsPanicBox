using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BobsPanicBox
{
   
    public class Module_BobsPanicBox : PartModule
    {
        internal BPB_VesselModule vm;
        AbortValues av;

        static internal Editor editorInfo = new Editor();
        internal Flight flightInfo;

 
        [KSPField(isPersistant = true)]
        internal float disableAfter = 600;

        [KSPField(isPersistant = true)]
        internal int actionAfterTimeout = 0; 

        [KSPField(isPersistant = true)]
        bool initted = false;

        [KSPField(isPersistant = true)]
        internal bool aborted = false;

        [KSPField(isPersistant = true)]
        internal string status = "Off";

        [KSPField(isPersistant = true)]
        internal bool armed = false;

        [KSPField(isPersistant = true)]
        internal bool vertSpeedTriggerEnabled = false;

        [KSPField(isPersistant = true)]
        internal float vertSpeed = -40;

        [KSPField(isPersistant = true)]
        internal bool gForceTriggerEnabled = false;

        [KSPField(isPersistant = true)]
        public float gForceTrigger = 6;

        [KSPField(isPersistant = true)]
        internal bool exceedingAoA = false;

        [KSPField(isPersistant = true)]
        internal int maxAoA = 20;

        [KSPField(isPersistant = true)]
        internal bool explosiveTriggerEnabled = false;

        [KSPField(isPersistant = true)]
        internal int postAbortAction;

        [KSPField(isPersistant = true)]
        internal float postAbortDelay;

        [KSPField(isPersistant = true)]
        internal bool delayPostAbortUntilSafe;

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Bob's Panic Box Window")]
        public void openAutoAbort()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                BPB_Editor.Instance.EnableWindow(editorInfo.av);
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                Log.Info("Module_BobsPanicBox, openAutoAbort");
                BPB_Editor.Instance.EnableWindow(flightInfo.av);
            }
        }

        public void SetAllValues(AbortValues a)
        {
            this.vertSpeedTriggerEnabled = a.vertSpeedTriggerEnabled;
            this.vertSpeed = a.vertSpeed;

            this.gForceTriggerEnabled = a.gForceTriggerEnabled;
            this.gForceTrigger = a.gForceTrigger;

            this.exceedingAoA = a.exceedingAoA;
            this.maxAoA = a.maxAoA;

            this.explosiveTriggerEnabled = a.explosiveTriggerEnabled;
            this.disableAfter = a.disableAfter;
            this.actionAfterTimeout = a.actionAfterTimeout;
            this.postAbortAction = a.postAbortAction;
            this.postAbortDelay = a.postAbortDelay;
            this.delayPostAbortUntilSafe = a.delayPostAbortUntilSafe;
            av = a;
        }

        void CopyOrInit()
        {
            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                if (p != this.part)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    SetAllValues(m.av);
                    return;
                }
            }
            BPB_Editor.ResetToDefault(av);
        }

        void Start()
        {
            Log.Info("Module_BobsAbortBox.Start, LoadedScene: " + HighLogic.LoadedScene + ", initted: " + initted);
            if (HighLogic.LoadedSceneIsFlight)
            {
                vm = this.vessel.GetComponent<BPB_VesselModule>();
                flightInfo = new Flight();
            }

            if (!initted)
            {
                Log.Info("Module_BobsAbortBox.Start, !initted");
                if (HighLogic.LoadedSceneIsEditor)
                {
                    CopyOrInit();
                }
                else
                {
                    BPB_Editor.ResetToDefault(flightInfo.av);
                    SetAllValues(av);

                    flightInfo.SaveCurrent(this);
                }
                initted = true;
            }
            else
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    editorInfo.av.armed = armed;
                    editorInfo.av.vertSpeedTriggerEnabled = vertSpeedTriggerEnabled;
                    editorInfo.av.gForceTriggerEnabled = gForceTriggerEnabled;
                    editorInfo.av.vertSpeed = vertSpeed;
                    editorInfo.av.exceedingAoA = exceedingAoA;
                    editorInfo.av.maxAoA = maxAoA;
                    editorInfo.av.gForceTrigger = gForceTrigger;
                    editorInfo.av.explosiveTriggerEnabled = explosiveTriggerEnabled;
                    editorInfo.av.disableAfter = disableAfter;
                    editorInfo.av.actionAfterTimeout = actionAfterTimeout;
                    editorInfo.av.postAbortAction = postAbortAction;
                    editorInfo.av.postAbortDelay = postAbortDelay;
                    editorInfo.av.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                }
                else
                {
                    flightInfo.av.armed = armed;
                    flightInfo.av.vertSpeedTriggerEnabled = vertSpeedTriggerEnabled;
                    flightInfo.av.gForceTriggerEnabled = gForceTriggerEnabled;
                    flightInfo.av.vertSpeed = vertSpeed;
                    flightInfo.av.exceedingAoA = exceedingAoA;
                    flightInfo.av.maxAoA = maxAoA;
                    flightInfo.av.gForceTrigger = gForceTrigger;
                    flightInfo.av.explosiveTriggerEnabled = explosiveTriggerEnabled;
                    flightInfo.av.disableAfter = disableAfter;
                    flightInfo.av.actionAfterTimeout = actionAfterTimeout;
                    flightInfo.av.postAbortAction = postAbortAction;
                    flightInfo.av.postAbortDelay = postAbortDelay;
                    flightInfo.av.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                    flightInfo.SaveCurrent(this);
                }
            }


        }

        internal static bool IsInitiallyActive()
        {
            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB && HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB)
                return HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB;
            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH && HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH)
                return HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB;
            if (HighLogic.LoadedSceneIsFlight && HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeAtLaunch)
                return HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB;
            return false;
        }
    }
}