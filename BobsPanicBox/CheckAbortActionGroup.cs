using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using ClickThroughFix;
using KSP.Localization;
using KSP.UI.Dialogs;
using KSP.UI.Screens;
using UnityEngine;

#if false
namespace BobsPanicBox
{
    public class CheckAbortActionGroup
    {

        /// <summary>
        /// Returns true if the provided <see cref="BaseAction"/> is assigned to the action group.
        /// </summary>
        /// <param name="group">The action group to check.</param>
        /// <param name="action">The base action to look for.</param>
        /// <returns>True if the <see cref="KSPActionGroup"/> contains the <see cref="BaseAction"/>.</returns>
        public static bool ContainsAction(KSPActionGroup group, BaseAction action)
        {
            return action == null ? false : (action.actionGroup & group) == group;
        }

        /// <summary>
        /// Returns a list of action groups that the provided base action belongs to.
        /// </summary>
        /// <param name="action">The base action to find action groups for.</param>
        /// <returns>A list of action groups the base action is assigned to.</returns>
        public static List<KSPActionGroup> GetActionGroupList(BaseAction action)
        {
            var ret = new List<KSPActionGroup>();
            var groups = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];

            foreach (KSPActionGroup group in groups)
            {
                if (group == KSPActionGroup.None || group == KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    continue;
                }

                if (ContainsAction(group, action))
                {
                    ret.Add(group);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of base actions that exist for the provided part.
        /// </summary>
        /// <param name="part">The part to get base actions from.</param>
        /// <returns>The base actions available to the part.</returns>
        public static List<BaseAction> FromParts(Part part)
        {
            var partList = new List<BaseAction>();
            if (part != null)
            {
                // Add BaseActions in the part
                foreach (BaseAction action in part.Actions)
                {
                    partList.Add(action);
                }

                // Add BaseActions in the part modules.
                foreach (PartModule module in part.Modules)
                {
                    foreach (BaseAction action in module.Actions)
                    {
                        partList.Add(action);
                    }
                }
            }

            return partList;
        }

        static string  getGroupListBaseAction(BaseAction action)
        {
            string content = "";
            foreach (KSPActionGroup group in GetActionGroupList(action))
            {
            
                // Configure the button
                if (true)//VisualUi.UiSettings.TextActionGroupButtons)
                {
                    content += "Action is linked to " + group.ToString() + ".\n";

                }
            }
            return content;
        }

        public static void getAllActions()
        {
            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                foreach (BaseAction action in FromParts(part))
                {
                    var s = getGroupListBaseAction(action);
                    if (s!="")
                        Log.Info("Part: " + part.partInfo.title + ", action:" + s);
                }
            }
        }
    }
}
#endif