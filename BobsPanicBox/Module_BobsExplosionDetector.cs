using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BobsPanicBox
{
    class Module_BobsExplosionDetector : PartModule
    {
        

        public void OnDestroy()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                BPB_VesselModule vm = vessel.GetComponent<BPB_VesselModule>();
                if (vm != null && vm.armed && vm.explosiveTriggerEnabled && !revert)
                {
                    Log.Info("Explosion Detected, part: " + this.part.partInfo.title);
                    ScreenMessages.PostScreenMessage("<color=red>ABORTING - Explosion Detected!</color> - "  + this.part.partInfo.title, 10f);

                    vm.SetAllActive(true, false, "Aborted! Explosion Detected");
                    GameEvents.onGameSceneSwitchRequested.Remove(onGameSceneSwitchRequested);
                    vm.TriggerAbort();
                    return;
                }
            }
            GameEvents.onGameSceneSwitchRequested.Remove(onGameSceneSwitchRequested);
        }

        void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)               
                GameEvents.onGameSceneSwitchRequested.Add(onGameSceneSwitchRequested);
        }

        bool revert = false;
        void onGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes , GameScenes > data)
        {
            if (data.from == data.to)
                revert = true;
        }
    }
}
