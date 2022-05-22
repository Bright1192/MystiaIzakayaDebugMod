#if Release
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using DEYU.Utils;
using GameData.Core.Collections;
using GameData.Core.Collections.NightSceneUtility;
using GameData.CoreLanguage;
using GameData.CoreLanguage.Collections;
using GameData.Profile;
using GameData.RunTime.Common;
using GameData.RunTime.NightSceneUtility;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using MystiaIzakayaDebugMod;
using NightScene.CookingUtility;
using NightScene.EventUtility;
using NightScene.GuestManagementUtility;
using NightScene.UI.GuestManagementUtility;
using Omt.Serializations;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using UnhollowerBaseLib;
using UnityEngine;
using static NightScene.EventUtility.EventManager;
using Directory = System.IO.Directory;
using File = System.IO.File;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;

/*==========================
* 创建时间: 2022/5/17 21:45:03
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.扩展计划
{
    public static class 扩展计划Hook
    {
        static 扩展计划Hook()
        {
            Harmony Harmony = new Harmony("扩展计划Hook");
            Harmony.PatchAll(typeof(扩展计划Hook));
        }

        public static void Init()
        {

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameDataProfile), "ActiveDLCLabel", MethodType.Getter)]
        private static void get_ActiveDLCLabel(MethodBase __originalMethod, GameDataProfile __instance, ref List<string> __result)
        {
            foreach (var Mod in 扩展计划Manager.Mods)
            {
                __result.Add(Mod.modid);
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DayScene.SceneManager), "Awake")]
        private static void AwakePostfix(DayScene.SceneManager __instance)
        {
            foreach (var mod in 扩展计划.扩展计划Manager.Mods)
            {
                扩展计划.扩展计划Manager.AddRecipes(mod.modid, mod.菜单和食物list);
            }
        }
        
        public static string DictionaryToString<T, T2>(this Dictionary<T, T2> Dic, System.Func<T, string> func = null, System.Func<T2, string> func2 = null)
        {
            if (Dic.Count == 0)
            {
                return "Null";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var dic in Dic)
            {
                sb.Append(func == null ? dic.key.ToString() : func(dic.key)).Append(":").AppendLine(func2 == null ? dic.value.ToString() : func2(dic.value));

            }
            return sb.ToString();
        }
    }

}
#endif