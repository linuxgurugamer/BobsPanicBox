using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KSP.UI.Screens;
using ToolbarControl_NS;

namespace BobsPanicBox
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(BPB_Editor.MODID, BPB_Editor.MODNAME);
        }
    }



    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    internal class BPB_Editor : MonoBehaviour
    {
        internal static BPB_Editor Instance;

        internal bool visible = false;
        const int WIDTH = 500;
        const int HEIGHT = 200;

        Rect bpbWinRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        internal AbortValues abortValues;

        internal const string MODID = "BobsPanicBox_NS";
        internal const string MODNAME = "Bob's Panic Box";
        ToolbarControl toolbarControl;


        internal void EnableWindow(AbortValues a)
        {
            Log.Info("BPB_Editor.EnableWindow");
            if (!visible)
            {
                abortValues = a;
                visible = true;
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
                GUI.skin = HighLogic.Skin;
                bpbWinRect = GUILayout.Window(23874244, bpbWinRect, BPB_Window, "Bob's Panic Box");
            }
        }

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleOn, CloseWindow,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
                MODID,
                "bpbButton",
                "BobsPanicBox/PluginData/BPB-38",
                "BobsPanicBox/PluginData/BPB-24",
                MODNAME
            );
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
                        EnableWindow(Module_BobsPanicBox.editorInfo.av);
                        return;
                    }
                }
            }
            else
            {
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                    if (m != null)
                    {
                        Log.Info("BPB_Editor.ToggleOn");
                        EnableWindow(m.flightInfo.av);
                        return;
                    }
                }
            }
            toolbarControl.SetFalse(false);
        }

        void Start()
        {
            Instance = this;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BPB_UI_Options>().useToolbar)
            {
                if ((HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB && HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB) ||
                    (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH && HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH) ||
                    (HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().allowChangeInFlight && HighLogic.LoadedSceneIsFlight) )
                    CreateButton();
            }
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
            if(abortValues.armed && !abortValues.exceedingAoA)
                GUI.enabled = false;
            GUILayout.FlexibleSpace();
            abortValues.maxAoA = (int)GUILayout.HorizontalSlider(abortValues.maxAoA, 1f, 179f, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            if (abortValues.armed && !abortValues.exceedingAoA)
                GUI.enabled = true;

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Disable after flight time (secs):" + abortValues.disableAfter.ToString("F1"));
            GUILayout.FlexibleSpace();
            abortValues.disableAfter = GUILayout.HorizontalSlider(abortValues.disableAfter, 10f, 600f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (abortValues.actionAfterTimeout == 0)
                GUILayout.Label("Trigger action after timeout: None");
            else
                GUILayout.Label("Trigger action after timeout: Custom" + abortValues.actionAfterTimeout.ToString("D2"));
            GUILayout.FlexibleSpace();
            abortValues.actionAfterTimeout = (int)GUILayout.HorizontalSlider(abortValues.actionAfterTimeout, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();


            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (abortValues.postAbortAction == 0)
                GUILayout.Label("Trigger action after abort: None");
            else
                GUILayout.Label("Trigger action after abort: Custom" + abortValues.postAbortAction.ToString("D2"));
            GUILayout.FlexibleSpace();
            abortValues.postAbortAction = (int)GUILayout.HorizontalSlider(abortValues.postAbortAction, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Post-abort delay (secs):" + abortValues.postAbortDelay.ToString("F1"));
            GUILayout.FlexibleSpace();
            abortValues.postAbortDelay = GUILayout.HorizontalSlider(abortValues.postAbortDelay, 10f, 60f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            abortValues.delayPostAbortUntilSafe = GUILayout.Toggle(abortValues.delayPostAbortUntilSafe, "Delay post-abort until safe for chutes");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset all values to defaults", GUILayout.Width(250)))
            {
                ResetToDefault(abortValues);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }


        internal static void ResetToDefault(AbortValues abortValues)
        {
            if (HighLogic.LoadedSceneIsFlight)
                abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeAtLaunch;
            else
            {
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB)
                    abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInVAB;
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH)
                    abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options>().activeInSPH;
            }
            abortValues.vertSpeedTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().negativeVelDetection;
            abortValues.gForceTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().highGDetection;
            abortValues.vertSpeed = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().defaultNegVel;
            abortValues.exceedingAoA = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().exceedingAoA;
            abortValues.maxAoA = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().maxAoA;
            abortValues.gForceTrigger = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().defaultG;
            abortValues.explosiveTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().explosionDetection;
            abortValues.disableAfter = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().disableAfter;
            abortValues.actionAfterTimeout = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().actionAfterTimeout;
            abortValues.postAbortAction = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().postAbortAction;
            abortValues.postAbortDelay = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().postAbortDelay;
            abortValues.delayPostAbortUntilSafe = HighLogic.CurrentGame.Parameters.CustomParams<BPB_Options2>().delayPostAbortUntilSafe;
        }
    }
}
