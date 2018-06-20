using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        internal float disableAfter;
        internal int actionAfterTimeout;

        internal int postAbortAction;
        internal float postAbortDelay;
        internal bool delayPostAbortUntilSafe;

        Flight flightParent = null;
        Editor editorParent = null;

        Module_BobsAbortBox moduleBAB = null;

        internal AbortValues()
        { }
        internal void SetParent(Flight f, Module_BobsAbortBox m)
        {
            flightParent = f;
            moduleBAB = m;
        }
        internal void SetParent(Editor e, Module_BobsAbortBox m)
        {
            editorParent = e;
            moduleBAB = m;
        }


        internal void UpdateParent()
        {
            Log.Info("UpdateParent");
            moduleBAB.armed = armed;
            moduleBAB.status = status;
            moduleBAB.vertSpeedTriggerEnabled = vertSpeedTriggerEnabled;
            moduleBAB.vertSpeed = vertSpeed;
            moduleBAB.gForceTriggerEnabled = gForceTriggerEnabled;
            moduleBAB.gForceTrigger = gForceTrigger;
            moduleBAB.exceedingAoA = exceedingAoA;
            moduleBAB.explosiveTriggerEnabled = explosiveTriggerEnabled;
            moduleBAB.disableAfter = disableAfter;
            moduleBAB.actionAfterTimeout = actionAfterTimeout;

            if (flightParent != null)
                flightParent.SaveCurrent(moduleBAB);
            if (editorParent != null)
                editorParent.SaveCurrent(moduleBAB);


            //moduleBAB.SetActiveGUI();
            //moduleBAB.SetVertSpeedGUI();
            //moduleBAB.SetGForceGUI();
            //moduleBAB.SetExplosionGUI();

        }
        public bool Changed(Module_BobsAbortBox m)
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
                actionAfterTimeout != m.actionAfterTimeout;

        }
        public void SaveCurrent(Module_BobsAbortBox m)
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
            }
        }
    }

    internal class Flight
    {
        internal AbortValues av;

        internal Flight()
        {
            av = new AbortValues();
        }
        public void SaveCurrent(Module_BobsAbortBox m)
        {
            Log.Info("Flight.SaveCurrent");
            av.SaveCurrent(m);
        }
        public bool Changed(Module_BobsAbortBox m)
        {
            return av.Changed(m);
        }
        public void SetAllVertSpeed(Vessel vessel, bool enabled, float vspeed)
        {
            Log.Info("Flight, SetAllVertSpeed: enabled: " + enabled + ", vspeed: " + vspeed);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.vertSpeedTriggerEnabled = enabled;
                    m.vertSpeed = vspeed;
                    //m.SetVertSpeedGUI();
                }
            }
        }

        public void SetAllgForce(Vessel vessel, bool enabled, float gForceTrigger)
        {
            Log.Info("Editor, SetAllgForce: enabled: " + enabled + ", gForceTrigger: " + gForceTrigger);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.gForceTriggerEnabled = enabled;
                    m.gForceTrigger = gForceTrigger;
                    //m.SetGForceGUI();
                }
            }
        }
        public void SetAllMaxAoA(Vessel vessel, bool enabled, int maxAoA)
        {
            Log.Info("Editor, SetAllMaxAoA: enabled: " + enabled + ", maxAoA: " + maxAoA);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.exceedingAoA = enabled;
                    m.maxAoA = maxAoA;
                    //m.SetGForceGUI();
                }
            }
        }
        public void SetAllExplosionDetection(Vessel vessel, bool explosiveTriggerEnabled)
        {
            Log.Info("Editor, SetAllExplosionDetection:  explosiveTriggerEnabled: " + explosiveTriggerEnabled);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.explosiveTriggerEnabled = explosiveTriggerEnabled;
                    //m.SetExplosionGUI();
                }
            }
        }

        public void SetDisableAfter(Vessel vessel, float disableAfter)
        {
            Log.Info("Editor, SetDisableAfter:  disableAfter: " + disableAfter);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.disableAfter = disableAfter;
                }
            }
        }
        public void SetActionAfterTimeout(Vessel vessel, int actionAfterTimeout)
        {
            Log.Info("Editor, SetActionAfterTimeout:  actionAfterTimeout: " + actionAfterTimeout);

            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.actionAfterTimeout = actionAfterTimeout;
                }
            }
        }


        public void SetPostAbortAction(Vessel vessel, int postAbortAction)
        {
            foreach (var p in vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.postAbortAction = postAbortAction;
                }
            }
        }
        public void SetPostAbortDelay(Vessel vessel, float postAbortDelay)
        {
            foreach (var p in vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.postAbortDelay = postAbortDelay;
                }
            }
        }
        public void SetDelayPostAbortUntilSafe(Vessel vessel, bool delayPostAbortUntilSafe)
        {
            foreach (var p in vessel.Parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                }
            }
        }



    }

    internal class Editor
    {
        internal AbortValues av;

        internal Editor()
        {
            av = new AbortValues();
        }

        public void SaveCurrent(Module_BobsAbortBox m)
        {
            Log.Info("Editor.SaveCurrent");
            av.SaveCurrent(m);
        }
        public bool Changed(Module_BobsAbortBox m)
        {
            return av.Changed(m);
        }

        public void CopyOrInit(Module_BobsAbortBox aaa)
        {
            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                if (p != aaa.part)
                {
                    var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                    if (m != null)
                    {
                        aaa.armed = m.armed;
                        aaa.status = m.status;
                        aaa.vertSpeedTriggerEnabled = m.vertSpeedTriggerEnabled;
                        aaa.vertSpeed = m.vertSpeed;
                        aaa.gForceTriggerEnabled = m.gForceTriggerEnabled;
                        aaa.gForceTrigger = m.gForceTrigger;
                        aaa.exceedingAoA = m.exceedingAoA;
                        aaa.maxAoA = m.maxAoA;
                        aaa.explosiveTriggerEnabled = m.explosiveTriggerEnabled;
                        aaa.disableAfter = m.disableAfter;
                        aaa.actionAfterTimeout = m.actionAfterTimeout;

                        aaa.postAbortAction = m.postAbortAction;
                        aaa.postAbortDelay = m.postAbortDelay;
                        aaa.delayPostAbortUntilSafe = m.delayPostAbortUntilSafe;

                        Log.Info("Found existing command pod, copying values");

                        SaveCurrent(aaa);
                        return;
                    }
                }
            }

            Log.Info("No previous command pod found, initting values");

            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB && HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB)
                aaa.armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH && HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInSPH)
                aaa.armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
            aaa.vertSpeedTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().negativeVelDetection;
            aaa.gForceTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().highGDetection;
            aaa.vertSpeed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultNegVel;
            aaa.exceedingAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().exceedingAoA;
            aaa.maxAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().maxAoA;
            aaa.gForceTrigger = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultG;
            aaa.explosiveTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().explosionDetection;
            aaa.disableAfter = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().disableAfter * 60f;
            aaa.actionAfterTimeout = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().actionAfterTimeout;
            aaa.postAbortAction = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortAction;
            aaa.postAbortDelay = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortDelay;
            aaa.delayPostAbortUntilSafe = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().delayPostAbortUntilSafe;
        }
       
        public void SetAllActive(bool armed, string status)
        {
            Log.Info("SetAllActive, armed: " + armed + ", status: " + status);

            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.armed = armed;
                    m.status = status;
                    //m.SetActiveGUI();
                }
            }
        }

        public void SetAllVertSpeed(bool enabled, float vspeed)
        {
            Log.Info("Editor, SetAllVertSpeed: enabled: " + enabled + ", vspeed: " + vspeed);

            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.vertSpeedTriggerEnabled = enabled;
                    m.vertSpeed = vspeed;
                    //m.SetVertSpeedGUI();
                }
            }
        }

        public void SetAllgForce(bool enabled, float gForceTrigger)
        {
            Log.Info("Editor, SetAllgForce: enabled: " + enabled + ", gForceTrigger: " + gForceTrigger);

            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.gForceTriggerEnabled = enabled;
                    m.gForceTrigger = gForceTrigger;
                    //m.SetGForceGUI();
                }
            }
        }
        public void SetAllMaxAoA(bool enabled, int maxAoA)
        {
            Log.Info("Editor, SetAllMaxAoA: enabled: " + enabled + ", maxAoA: " + maxAoA);

            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.exceedingAoA = enabled;
                    m.maxAoA = maxAoA;
                   // m.SetGForceGUI();
                }
            }
        }
        public void SetAllExplosionDetection(bool explosiveTriggerEnabled)
        {
            Log.Info("Editor, SetAllExplosionDetection:  explosiveTriggerEnabled: " + explosiveTriggerEnabled);

            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.explosiveTriggerEnabled = explosiveTriggerEnabled;
                    //m.SetExplosionGUI();
                }
            }
        }
        public void SetDisableAfter(float disableAfter)
        {
            Log.Info("Editor, SetDisableAfter:  disableAfter: " + disableAfter);

            foreach (var p in EditorLogic.fetch.ship.parts)
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
            Log.Info("Editor, SetActionAfterTimeout:  actionAfterTimeout: " + actionAfterTimeout);

            foreach (var p in EditorLogic.fetch.ship.parts)
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
            foreach (var p in EditorLogic.fetch.ship.parts)
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
            foreach (var p in EditorLogic.fetch.ship.parts)
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
            foreach (var p in EditorLogic.fetch.ship.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                if (m != null)
                {
                    m.delayPostAbortUntilSafe = delayPostAbortUntilSafe;
                }
            }
        }



    }
    public class Module_BobsAbortBox : PartModule
    {
        internal BPB_VesselModule vm;

        static internal Editor editorInfo = new Editor();
        internal Flight flightInfo;

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "Bob's Abort Box Window")]
        public void openAutoAbort()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                editorInfo.av.SetParent(editorInfo, this);
                BPB_Editor.Instance.EnableWindow(editorInfo.av);
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                editorInfo.av.SetParent(flightInfo, this);
                BPB_Editor.Instance.EnableWindow(flightInfo.av);
            }
        }


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


        /// <summary>
        /// Vertical speed toggle
        /// </summary>
        [KSPField(isPersistant = true)]
        internal bool vertSpeedTriggerEnabled = false;
        [KSPField(isPersistant = true)]
        internal float vertSpeed = -40;

        

        /// <summary>
        /// High G Force toggle
        /// </summary>

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


        void OnDestroy()
        {
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorShipModified.Remove(onEditorShipModified);
            //if (HighLogic.LoadedSceneIsFlight)
            //    GameEvents.onVesselStandardModification.Remove(onVesselWasModified);
        }

        void onEditorShipModified(ShipConstruct sc)
        {
            if (editorInfo.Changed(this))
            {
                Log.Info("onEditorShipModified, part: " + part.partInfo.title);
                editorInfo.SaveCurrent(this);
                editorInfo.SetAllVertSpeed(vertSpeedTriggerEnabled, vertSpeed);
                editorInfo.SetAllgForce(gForceTriggerEnabled, gForceTrigger);
                editorInfo.SetAllMaxAoA(exceedingAoA, maxAoA);
                editorInfo.SetAllExplosionDetection(explosiveTriggerEnabled);
                editorInfo.SetDisableAfter(disableAfter);
                editorInfo.SetActionAfterTimeout(actionAfterTimeout);

                editorInfo.SetPostAbortAction(postAbortAction);
                editorInfo.SetPostAbortDelay(postAbortDelay);
                editorInfo.SetDelayPostAbortUntilSafe(delayPostAbortUntilSafe);
            }
        }


        void LateUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (flightInfo != null && flightInfo.Changed(this))
                {
                    Log.Info("LateUpdate, part: " + part.partInfo.title);
                    flightInfo.SaveCurrent(this);
                    flightInfo.SetAllVertSpeed(this.vessel, vertSpeedTriggerEnabled, vertSpeed);
                    flightInfo.SetAllgForce(this.vessel, gForceTriggerEnabled, gForceTrigger);
                    flightInfo.SetAllMaxAoA(this.vessel, exceedingAoA, maxAoA);
                    flightInfo.SetAllExplosionDetection(this.vessel, explosiveTriggerEnabled);
                    flightInfo.SetDisableAfter(this.vessel, disableAfter);
                    flightInfo.SetActionAfterTimeout(this.vessel, actionAfterTimeout);

                    flightInfo.SetPostAbortAction(this.vessel, postAbortAction);
                    flightInfo.SetPostAbortDelay(this.vessel, postAbortDelay);
                    flightInfo.SetDelayPostAbortUntilSafe(this.vessel, delayPostAbortUntilSafe);

                }
            }
        }

        void Start()
        {
            Log.Info("Module_BobsAbortBox" +
                ".Start, LoadedScene: " + HighLogic.LoadedScene + ", initted: " + initted);
            if (HighLogic.LoadedSceneIsFlight)
            {
                vm = this.vessel.GetComponent<BPB_VesselModule>();
            }
            else
                GameEvents.onEditorShipModified.Add(onEditorShipModified);

            if (!initted)
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    editorInfo.CopyOrInit(this);
                }
                else
                {
                    armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeAtLaunch;
                    vertSpeedTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().negativeVelDetection;
                    gForceTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().highGDetection;
                    vertSpeed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultNegVel;
                    exceedingAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().exceedingAoA;
                    maxAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().maxAoA;
                    gForceTrigger = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultG;
                    explosiveTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().explosionDetection;
                    disableAfter = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().disableAfter * 60f;
                    actionAfterTimeout = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().actionAfterTimeout;
                    postAbortAction = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortAction;
                    postAbortDelay = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortDelay;
                    delayPostAbortUntilSafe = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().delayPostAbortUntilSafe;
                }
                initted = true;
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                flightInfo = new Flight();
                flightInfo.SaveCurrent(this);
            }
            //SetActiveGUI();
            //SetVertSpeedGUI();
            //SetGForceGUI();
            //SetExplosionGUI();
        }

        internal static bool IsInitiallyActive()
        {
            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB && HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB)
                return HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
            if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH && HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInSPH)
                return HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
            if (HighLogic.LoadedSceneIsFlight && HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeAtLaunch)
                return HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
            return false;
        }
    }
}