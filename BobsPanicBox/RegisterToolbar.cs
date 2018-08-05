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
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(BPB_Editor_Window.MODID, BPB_Editor_Window.MODNAME);
        }
    }
}