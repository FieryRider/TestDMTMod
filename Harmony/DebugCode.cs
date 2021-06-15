using DMT;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Xml;

public class DebugCode
{
    public class DebugCode_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log("Loading Patch: " + GetType().ToString());
            Harmony harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(XUiFromXml), "parseController")]
    class XUiFromXmlController
    {
        static void Postfix(XmlNode _node, XUiView _viewComponent, XUiController _parent)
        {
            XmlElement xmlElement = (XmlElement)_node;
            XUi xui = _viewComponent.xui;
            if (xmlElement.HasAttribute("controller"))
            {
                string text = xmlElement.GetAttribute("controller");
                if (!text.StartsWith("XUiC_"))
                {
                    text = string.Format("XUiC_{0}", text);
                }
                if (text == "XUiC_ParallelCraftingQueue")
                {
                    //Type type = AccessTools.TypeByName(text);
                    //if (type == null)
                    //    Log.Warning($"Not found! {text}");
                    //else
                    //    Log.Out($"Found!!!!!!!!! {type.Name}");

                    Log.Warning(_viewComponent.Controller.GetType().Name);
                }
            }
        }
    }
}
