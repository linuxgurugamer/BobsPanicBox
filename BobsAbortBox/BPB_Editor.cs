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

        Rect aaaWinRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        AbortValues abortValues;

        internal const string MODID = "BobsAbortBox_NS";
        internal const string MODNAME = "Bob's Abort Box";
        ToolbarControl toolbarControl;


        internal void EnableWindow(AbortValues a)
        {
            Log.Info("AAA_Editor.EnableWindow");
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
            abortValues.UpdateParent();
            toolbarControl.SetFalse(false);
        }


        void OnGUI()
        {
            if (visible)
            {
                GUI.skin = HighLogic.Skin;
                aaaWinRect = GUILayout.Window(23874244, aaaWinRect, BAB_Window, "Bob's Abort Box");
            }
        }
        void CreateButton()
        {
            Log.Info("AAA_Editor.CreateButton");
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleOn, CloseWindow,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
                MODID,
                "babButton",
                "BobsAbortBox/PluginData/BAB-38",
                "BobsAbortBox/PluginData/BAB-24",
                MODNAME
            );
        }
        void ToggleOn()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (var p in EditorLogic.fetch.ship.parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                    if (m != null)
                    {
                        Module_BobsAbortBox.editorInfo.av.SetParent(Module_BobsAbortBox.editorInfo, m);
                        EnableWindow(Module_BobsAbortBox.editorInfo.av);
                        return;
                    }
                }
            }
            else
            {
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    var m = p.FindModuleImplementing<Module_BobsAbortBox>();
                    if (m != null)
                    {
                        Module_BobsAbortBox.editorInfo.av.SetParent(m.flightInfo, m);
                        EnableWindow(Module_BobsAbortBox.editorInfo.av);
                        return;
                    }
                }
            }
        }

        void Start()
        {
            Instance = this;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BAB_UI_Options>().useToolbar)
                CreateButton();
        }
        void OnDestroy()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
        }
        void BAB_Window(int windowId)
        {
            GUILayout.BeginHorizontal();
            abortValues.armed = GUILayout.Toggle(abortValues.armed, "Active");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            abortValues.explosiveTriggerEnabled = GUILayout.Toggle(abortValues.explosiveTriggerEnabled, "Arm Explosion Detection");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            abortValues.vertSpeedTriggerEnabled = GUILayout.Toggle(abortValues.vertSpeedTriggerEnabled, "Neg-Vel-Limit (m/sec): " + abortValues.vertSpeed.ToString("F1"));
            GUILayout.FlexibleSpace();
            abortValues.vertSpeed = GUILayout.HorizontalSlider(abortValues.vertSpeed, -100f, -1f, GUILayout.Width(200));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            abortValues.gForceTriggerEnabled = GUILayout.Toggle(abortValues.gForceTriggerEnabled, "High-G-Limit (G): " + abortValues.gForceTrigger.ToString("F1"));
            GUILayout.FlexibleSpace();
            abortValues.gForceTrigger = GUILayout.HorizontalSlider(abortValues.gForceTrigger, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            abortValues.exceedingAoA = GUILayout.Toggle(abortValues.exceedingAoA, "Max AoA (degrees): " + abortValues.maxAoA.ToString());
            GUILayout.FlexibleSpace();
            abortValues.maxAoA = (int)GUILayout.HorizontalSlider(abortValues.maxAoA, 0f, 179f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

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

#if true



            GUILayout.BeginHorizontal();
            if (abortValues.postAbortAction == 0)
                GUILayout.Label("Trigger action after abort: None");
            else
                GUILayout.Label("Trigger action after abort: Custom" + abortValues.actionAfterTimeout.ToString("D2"));
            GUILayout.FlexibleSpace();
            abortValues.postAbortAction = (int)GUILayout.HorizontalSlider(abortValues.postAbortAction, 0f, 10f, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Post-abort delay (secs):" + abortValues.postAbortDelay.ToString("F1"));
            GUILayout.FlexibleSpace();
            abortValues.postAbortDelay = GUILayout.HorizontalSlider(abortValues.postAbortDelay, 10f, 600f, GUILayout.Width(200));
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
#endif


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
                abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeAtLaunch;
            else
            {
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.VAB)
                    abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInVAB;
                if (HighLogic.LoadedSceneIsEditor && EditorDriver.editorFacility == EditorFacility.SPH)
                    abortValues.armed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options>().activeInSPH;
            }
            abortValues.vertSpeedTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().negativeVelDetection;
            abortValues.gForceTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().highGDetection;
            abortValues.vertSpeed = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultNegVel;
            abortValues.exceedingAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().exceedingAoA;
            abortValues.maxAoA = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().maxAoA;
            abortValues.gForceTrigger = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().defaultG;
            abortValues.explosiveTriggerEnabled = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().explosionDetection;
            abortValues.disableAfter = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().disableAfter * 60f;
            abortValues.actionAfterTimeout = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().actionAfterTimeout;
            abortValues.postAbortAction = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortAction;
            abortValues.postAbortDelay = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().postAbortDelay;
            abortValues.delayPostAbortUntilSafe = HighLogic.CurrentGame.Parameters.CustomParams<BAB_Options2>().delayPostAbortUntilSafe;
        }
    }
}
