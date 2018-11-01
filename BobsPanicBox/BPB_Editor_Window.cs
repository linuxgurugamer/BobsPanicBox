using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

using KSP.UI.Screens;
using ToolbarControl_NS;
using ClickThroughFix;

namespace BobsPanicBox
{
    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    internal class BPB_Editor_Window : MonoBehaviour
    {
        internal static BPB_Editor_Window Instance;

        internal bool visible = false;
        const int WIDTH = 500;
        const int HEIGHT = 200;

        Rect bpbWinRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        internal AbortValues abortValues; // = new AbortValues();

        internal const string MODID = "BobsPanicBox_NS";
        internal const string MODNAME = "Bob's Panic Box";
        ToolbarControl toolbarControl;

       

        Vessel lastActiveVessel = null;
        BPB_VesselModule vmLastActiveVessel;
        bool buttonRed = false;
        double lastFlashTime = 0;

        const string NORMAL_BUTTON_BIG = "BobsPanicBox/PluginData/BPB-38";
        const string NORMAL_BUTTON_SML = "BobsPanicBox/PluginData/BPB-24";

        const string ALERT_BUTTON_BIG = "BobsPanicBox/PluginData/BPB-Red-38";
        const string ALERT_BUTTON_SML = "BobsPanicBox/PluginData/BPB-Red-24";

        string curSceneStr;


        internal void EnableWindow(AbortValues a)
        {
            if (!visible)
            {
                abortValues = (AbortValues)a.Clone();
                visible = true;
                if (HighLogic.LoadedSceneIsEditor)
                    curSceneStr = "Editor";
                else
                    curSceneStr = "Flight";

            }
            else
            {
                CloseWindow();
            }
        }

        void CloseWindow()
        {
            visible = false;
            abortValues.SetAllValues(abortValues);
            if (toolbarControl != null)
                toolbarControl.SetFalse(false);
        }

        void OnGUI()
        {
            if (visible)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().useKSPskin)
                    GUI.skin = HighLogic.Skin;

                bpbWinRect = ClickThruBlocker.GUILayoutWindow(23874244, bpbWinRect, BPB_Window, "Bob's Panic Box: " + curSceneStr);
            }
        }

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleOn, CloseWindow,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
                MODID,
                "bpbButton",
                NORMAL_BUTTON_BIG,
                NORMAL_BUTTON_SML,
                MODNAME
            );
        }

        void SetButtonNormal()
        {
            buttonRed = false;
            toolbarControl.SetTexture(NORMAL_BUTTON_BIG, NORMAL_BUTTON_SML);
        }

        void SetButtonRed()
        {
            buttonRed = true;
            toolbarControl.SetTexture(ALERT_BUTTON_BIG, ALERT_BUTTON_SML);
        }

        IEnumerator UpdateButton()
        {
            while (true)
            {
                if (FlightGlobals.ActiveVessel != null)
                {
                    if (lastActiveVessel != FlightGlobals.ActiveVessel)
                    {
                        lastActiveVessel = FlightGlobals.ActiveVessel;
                        vmLastActiveVessel = FlightGlobals.ActiveVessel.GetComponent<BPB_VesselModule>();
                    }
                    if (vmLastActiveVessel.armed && (!lastActiveVessel.IsControllable || lastActiveVessel.Parts.Count == 0))
                    {
                        DisableAbortSequence();
                    }
                    if (toolbarControl != null && vmLastActiveVessel.aborted && !vmLastActiveVessel.abortAcknowledged)
                    {
                        // Flash the button here
                        if (Planetarium.GetUniversalTime() - lastFlashTime > 0.5f)
                        {
                            lastFlashTime = Planetarium.GetUniversalTime();
                            if (buttonRed)
                                SetButtonNormal();
                            else
                                SetButtonRed();
                        }
                    }
                    else
                    {
                        if (vmLastActiveVessel.abortAcknowledged)
                        {
                            if (!buttonRed)
                            {
                                SetButtonRed();
                            }
                        }
                        else
                        {
                            if (buttonRed)
                            {
                                SetButtonNormal();
                            }
                        }
                    }
                }
                else
                    if (toolbarControl != null)
                        SetButtonNormal();
                yield return new WaitForSeconds(0.5f);
            }
        }

        void ToggleOn()
        {
            if (HighLogic.LoadedSceneIsEditor && EditorLogic.fetch.ship != null)
            {
                foreach (var p in EditorLogic.fetch.ship.parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    if (m != null)
                    {
                        if (m.av == null)
                            Log.Error("in ToggleOn, m.av is null");
                        EnableWindow(m.av);
                        return;
                    }
                }
            }
            else
            {
                if (vmLastActiveVessel.aborted && !vmLastActiveVessel.abortAcknowledged)
                {
                    ScreenMessages.PostScreenMessage("Abort Acknowledged");

                    vmLastActiveVessel.abortAcknowledged = true;
                    toolbarControl.SetFalse(false);
                    return;
                }
                if (vmLastActiveVessel.abortAcknowledged)
                {
                    DisableAbortSequence();
                    toolbarControl.SetFalse(false);
                    return;
                }
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    if (m != null)
                    {
                        EnableWindow(m.flightInfo.av);
                        return;
                    }
                }
            }
            toolbarControl.SetFalse(false);
        }

        void DisableAbortSequence()
        {
            ScreenMessages.PostScreenMessage("Abort Sequence Canceled");
            vmLastActiveVessel.armed = false;
            vmLastActiveVessel.aborted = false;
            vmLastActiveVessel.abortAcknowledged = false;
        }

        void Start()
        {
            Instance = this;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_UI_Options>().useToolbar)
            {
                if ((HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB && HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB) ||
                    (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH && HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH) ||
                    (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().allowChangeInFlight && HighLogic.LoadedSceneIsFlight))
                    CreateButton();
            }
            if (HighLogic.LoadedSceneIsFlight)
                StartCoroutine(UpdateButton());
        }

        void OnDestroy()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
        }

        void BPB_Window(int windowId)
        {
            if (abortValues == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("abortValues is null");
                GUILayout.EndHorizontal();
                return;
            }
            GUILayout.BeginHorizontal();
            abortValues.armed = GUILayout.Toggle(abortValues.armed, "Active");
            GUILayout.EndHorizontal();
            if (!abortValues.armed)
                GUI.enabled = false;
            
            GUILayout.BeginHorizontal();
            abortValues.explosiveTriggerEnabled = GUILayout.Toggle(abortValues.explosiveTriggerEnabled, "Arm Explosion Detection");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            abortValues.vertSpeedTriggerEnabled = GUILayout.Toggle(abortValues.vertSpeedTriggerEnabled, "Neg-Vel-Limit (m/sec): " + abortValues.vertSpeed.ToString("F1"));
            if (abortValues.armed && !abortValues.vertSpeedTriggerEnabled)
                GUI.enabled = false;
            GUILayout.FlexibleSpace();
            abortValues.vertSpeed = GUILayout.HorizontalSlider(abortValues.vertSpeed, -100f, -1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            if (abortValues.armed && !abortValues.vertSpeedTriggerEnabled)
                GUI.enabled = true;

            GUILayout.BeginHorizontal();
            abortValues.gForceTriggerEnabled = GUILayout.Toggle(abortValues.gForceTriggerEnabled, "High-G-Limit (G): " + abortValues.gForceTrigger.ToString("F1"));
            GUILayout.FlexibleSpace();
            if (abortValues.armed && !abortValues.gForceTriggerEnabled)
                GUI.enabled = false;
            abortValues.gForceTrigger = GUILayout.HorizontalSlider(abortValues.gForceTrigger, 1f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            if (abortValues.armed && !abortValues.gForceTriggerEnabled)
                GUI.enabled = true;

            GUILayout.BeginHorizontal();
            abortValues.exceedingAoA = GUILayout.Toggle(abortValues.exceedingAoA, "Max AoA (degrees): " + abortValues.maxAoA.ToString());
            if (abortValues.armed && !abortValues.exceedingAoA)
                GUI.enabled = false;
            GUILayout.FlexibleSpace();
            abortValues.maxAoA = (int)GUILayout.HorizontalSlider(abortValues.maxAoA, 1f, 179f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().useAtmoPercentage)
                GUILayout.Label("Ignore AoA above altitude (%):");
            else
                GUILayout.Label("Ignore AoA above altitude (km):");
            try
            {
                GUILayout.FlexibleSpace();
                abortValues.ignoreAoAAboveAltitudeKm = Int32.Parse(GUILayout.TextField(abortValues.ignoreAoAAboveAltitudeKm.ToString(), GUILayout.Width(50)));
            }
            catch { }
            GUILayout.Space(10);
            abortValues.ignoreAoAAboveAltitudeKm = (int)GUILayout.HorizontalSlider(abortValues.ignoreAoAAboveAltitudeKm, 1f, 100f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            if (abortValues.armed && !abortValues.exceedingAoA)
                GUI.enabled = true;

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Timeout (disable) after (secs):");
            try
            {
                GUILayout.FlexibleSpace();
                abortValues.disableAfter = Int32.Parse(GUILayout.TextField(abortValues.disableAfter.ToString(), GUILayout.Width(50)));
            }
            catch { }
            GUILayout.Space(10);
            abortValues.disableAfter = (int)GUILayout.HorizontalSlider(abortValues.disableAfter, 10f, 600f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().useAtmoPercentage)
                GUILayout.Label("Disable at altitude (%):");
            else
                GUILayout.Label("Disable at altitude (km):");
            try
            {
                GUILayout.FlexibleSpace();
                abortValues.disableAtAltitudeKm = Int32.Parse(GUILayout.TextField(abortValues.disableAtAltitudeKm.ToString(), GUILayout.Width(50)));
            }
            catch { }
            GUILayout.Space(10);
            abortValues.disableAtAltitudeKm = (int)GUILayout.HorizontalSlider(abortValues.disableAtAltitudeKm, 1f, 100f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (abortValues.actionAfterTimeout == 0)
                GUILayout.Label("Trigger action after timeout: None");
            else
                GUILayout.Label("Trigger action after timeout: Custom" + abortValues.actionAfterTimeout.ToString("D2"));
            GUILayout.FlexibleSpace();
            abortValues.actionAfterTimeout = (int)GUILayout.HorizontalSlider(abortValues.actionAfterTimeout, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max G for timeout action:");
            try
            {
                GUILayout.FlexibleSpace();
                abortValues.maxTimeoutActionG = (float)Double.Parse(GUILayout.TextField(abortValues.maxTimeoutActionG.ToString("F1"), GUILayout.Width(50)));
            }
            catch { }
            GUILayout.Space(10);
            abortValues.maxTimeoutActionG = GUILayout.HorizontalSlider(abortValues.maxTimeoutActionG, 1f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Post-abort delay (secs):");
            try
            {
                GUILayout.FlexibleSpace();
                abortValues.postAbortDelay = Int32.Parse(GUILayout.TextField(abortValues.postAbortDelay.ToString(), GUILayout.Width(50)));
            }
            catch { }
            GUILayout.Space(10);
            abortValues.postAbortDelay = (int)GUILayout.HorizontalSlider(abortValues.postAbortDelay, 10f, 60f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (abortValues.postAbortAction == 0)
                GUILayout.Label("Trigger action after abort: None");
            else
                GUILayout.Label("Trigger action after abort: Custom" + abortValues.postAbortAction.ToString("D2"));
            GUILayout.FlexibleSpace();
            abortValues.postAbortAction = (int)GUILayout.HorizontalSlider(abortValues.postAbortAction, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            abortValues.delayPostAbortUntilSafe = GUILayout.Toggle(abortValues.delayPostAbortUntilSafe, "Delay post-abort until safe for chutes");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUI.enabled = true;
            if (GUILayout.Button("Reset all values to defaults", GUILayout.Width(250)))
            {
                abortValues.ResetToDefault();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (vmLastActiveVessel.aborted)
                {
                    if (!vmLastActiveVessel.abortAcknowledged)
                    {
                        var ack = "Acknowledge";
                        if (buttonRed)
                            ack = "<color=red>Acknowledge</color>";
                        if (GUILayout.Button(ack))
                        {
                            vmLastActiveVessel.abortAcknowledged = true;
                        }
                    } else
                    {
                        if (GUILayout.Button("Disable Abort Sequence"))
                        {
                            DisableAbortSequence();
                        }
                    }
                
                }
                else
                {
                    if (!vmLastActiveVessel.Changed(abortValues))
                        GUI.enabled = false;
                    if (GUILayout.Button("Apply"))
                    {
                        //BPB_VesselModule vm = FlightGlobals.ActiveVessel.GetComponent<BPB_VesselModule>();
                        vmLastActiveVessel.SetAllValues(abortValues);
                        //vm.armed = vm.vertSpeedTriggerEnabled | vm.gForceTriggerEnabled | vm.explosiveTriggerEnabled; 
                        abortValues.SetAllValues(abortValues);
                    }
                    GUI.enabled = true;
                }
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }       
    }
}
