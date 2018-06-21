using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BobsPanicBox
{
    class Module_BobsExplosionDetector : PartModule
    {
        BPB_VesselModule vm;

        public void OnDestroy()
        {
            if (HighLogic.LoadedSceneIsFlight && vm != null)
            {
                if (vm.armed && vm.explosiveTriggerEnabled)
                {
                    Log.Info("Explosion Detected, part: " + this.part.partInfo.title);
                    ScreenMessages.PostScreenMessage("<color=red>ABORTING - Explosion Detected!</color> - "  + this.part.partInfo.title, 10f);

                    vm.SetAllActive(true, false, "Aborted! Explosion Detected");
                    vm.TriggerAbort();
                }
            }
        }

        void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
                vm = vessel.GetComponent<BPB_VesselModule>();
        }
    }
}
