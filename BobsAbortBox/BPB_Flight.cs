using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BobsPanicBox
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BPB_Flight : MonoBehaviour
    {
        Vessel lastActiveVessel;
        BPB_VesselModule vm;


        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                Vessel v = FlightGlobals.ActiveVessel;

                if (v != lastActiveVessel)
                {
                    lastActiveVessel = v;
                    vm = v.GetComponent<BPB_VesselModule>();
                }

                if (vm.armed)
                {
                    if (v.missionTime <= vm.disableAfter)
                    {
                        if (v.verticalSpeed < vm.vertSpeed && vm.vertSpeedTriggerEnabled)
                        {
                            if (!vm.aborted )
                            {
                                ScreenMessages.PostScreenMessage("<color=red>ABORTING - AAS Negative Vertical Velocity Detected!</color> - " + v.verticalSpeed, 10f);
                                v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                                vm.SetAllActive(true, false, "Aborted! Negative Vertical Velocity Detected");
                            }
                        }

                        if (v.geeForce > vm.gForceTrigger && vm.gForceTriggerEnabled)
                        {
                            if (!vm.aborted )
                            {
                                ScreenMessages.PostScreenMessage("<color=red>ABORTING - AAS High G-Force Detected!</color> - " + v.geeForce, 10f);
                                v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                                vm.SetAllActive(true, false, "Aborted! High G-Force Detected");
                            }
                        }

                        Log.Info("BAB_Flight.Update, vm.exceedingAoA: " + vm.exceedingAoA);
                        if (vm.exceedingAoA && v.GetSrfVelocity().magnitude > 10 && v.missionTime > 1)
                        {
                            var v3d1 = Vector3d.Angle(v.GetTransform().up, v.GetSrfVelocity());
                            Log.Info("BAB_Flight.Update, v3d1: " + v3d1);
                            if (v3d1 > vm.maxAoA)
                            {
                                Log.Info("v3d1: " + v3d1 + ", v.GetSrfVelocity().magnitude: " + v.GetSrfVelocity().magnitude + ", vm.maxAoA: " + vm.maxAoA);
                                ScreenMessages.PostScreenMessage("<color=red>ABORTING - Max AoA Exceeded!</color> - " + vm.maxAoA + " degrees", 10f);
                                v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                                vm.SetAllActive(true, false, "Aborted! Max AoA Exceeded");
                            }

                        }
                    } else
                    {
                        vm.armed = false;
                        ScreenMessages.PostScreenMessage("Bob's Abort Box disabled due to timeout", 10f);
                        Log.Info("Bob's Abort Box disabled due to timeout");
                        if (vm.actionAfterTimeout > 0)
                        {
                            var kg = GetActionGroup((int)vm.actionAfterTimeout);
                            Log.Info("Calling action group: " + kg);
                            v.ActionGroups.SetGroup(kg, true);
                        }
                    }
                }
                if (vm.aborted && vm.postAbortAction != 0 && !vm.postAbortActionCompleted)
                {
                    if (vm.abortTime + vm.postAbortDelay <= v.missionTime)
                    {
                        // Check for safechute
                        // check for chutes
                        var pa = v.FindPartModulesImplementing<ModuleParachute>();
                        if (pa != null && pa.Count > 0)
                        {
                            foreach (var chute in pa)
                            {
                                if (chute.deploymentState == ModuleParachute.deploymentStates.STOWED && FlightGlobals.ActiveVessel.atmDensity > 0)
                                {
                                    if (chute.deploySafe == "Safe")
                                    {
                                        var kg = GetActionGroup(vm.postAbortAction);
                                        Log.Info("Calling action group: " + kg);
                                        v.ActionGroups.SetGroup(kg, true);
                                        vm.postAbortActionCompleted = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var kg = GetActionGroup(vm.postAbortAction);
                            Log.Info("Calling action group: " + kg);
                            v.ActionGroups.SetGroup(kg, true);
                            vm.postAbortActionCompleted = true;
                        }
                    }
                }
            }
        }

        KSPActionGroup GetActionGroup(int action)
        {
            KSPActionGroup kg = 0;
            switch (action)
            {
                case 1:
                    kg = KSPActionGroup.Custom01; break;
                case 2:
                    kg = KSPActionGroup.Custom02; break;
                case 3:
                    kg = KSPActionGroup.Custom03; break;
                case 4:
                    kg = KSPActionGroup.Custom04; break;
                case 5:
                    kg = KSPActionGroup.Custom05; break;
                case 6:
                    kg = KSPActionGroup.Custom06; break;
                case 7:
                    kg = KSPActionGroup.Custom07; break;
                case 8:
                    kg = KSPActionGroup.Custom08; break;
                case 9:
                    kg = KSPActionGroup.Custom09; break;
                case 10:
                    kg = KSPActionGroup.Custom10; break;
            }
            return kg;
        }
    }
}
