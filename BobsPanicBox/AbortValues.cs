using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BobsPanicBox
{
    public class AbortValues
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

        internal int postAbortAction;
        internal int postAbortDelay;
        internal bool delayPostAbortUntilSafe;

        Flight flightParent = null;
        Editor editorParent = null;

       // Module_BobsPanicBox moduleBPB = null;

   
        internal AbortValues()
        { }
        internal AbortValues(Flight f)
        {
            flightParent = f;
        }
        internal AbortValues(Editor f)
        {
            editorParent = f;
        }

        public void SetAllValues(AbortValues av)
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
                delayPostAbortUntilSafe != m.delayPostAbortUntilSafe;
        }

        public void SaveCurrent(Module_BobsPanicBox m)
        {
            Log.Info("AbortValues.SaveCurrent, armed: " + m.armed);
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

                m.vm.postAbortAction = m.postAbortAction;
                m.vm.postAbortDelay = m.postAbortDelay;
                m.vm.delayPostAbortUntilSafe = m.delayPostAbortUntilSafe;

            }
        }
    }
}
