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
        internal float disableAfter = 600;

        [KSPField(isPersistant = true)]
        internal int actionAfterTimeout = 0;


        [KSPField(isPersistant = true)]
        internal int postAbortAction;
        [KSPField(isPersistant = true)]
        internal float postAbortDelay;
        [KSPField(isPersistant = true)]
        internal bool delayPostAbortUntilSafe;

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
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.aborted = aborted;
                    m.armed = armed;
                    m.status = status;
                }
            }
        }

        public void SetAllVertSpeed(bool enabled, float vspeed)
        {
            this.vertSpeedTriggerEnabled = enabled;
            this.vertSpeed = vspeed;
            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.vertSpeedTriggerEnabled = enabled;
                    m.vertSpeed = vspeed;
                }
            }
        }

        public void SetAllgForce(bool enabled, float gForceTrigger)
        {
            this.gForceTriggerEnabled = enabled;
            this.gForceTrigger = gForceTrigger;
            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.gForceTriggerEnabled = enabled;
                    m.gForceTrigger = gForceTrigger;
                }
            }
        }

        public void SetAllMaxAoA(bool enabled, int maxAoA)
        {
            this.exceedingAoA = enabled;
            this.maxAoA = maxAoA;
            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.exceedingAoA = enabled;
                    m.maxAoA = maxAoA;
                }
            }
        }

        public void SetAllExplosionDetection(bool explosiveTriggerEnabled)
        {
            this.explosiveTriggerEnabled = explosiveTriggerEnabled;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.explosiveTriggerEnabled = explosiveTriggerEnabled;
                }
            }
        }
        public void SetDisableAfter(float disableAfter)
        {
            this.disableAfter = disableAfter;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.disableAfter = disableAfter;
                }
            }
        }
        public void SetActionAfterTimeout(int actionAfterTimeout)
        {
            this.actionAfterTimeout = actionAfterTimeout;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.actionAfterTimeout = actionAfterTimeout;
                }
            }
        }


        public void SetPostAbortAction(int postAbortAction)
        {
            this.postAbortAction = postAbortAction;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.postAbortAction = postAbortAction;
                }
            }
        }
        public void SetPostAbortDelay(float postAbortDelay)
        {
            this.postAbortDelay = postAbortDelay;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.postAbortDelay = postAbortDelay;
                }
            }
        }
        public void SetDelayPostAbortUntilSafe(bool delayPostAbortUntilSafe)
        {
            this.delayPostAbortUntilSafe = delayPostAbortUntilSafe;

            foreach (var p in this.vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                }
            }
        }


        public void TriggerAbort()
        {
            this.vessel.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
        }
    }
}
