using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BobsPanicBox
{
    class BPB_VesselModule : VesselModule
    {

        [KSPField(isPersistant = true)]
        internal bool aborted = false;

        [KSPField(isPersistant = true)]
        internal double abortTime = 0;

        [KSPField(isPersistant = true)]
        internal bool postAbortActionCompleted = false;

        [KSPField(isPersistant = true)]
        internal bool armed = false;

        [KSPField(isPersistant = true)]
        internal string status = "Off";

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
        public int maxAoA = 20;
        
        [KSPField(isPersistant = true)]
        internal bool explosiveTriggerEnabled = false;

        [KSPField(isPersistant = true)]
        internal int disableAfter = 600;

        [KSPField(isPersistant = true)]
        internal int actionAfterTimeout = 0;

        [KSPField(isPersistant = true)]
        internal float maxTimeoutActionG = 10f;


        [KSPField(isPersistant = true)]
        internal int postAbortAction;

        [KSPField(isPersistant = true)]
        internal int postAbortDelay;

        [KSPField(isPersistant = true)]
        internal bool delayPostAbortUntilSafe;

        AbortValues av;

        new void Start()
        {
            av = new AbortValues();
            base.Start();
        }

        public void SetAllActive(bool aborted, bool armed, string status)
        {
            Log.Info("SetAllActive, aborted: " + aborted + ", armed: " + armed + ", status: " + status);
            this.aborted = aborted;
            if (aborted)
                this.abortTime = vessel.missionTime;
            this.armed = armed;
            this.status = status;
            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                if (m != null)
                {
                    m.aborted = aborted;
                    m.armed = armed;
                    m.status = status;
                }
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
            this.maxTimeoutActionG = a.maxTimeoutActionG;
            this.postAbortAction = a.postAbortAction;
            this.postAbortDelay = a.postAbortDelay;
            this.delayPostAbortUntilSafe = a.delayPostAbortUntilSafe;
            av = a;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                if (m != null)
                {
                    m.SetAllValues(av);        
                }
            }
        }

        public void TriggerAbort()
        {
            Log.Info("Triggering abort, vessel: " + vessel.name + ", vessel id: " + vessel.id);
            this.vessel.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
        }
    }
}
