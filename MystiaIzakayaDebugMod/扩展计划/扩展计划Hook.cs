#if Release
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Common.UI;
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
#if false
            MethodInfo method = typeof(扩展计划Hook).GetMethod("DataBaseCoreInitialize", AccessTools.all);
            Harmony.Patch((MethodBase)typeof(DataBaseCore).GetMember("Initialize")[1], null, new HarmonyMethod(method));
#endif
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
#if false
        private static void DataBaseCoreInitialize(Il2CppSystem.Collections.Generic.Dictionary<string, GameData.Core.Collections.DataBaseCore.DataBaseCoreData> __0, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.Core.Collections.DataBaseCore.LevelProperties> __1, Il2CppSystem.Collections.Generic.Dictionary<string, GameData.Core.Collections.CollabPackage> __2, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.Profile.ClothesProfile.Clothes> __3, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.Core.Collections.Decoration> __4, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.Core.Collections.Record> __5)
        {
            System.NullReferenceException: Object reference not set to an instance of an object
            at MystiaIzakayaDebugMod.扩展计划.扩展计划Manager.OnDataBaseCoreInitialize(System.String ModID, System.Collections.Generic.List`1[T] list)
            在DataBaseCore::Initialize方法执行后DataBaseLanguage未执行Initialize方法，这导致此报错
        }
        //应当HOOK此方法，但顺序让人疑惑，且此方法未来有概率改变，故放弃
        private static void DataBaseLanguageInitialize(Il2CppSystem.Collections.Generic.Dictionary<int, string> __0, Il2CppSystem.Collections.Generic.Dictionary<int, string> __1, Il2CppSystem.Collections.Generic.Dictionary<int, string> __2, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.ObjectLanguageBase> __3, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.ObjectLanguageBase> __4, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.ObjectLanguageBase> __5, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.ObjectLanguageBase> __6, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.ObjectLanguageBase> __7, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.LanguageBase> __8, Il2CppSystem.Collections.Generic.Dictionary<string, GameData.CoreLanguage.LanguageBase> __9, Il2CppSystem.Collections.Generic.Dictionary<string, GameData.CoreLanguage.LanguageBase> __10, Il2CppSystem.Collections.Generic.Dictionary<GameData.Core.Collections.Cooker.CookerType, GameData.CoreLanguage.ObjectLanguageBase> __11, Il2CppSystem.Collections.Generic.Dictionary<int, GameData.CoreLanguage.LanguageBase> __12, Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.ValueTuple<string, string, string, string>> __13, Il2CppSystem.Collections.Generic.Dictionary<int, UnhollowerBaseLib.Il2CppStringArray> __14, Il2CppSystem.Collections.Generic.Dictionary<int, UnhollowerBaseLib.Il2CppStringArray> __15, Il2CppSystem.Collections.Generic.Dictionary<NightScene.EventUtility.EventManager.BuffType, GameData.CoreLanguage.ObjectLanguageBase> __16, UnityEngine.Sprite __17, UnityEngine.Sprite __18, Il2CppSystem.Collections.Generic.Dictionary<int, UnityEngine.Sprite> __19, Il2CppSystem.Collections.Generic.Dictionary<int, UnityEngine.Sprite> __20, Il2CppSystem.Collections.Generic.Dictionary<GameData.Profile.SchedulerNodeCollection.MissionNode.FinishCondition.ConditionType, string> __21, Il2CppSystem.Collections.Generic.Dictionary<GameData.Core.Collections.DaySceneUtility.Collections.Product.ProductType, string> __22, Il2CppSystem.Collections.Generic.Dictionary<GameData.Profile.PartnerBase.PartnerType, GameData.CoreLanguage.LanguageBase> __23, Il2CppSystem.Collections.Generic.Dictionary<string, GameData.CoreLanguage.LanguageBase> __24, Il2CppSystem.Collections.Generic.Dictionary<string, string> __25, Il2CppSystem.Collections.Generic.Dictionary<int, UnhollowerBaseLib.Il2CppReferenceArray<GameData.CoreLanguage.LanguageBase>> __26)
        {
        }
#endif
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