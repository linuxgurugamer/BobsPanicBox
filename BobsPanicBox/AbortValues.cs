using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BobsPanicBox
{
    public class AbortValues : ICloneable
    {
        public bool armed;
        public string status;
        public bool vertSpeedTriggerEnabled;
        public float vertSpeed;
        public bool gForceTriggerEnabled;
        public float gForceTrigger;
        public bool exceedingAoA;
        public int maxAoA;
        public bool explosiveTriggerEnabled;
        internal int disableAfter;
        internal int actionAfterTimeout;
        internal int disableAtAltitudeKm = 100;
        internal int disableAtAltitude = 100000;
        internal int ignoreAoAAboveAltitudeKm = 100;
        internal int ignoreAoAAboveAltitude = 100000;

        internal float maxTimeoutActionG = 10f;

        internal int postAbortAction;
        internal int postAbortDelay;
        internal bool delayPostAbortUntilSafe;

        Flight flightParent = null;
        Editor editorParent = null;

        public AbortValues()
        {
            ResetToDefault();
        }

        public Object Clone()
        {
            AbortValues av = (AbortValues)this.MemberwiseClone();
            return av;
        }

        internal void ResetToDefault()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT && HighLogic.LoadedScene != GameScenes.EDITOR)
                return;

            if (HighLogic.LoadedSceneIsFlight)
                armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeAtLaunch;
            else
            {
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB)
                    armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB;
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH)
                    armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH;
            }
            vertSpeedTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().negativeVelDetection;
            gForceTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().highGDetection;
            vertSpeed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().defaultNegVel;
            exceedingAoA = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().exceedingAoA;
            maxAoA = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().maxAoA;
            gForceTrigger = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().defaultG;
            explosiveTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().explosionDetection;
            disableAfter = (int)HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().disableAfter;
            actionAfterTimeout = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().actionAfterTimeout;
            maxTimeoutActionG = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().maxTimeoutActionG;
            postAbortAction = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().postAbortAction;
            postAbortDelay = (int)HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().postAbortDelay;
            delayPostAbortUntilSafe = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().delayPostAbortUntilSafe;
            ignoreAoAAboveAltitudeKm = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().ignoreAoAAboveAltitudeKm;
            disableAtAltitudeKm = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().disableAtAltitudeKm;
        }

 
        internal AbortValues(Flight f)
        {
            flightParent = f;
            ResetToDefault();
        }
        internal AbortValues(Editor f)
        {
            editorParent = f;
            ResetToDefault();
        }

        public void SetAllValues(AbortValues av)
        {
            if (av != null)
            {
                this.vertSpeedTriggerEnabled = av.vertSpeedTriggerEnabled;
                this.vertSpeed = av.vertSpeed;

                this.gForceTriggerEnabled = av.gForceTriggerEnabled;
                this.gForceTrigger = av.gForceTrigger;

                this.exceedingAoA = av.exceedingAoA;
                this.maxAoA = av.maxAoA;

                this.explosiveTriggerEnabled = av.explosiveTriggerEnabled;
                this.disableAfter = av.disableAfter;
                this.actionAfterTimeout = av.actionAfterTimeout;
                this.disableAtAltitudeKm = av.disableAtAltitudeKm;
                this.disableAtAltitude = av.disableAtAltitudeKm * 1000;
                this.ignoreAoAAboveAltitudeKm = av.ignoreAoAAboveAltitudeKm;
                this.ignoreAoAAboveAltitude = av.ignoreAoAAboveAltitudeKm * 1000;
                this.maxTimeoutActionG = av.maxTimeoutActionG;
                this.postAbortAction = av.postAbortAction;
                this.postAbortDelay = av.postAbortDelay;
                this.delayPostAbortUntilSafe = av.delayPostAbortUntilSafe;

                List<Part> parts;
                if (HighLogic.LoadedSceneIsEditor)
                    parts = EditorLogic.fetch.ship.parts;
                else
                    parts = FlightGlobals.ActiveVessel.Parts;


                foreach (var p in parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    if (m != null)
                    {
                        m.SetAllValues(av);
                    }
                }
            }
        }

        public bool Changed(Module_BobsPanicBox m)
        {
            return armed != m.armed ||
                status != m.status ||
                vertSpeedTriggerEnabled != m.vertSpeedTriggerEnabled ||
                vertSpeed != m.vertSpeed ||
                gForceTriggerEnabled != m.gForceTriggerEnabled ||
                gForceTrigger != m.gForceTrigger ||
                explosiveTriggerEnabled != m.explosiveTriggerEnabled ||
                disableAfter != m.disableAfter ||
                exceedingAoA != m.exceedingAoA ||
                maxAoA != m.maxAoA ||
                actionAfterTimeout != m.actionAfterTimeout ||
                postAbortAction != m.postAbortAction ||
                postAbortDelay != m.postAbortDelay ||
                delayPostAbortUntilSafe != m.delayPostAbortUntilSafe ||
                maxTimeoutActionG != m.maxTimeoutActionG ||
                disableAtAltitudeKm != m.disableAtAltitudeKm ||
                ignoreAoAAboveAltitudeKm != m.ignoreAoAAboveAltitudeKm;
        }

        public void SaveCurrent(Module_BobsPanicBox m)
        {
            armed = m.armed;
            status = m.status;
            vertSpeedTriggerEnabled = m.vertSpeedTriggerEnabled;
            vertSpeed = m.vertSpeed;
            gForceTriggerEnabled = m.gForceTriggerEnabled;
            gForceTrigger = m.gForceTrigger;
            exceedingAoA = m.exceedingAoA;
            maxAoA = m.maxAoA;
            explosiveTriggerEnabled = m.explosiveTriggerEnabled;
            disableAfter = m.disableAfter;
            actionAfterTimeout = m.actionAfterTimeout;
            disableAtAltitudeKm = m.disableAtAltitudeKm;
            disableAtAltitude = m.disableAtAltitudeKm * 1000;
            ignoreAoAAboveAltitudeKm = m.ignoreAoAAboveAltitudeKm;
            ignoreAoAAboveAltitude = m.ignoreAoAAboveAltitudeKm * 1000;
            maxTimeoutActionG = m.maxTimeoutActionG;

            postAbortAction = m.postAbortAction;
            postAbortDelay = m.postAbortDelay;
            delayPostAbortUntilSafe = m.delayPostAbortUntilSafe;

            if (HighLogic.LoadedSceneIsFlight)
            {
                m.vm.armed = m.armed;
                m.vm.status = m.status;
                m.vm.vertSpeedTriggerEnabled = m.vertSpeedTriggerEnabled;
                m.vm.vertSpeed = m.vertSpeed;
                m.vm.gForceTriggerEnabled = m.gForceTriggerEnabled;
                m.vm.gForceTrigger = m.gForceTrigger;
                m.vm.exceedingAoA = m.exceedingAoA;
                m.vm.maxAoA = m.maxAoA;
                m.vm.explosiveTriggerEnabled = m.explosiveTriggerEnabled;
                m.vm.disableAfter = m.disableAfter;
                m.vm.actionAfterTimeout = m.actionAfterTimeout;
                m.vm.disableAtAltitudeKm = m.disableAtAltitudeKm;
                m.vm.disableAtAltitude = m.disableAtAltitudeKm * 1000;
                m.vm.ignoreAoAAboveAltitudeKm = m.ignoreAoAAboveAltitudeKm;
                m.vm.ignoreAoAAboveAltitude = m.ignoreAoAAboveAltitudeKm * 1000;
                m.vm.maxTimeoutActionG = m.maxTimeoutActionG;

                m.vm.postAbortAction = m.postAbortAction;
                m.vm.postAbortDelay = m.postAbortDelay;
                m.vm.delayPostAbortUntilSafe = m.delayPostAbortUntilSafe;

            }
        }
    }
}
