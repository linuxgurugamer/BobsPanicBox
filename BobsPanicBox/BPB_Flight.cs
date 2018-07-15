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

        void Start()
        {
            GameEvents.onVesselsUndocking.Add(onVesselsUndocking);
            GameEvents.onVesselCreate.Add(onVesselCreate);
        }
        void OnDestroy()
        {
            GameEvents.onVesselsUndocking.Remove(onVesselsUndocking);
            GameEvents.onVesselCreate.Remove(onVesselCreate);
        }


        void onVesselCreate(Vessel v)
        {
            var vm = v.GetComponent<BPB_VesselModule>();
            vm.missionTime = FlightGlobals.ActiveVessel.missionTime;
        }


        void onVesselsUndocking(Vessel v1, Vessel v2)
        {
            var vm1 = v1.GetComponent<BPB_VesselModule>();
            var vm2 = v2.GetComponent<BPB_VesselModule>();
            vm1.missionTime = Math.Max(v1.missionTime, v2.missionTime);
            vm2.missionTime = vm1.missionTime;
        }


    }
    partial class BPB_VesselModule : VesselModule
    {
        //Vessel lastActiveVessel;
        //BPB_VesselModule vm;

        bool timeoutInProgress = false;

        void Start2()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                Vessel v = FlightGlobals.ActiveVessel;
                //lastActiveVessel = v;
                //vm = v.GetComponent<BPB_VesselModule>();


                //                if (FlightGlobals.ActiveVessel.missionTime > vm.disableAfter)
                //if (FlightGlobals.ActiveVessel.missionTime > disableAfter)
                if (this.missionTime > disableAfter)
                {
                    //vm.armed = false;
                    armed = false;
                }
                GameEvents.onPartUndock.Add(onPartUndock);
                GameEvents.onPartUndockComplete.Add(onPartUndockComplete);
            }
        }
        void onPartUndock(Part p)
        {
            Log.Info("onPartUndock, part " + p.partInfo.title);
        }
        void onPartUndockComplete(Part p)
        {
            Log.Info("onPartUndockComplete, part " + p.partInfo.title);
        }
        void OnDestroy()
        {
            GameEvents.onPartUndock.Remove(onPartUndock);
            GameEvents.onPartUndockComplete.Remove(onPartUndockComplete);
        }


        public void FixedUpdate()
        {
            if (vesselGuid != Guid.Empty)
            {
                missionTime = Math.Max(missionTime, vessel.missionTime);
                //Log.Info("Old vesselGuid: " + vesselGuid + ", missiontime: " + missionTime);               
            }
            else
            {
                vesselGuid = this.vessel.id;
                //Log.Info("vesselGuid: " + vesselGuid);
                //Log.Info("guid missionTime: " + vessel.missionTime);
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                missionTime = vessel.missionTime;
                var vm = this;
                //Vessel v = lastActiveVessel;
                Vessel v = this.vessel;
                //if (v != lastActiveVessel)
                //{
                //    lastActiveVessel = v;
                //    vm = v.GetComponent<BPB_VesselModule>();
                //}

                //if (vm.armed && !vm.aborted && !v.ActionGroups[KSPActionGroup.Abort])
                if (armed && !aborted && !v.ActionGroups[KSPActionGroup.Abort] && missionTime > 0)
                {
                    // Check to be sure it should still be active
                    //if (v.missionTime <= vm.disableAfter && v.altitude <= vm.disableAtAltitudeKm * 1000)
                    //if (v.missionTime <= disableAfter && v.altitude <= disableAtAltitudeKm * 1000)
                    if (this.missionTime <= disableAfter && v.altitude <= disableAtAltitudeKm * 1000)
                    {
                        // check for negative vertical speed
                        //if (v.verticalSpeed < vm.vertSpeed && vm.vertSpeedTriggerEnabled)
                        if (v.verticalSpeed < vertSpeed && vertSpeedTriggerEnabled)
                        {
                            //if (this.vessel == FlightGlobals.ActiveVessel)
                            if (this.vessel.isActiveVessel)
                                ScreenMessages.PostScreenMessage("<color=red>ABORTING - Negative Vertical Velocity Detected!</color>", 10f);
                            else
                                ScreenMessages.PostScreenMessage("ABORTING - Negative Vertical Velocity Detected!");

                            ScreenMessages.PostScreenMessage(v.verticalSpeed + " m/s", 10f);
                            v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                            //vm.SetAllActive(true, false, "Aborted! Negative Vertical Velocity Detected");
                            SetAllActive(true, false, "Aborted! Negative Vertical Velocity Detected");

                        }

                        // Check for G forces too high
                        //if (v.geeForce > vm.gForceTrigger && vm.gForceTriggerEnabled)
                        if (v.geeForce > gForceTrigger && gForceTriggerEnabled)
                        {
                            //if (this.vessel == FlightGlobals.ActiveVessel)
                            if (this.vessel.isActiveVessel)
                                ScreenMessages.PostScreenMessage("<color=red>ABORTING - High G-Force Detected!</color>", 10f);
                            else
                                ScreenMessages.PostScreenMessage("ABORTING - High G-Force Detected!");

                            ScreenMessages.PostScreenMessage(v.geeForce + " Gs", 10f);
                            v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                            vm.SetAllActive(true, false, "Aborted! High G-Force Detected");

                        }

                        // Check for AoA too high.  Also make sure that the velocity is >10 to avoid the
                        // inevitable jitter before launch
                        //if (vm.exceedingAoA && v.GetSrfVelocity().magnitude > 10 && v.missionTime > 1)
                        if (vm.exceedingAoA && v.GetSrfVelocity().magnitude > 10 && missionTime > 10 && v.altitude <= vm.ignoreAoAAboveAltitude)
                        {
                            Log.Info("AoA check, velocity: " + v.GetSrfVelocity().magnitude + ", missionTime: " + missionTime + ", altitude: " + v.altitude + ", ignoreAoAAboveAltitude: " + vm.ignoreAoAAboveAltitude);
                            var v3d1 = Vector3d.Angle(v.GetTransform().up, v.GetSrfVelocity());
                            if (v3d1 > vm.maxAoA)
                            {
                                //if (this.vessel == FlightGlobals.ActiveVessel)
                                if (this.vessel.isActiveVessel)
                                    ScreenMessages.PostScreenMessage("<color=red>ABORTING - Max AoA Exceeded!</color>", 10f);
                                else
                                    ScreenMessages.PostScreenMessage("ABORTING - Max AoA Exceeded!");
                                ScreenMessages.PostScreenMessage(vm.maxAoA + " degrees", 10f);
                                v.ActionGroups.SetGroup(KSPActionGroup.Abort, true);
                                vm.SetAllActive(true, false, "Aborted! Max AoA Exceeded");
                            }

                        }
                    }
                    else
                    {
                        if (!timeoutInProgress)
                        {
                            timeoutInProgress = true;
                        }
                        if (timeoutInProgress && v.geeForce <= vm.maxTimeoutActionG)
                        {
                            vm.armed = false;
                            timeoutInProgress = false;
                            //if (v.missionTime > vm.disableAfter)
                            if (this.missionTime > vm.disableAfter)
                            {
                                //if (this.vessel == FlightGlobals.ActiveVessel)
                                if (this.vessel.isActiveVessel)
                                    ScreenMessages.PostScreenMessage("Bob's Panic Box disabled due to timeout", 10f);
                                Log.Info("Bob's Panic Box disabled due to timeout");
                            }
                            if (v.altitude > vm.disableAtAltitude)
                            {
                                //if (this.vessel == FlightGlobals.ActiveVessel)
                                if (this.vessel.isActiveVessel)
                                    ScreenMessages.PostScreenMessage("Bob's Panic Box disabled due to altitude", 10f);
                                Log.Info("Bob's Panic Box disabled due to altitude");
                            }

                            if (vm.actionAfterTimeout > 0)
                            {
                                var kg = GetActionGroup((int)vm.actionAfterTimeout);
                                v.ActionGroups.SetGroup(kg, true);
                            }
                        }
                    }
                }

                if (vm.aborted && vm.postAbortAction != 0 && !vm.postAbortActionCompleted)
                {

                    //if (vm.abortTime + vm.postAbortDelay <= v.missionTime)
                    if (vm.abortTime + vm.postAbortDelay <= this.missionTime)

                    {
                        // Check for safechute
                        // check for chutes
                        var pa = v.FindPartModulesImplementing<ModuleParachute>();
                        if (pa != null && pa.Count > 0)
                        {
                            foreach (ModuleParachute chute in pa)
                            {
                                if (chute.deploymentState == ModuleParachute.deploymentStates.STOWED && FlightGlobals.ActiveVessel.atmDensity > 0)
                                {
                                    if (chute.deploySafe == "Safe")
                                    {
                                        var kg = GetActionGroup(vm.postAbortAction);
                                        v.ActionGroups.SetGroup(kg, true);
                                        vm.postAbortActionCompleted = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var kg = GetActionGroup(vm.postAbortAction);
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
