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
        internal AbortValues av;

        static internal Editor editorInfo = null;
        internal Flight flightInfo;

 
        [KSPField(isPersistant = true)]
        internal int disableAfter = 600;

        [KSPField(isPersistant = true)]
        internal int actionAfterTimeout = 0;

        [KSPField(isPersistant = true)]
        internal int disableAtAltitudeKm = 100;

        [KSPField(isPersistant = true)]
        internal int disableAtAltitude = 100000;

        [KSPField(isPersistant = true)]
        internal int ignoreAoAAboveAltitudeKm = 100;

        [KSPField(isPersistant = true)]
        internal int ignoreAoAAboveAltitude = 100000;

        [KSPField(isPersistant = true)]
        internal float maxTimeoutActionG = 10f;

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
        internal int postAbortDelay;

        [KSPField(isPersistant = true)]
        internal bool delayPostAbortUntilSafe;

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Bob's Panic Box Window")]
        public void openAutoAbort()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                BPB_Editor_Window.Instance.EnableWindow(editorInfo.av);
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                BPB_Editor_Window.Instance.EnableWindow(flightInfo.av);
            }
        }

        public void SetAllValues(AbortValues a)
        {
            if (a != null)
            {
                this.armed = a.armed;
    
                this.vertSpeedTriggerEnabled = a.vertSpeedTriggerEnabled;
                this.vertSpeed = a.vertSpeed;

                this.gForceTriggerEnabled = a.gForceTriggerEnabled;
                this.gForceTrigger = a.gForceTrigger;

                this.exceedingAoA = a.exceedingAoA;
                this.maxAoA = a.maxAoA;

                this.explosiveTriggerEnabled = a.explosiveTriggerEnabled;
                this.disableAfter = a.disableAfter;
                this.actionAfterTimeout = a.actionAfterTimeout;
                this.disableAtAltitudeKm = a.disableAtAltitudeKm;
                this.disableAtAltitude = a.disableAtAltitudeKm * 1000;
                this.ignoreAoAAboveAltitudeKm = a.ignoreAoAAboveAltitudeKm;
                this.ignoreAoAAboveAltitude = a.ignoreAoAAboveAltitudeKm * 1000;
                this.maxTimeoutActionG = a.maxTimeoutActionG;
                this.postAbortAction = a.postAbortAction;
                this.postAbortDelay = a.postAbortDelay;
                this.delayPostAbortUntilSafe = a.delayPostAbortUntilSafe;
                av = a;
            }
        }

        public void CopyOrInit()
        {
            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                if (p != this.part)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    if (m != null)
                    {
                        SetAllValues(m.av);
                        return;
                    }
                }
            }
            av.ResetToDefault();
            SetAllValues(av);
        }

        void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
                return;

            if (HighLogic.LoadedSceneIsFlight)
            {
                vm = this.vessel.GetComponent<BPB_VesselModule>();
                flightInfo = new Flight();
                av = new AbortValues(flightInfo);
                Events["openAutoAbort"].guiActive = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().allowChangeInFlight && HighLogic.CurrentGame.Parameters.CustomParams<BPB_UI_Options>().pawWindow;
            }
            else
            {
                if (editorInfo == null)
                {
                    editorInfo = new Editor();
                    SetAllValues(editorInfo.av);
                }
                if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB && EditorDriver.editorFacility == EditorFacility.VAB)
                    Events["openAutoAbort"].guiActive = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB && HighLogic.CurrentGame.Parameters.CustomParams<BPB_UI_Options>().pawWindow;
                if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH && EditorDriver.editorFacility == EditorFacility.SPH)
                    Events["openAutoAbort"].guiActive = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH && HighLogic.CurrentGame.Parameters.CustomParams<BPB_UI_Options>().pawWindow;
                av = new AbortValues(editorInfo);
            }

            if (!initted)
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    av = new AbortValues(flightInfo);
                    CopyOrInit();
                }
                else
                {
                    
                    flightInfo.av.ResetToDefault();
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
                    editorInfo.av.disableAtAltitudeKm = disableAtAltitudeKm;
                    editorInfo.av.disableAtAltitude = disableAtAltitudeKm * 1000;
                    editorInfo.av.ignoreAoAAboveAltitudeKm = ignoreAoAAboveAltitudeKm;
                    editorInfo.av.ignoreAoAAboveAltitude = ignoreAoAAboveAltitudeKm * 1000;
                    editorInfo.av.maxTimeoutActionG = maxTimeoutActionG;
                    editorInfo.av.postAbortAction = postAbortAction;
                    editorInfo.av.postAbortDelay = postAbortDelay;
                    editorInfo.av.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                    
                    SetAllValues(editorInfo.av);
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
                    flightInfo.av.disableAtAltitudeKm = disableAtAltitudeKm;
                    flightInfo.av.disableAtAltitude = disableAtAltitudeKm * 1000;
                    flightInfo.av.ignoreAoAAboveAltitudeKm = ignoreAoAAboveAltitudeKm;
                    flightInfo.av.ignoreAoAAboveAltitude = ignoreAoAAboveAltitudeKm * 1000;
                    flightInfo.av.maxTimeoutActionG = maxTimeoutActionG;
                    flightInfo.av.postAbortAction = postAbortAction;
                    flightInfo.av.postAbortDelay = postAbortDelay;
                    flightInfo.av.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                    av = (AbortValues)flightInfo.av.Clone();
                    SetAllValues(flightInfo.av);
                    if (vm != null)
                        vm.SetAllValues(flightInfo.av);
                    else
                        Log.Info("No VesselModule found");
                    flightInfo.SaveCurrent(this);
                }
            }
        }
    }
}