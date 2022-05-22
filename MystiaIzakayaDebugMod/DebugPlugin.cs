#if DEBUG
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using DEYU.Utils;
using GameData.Core.Collections.NightSceneUtility;
using GameData.CoreLanguage.Collections;
using GameData.Profile;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using NightScene.CookingUtility;
using NightScene.EventUtility;
using NightScene.GuestManagementUtility;
using NightScene.UI.GuestManagementUtility;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;

namespace MystiaIzakayaDebugMod
{
    #region 一些扩展
    public static class Tools
    {
        public static string ArrayToString<T>(this T[] Array)
        {
            if (Array.Length == 0)
            {
                return "Null";
            }
            return string.Join(", ", Array.Select((T x) => x.ToString()).ToArray());
        }
        public static string ArrayToString<T>(this T[] Array, System.Func<T, string> func)
        {
            if (Array.Length == 0)
            {
                return "Null";
            }
            return string.Join(", ", Array.Select((T x) => func(x)).ToArray());
        }
        public static string ListToString<T>(this List<T> List)
        {
            if (List.Count == 0)
            {
                return "Null";
            }
            return string.Join(", ", List.ToArray().Select((T x) => x.ToString()).ToArray());
        }
    }
    #endregion
    [BepInPlugin("MystiaIzakayaDebugMod", "MystiaIzakayaDebugMod", "1.0.0")]
    public class DebugPlugin : BasePlugin
    {
        public Harmony Harmony { get; } = new Harmony("MystiaIzakayaDebugMod");
        public static Regex intregex = new Regex("[0-9]+");
        public override void Load()
        {
            #region 不需要关注的Init项
            DebugPlugin.__instance = this;
            DebugPlugin.ActiveDLCLabelList = new List<string>();
            DebugPlugin.ActiveDLCLabelList.Add("CORE");
            DebugPlugin.ActiveDLCLabelList.Add("DLC1");
            DebugPlugin.ActiveDLCLabelList.Add("DLC2");
            DebugPlugin.debug = base.Config.Bind<bool>("Config", "debug", true, "调试");
            DebugPlugin.skipop = base.Config.Bind<bool>("Config", "skipop", true, "跳过启动游戏时的动画");
            DebugPlugin.quicklyload = base.Config.Bind<bool>("Config", "quicklyload", false, "自动点击主菜单的“继续”");
            #endregion
            try
            {
                MethodInfo method = typeof(DebugPlugin).GetMethod("Awake", AccessTools.all);
                this.Harmony.Patch(typeof(SplashScene.SceneManager).GetMethod("Awake", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
                method = typeof(DebugPlugin).GetMethod("MainSceneAwake", AccessTools.all);
                this.Harmony.Patch(typeof(MainScene.SceneManager).GetMethod("Awake", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
                method = typeof(DebugPlugin).GetMethod("get_ActiveDLCLabel", AccessTools.all);
                this.Harmony.Patch(typeof(GameDataProfile).GetMethod("get_ActiveDLCLabel", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
                method = typeof(DebugPlugin).GetMethod("PushToOrder", AccessTools.all);
                this.Harmony.Patch(typeof(GuestGroupController).GetMethod("PushToOrder", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
                method = typeof(DebugPlugin).GetMethod("Open", AccessTools.all);
                this.Harmony.Patch(typeof(ServeModuleUI).GetMethod("Open", AccessTools.all, null, new System.Type[] { typeof(bool), typeof(GuestsManager.OrderBase), typeof(Il2CppSystem.Action), typeof(Il2CppSystem.Action<int>), typeof(bool), typeof(Il2CppSystem.Action<UnityEngine.Sprite>), typeof(Il2CppSystem.Action<UnityEngine.Sprite>), typeof(NightScene.GuestManagementUtility.GuestGroupController) }, null), null, new HarmonyMethod(method), null, null, null);
                method = typeof(DebugPlugin).GetMethod("LoseAllCombo", AccessTools.all);
                this.Harmony.Patch(typeof(EventManager).GetMethod("LoseAllCombo", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
                method = typeof(DebugPlugin).GetMethod("SceneManagerStart", AccessTools.all);
                this.Harmony.Patch(typeof(NightScene.SceneManager).GetMethod("Start", AccessTools.all), null, new HarmonyMethod(method), null, null, null);
            }
            catch
            {
            }
        }
        #region 修改评价

        public static bool LoseAllCombo(EventManager __instance)
        {
            return false;
        }
        //热火朝天
        public static void Fever()
        {
            MonoSingleton<QTERewardManager>.Instance.Player_ThrowDeliver();
            MonoSingleton<EventManager>.Instance.CookTimeEditByTag(-20, 0, 9999, out _);
            MonoSingleton<EventManager>.Instance.ComboEdit(100);
            MonoSingleton<EventManager>.Instance.PassionEdit(100);
            MonoSingleton<EventManager>.Instance.Fever(99999, out _);
            MonoSingleton<EventManager>.Instance.MinEvalLevelSet(9999, 9, null, out _);
        }
        public static void SceneManagerStart(NightScene.SceneManager __instance)
        {
            Fever();
        }
        #endregion

        public static StringBuilder GetNormalGuest(IzakayaProfile __instance)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var izakaya in __instance.izakayas)
            {
                sb.AppendLine(izakaya.Text.Name);
                float all = 0f;
                int Length = 0;
                foreach (var normalGuest in izakaya.NormalGuestPool)
                {
                    foreach (var x in Il2CppSystem.Linq.Enumerable.ToList(normalGuest.Expanded))
                    {
                        foreach (var x2 in Il2CppSystem.Linq.Enumerable.ToList(x))
                        {
                            NormalGuest normalGuest1 = x2;
                            float FundRangeMin = izakaya.BaseFundRange.x * x2.fundMultiplier;
                            float FundRangeMax = izakaya.BaseFundRange.y * x2.fundMultiplier;

                            sb.Append(" " + normalGuest1.Text.BriefName).AppendLine(FundRangeMin.ToString() + "-" + FundRangeMax.ToString());
                            all += (FundRangeMin + FundRangeMax) / 2;
                            Length++;
                        }
                    }
                }
                sb.AppendLine(" 平均:" + (all / Length).ToString());
            }
            return sb;
        }
        #region 稀客订单推送
        public static void PushToOrder(NightScene.GuestManagementUtility.GuestGroupController __instance, NightScene.GuestManagementUtility.GuestsManager.OrderBase __0)
        {
            if (__0.Type == GuestsManager.OrderBase.OrderType.Special)
            {
                var SpecialGuests = __instance.Cast<SpecialGuestsController>();
                var 食物Tag需求 = __0.foodRequest;
                var 酒水Tag需求 = __0.beverageRequest;
                int[] 所有喜好酒水Tag = SpecialGuests.LikeBevTags;
                int[] 所有厌恶酒水Tag = SpecialGuests.HateBevTags;
                int[] 所有喜好食物Tag = SpecialGuests.LikeFoodTags;
                int[] 所有厌恶食物Tag = SpecialGuests.HateFoodTags;
                var 现有余额 = SpecialGuests.GetFund;
                var 名字 = __instance.OnGetGuestName();
                StringBuilder sb = new StringBuilder();
                sb.Append("我叫" + 名字).AppendLine("我还可以消费:" + 现有余额.ToString());
                sb.AppendLine("我需求这个食物Tag:" + 食物Tag需求.GetFoodTag());
                sb.AppendLine("我喜好这些食物Tag:" + 所有喜好食物Tag.ArrayToString((int x) => DataBaseLanguage.FoodTags[x]));
                sb.AppendLine("我厌恶这些食物Tag:" + 所有厌恶食物Tag.ArrayToString((int x) => DataBaseLanguage.FoodTags[x]));
                sb.AppendLine("我需求这个酒水Tag:" + 酒水Tag需求.GetBeverageTag());
                sb.AppendLine("我喜好这些酒水Tag:" + 所有喜好酒水Tag.ArrayToString((int x) => DataBaseLanguage.BeverageTags[x]));
                sb.AppendLine("我厌恶这些酒水Tag:" + 所有厌恶酒水Tag.ArrayToString((int x) => DataBaseLanguage.BeverageTags[x]));
                SpecialGuestOrder[SpecialGuests.DeskCode] = sb.ToString();
            }
        }
        public static Dictionary<int, string> SpecialGuestOrder = new Dictionary<int, string>();
        public static void Open(NightScene.UI.GuestManagementUtility.ServeModuleUI __instance, bool __0, NightScene.GuestManagementUtility.GuestsManager.OrderBase __1, Il2CppSystem.Action __2, Il2CppSystem.Action<int> __3, bool __4, Il2CppSystem.Action<UnityEngine.Sprite> __5, Il2CppSystem.Action<UnityEngine.Sprite> __6, NightScene.GuestManagementUtility.GuestGroupController __7)
        {
            if (__1.Type == GuestsManager.OrderBase.OrderType.Special)
            {
                __instance.specialRequest.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                __instance.specialRequest.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().m_text = SpecialGuestOrder[__1.DeskCode];
            }
        }
        #endregion


        public static void SpawnSpecialGuest(int id)
        {
            MonoSingleton<EventManager>.Instance.SpawnSpecialGuest(id);
        }
        #region DebugHookloadMethod
        public static void Awake(MethodBase __originalMethod, SplashScene.SceneManager __instance)
        {
            if (DebugPlugin.debug.Value)
            {
                SplashScene.SceneManager.LaunchConfigData.LaunchMode = SplashScene.SceneManager.ConsoleMode.Full;
                __instance.userName = "Nephthys";
                __instance.password = "2166085463";
            }
            if (DebugPlugin.skipop.Value || DebugPlugin.debug.Value)
            {
                __instance.LoadScene();
            }
        }
        public static ConfigEntry<bool> debug;

        public static ConfigEntry<bool> skipop;

        public static ConfigEntry<bool> quicklyload;

        public static List<string> ActiveDLCLabelList;
        public static void MainSceneAwake(MethodBase __originalMethod, MainScene.SceneManager __instance)
        {
            if (DebugPlugin.quicklyload.Value)
            {
                __instance.lastSelection.onSubmitAction.Invoke();
            }
        }
        public static GameDataProfile GameDataProfile;
        public static bool get_ActiveDLCLabel(MethodBase __originalMethod, GameDataProfile __instance, ref List<string> __result)
        {
            GameDataProfile = __instance;
            __result = DebugPlugin.ActiveDLCLabelList;
            return !DebugPlugin.debug.Value;
        }
        public static List<int> NotShouldSpawnSpecialGuests = new List<int>();
        public static List<int> NotShouldSpawnNormalGuests = new List<int>();
        #endregion
        public static DebugPlugin __instance;
    }
}
#endif