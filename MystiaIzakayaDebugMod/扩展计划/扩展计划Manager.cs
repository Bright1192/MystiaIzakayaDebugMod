#if Release
using BepInEx;
using BepInEx.IL2CPP;
using Common.UI;
using DEYU.Utils;
using GameData.Core.Collections;
using GameData.CoreLanguage.Collections;
using HarmonyLib;
using MystiaIzakayaDebugMod.IL2CPPUnityCall;
using Omt.Serializations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Directory = System.IO.Directory;
using DirectoryInfo = System.IO.DirectoryInfo;
using File = System.IO.File;
using Path = System.IO.Path;

/*==========================
* 创建时间: 2022/5/17 20:35:21
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.扩展计划
{

    [BepInPlugin("MystiaIzakayaBasemod", "MystiaIzakayaBasemod", "1.0.0")]
    public class 扩展计划Manager : BasePlugin
    {
        public override void Load()
        {
            扩展计划Hook.Init();
            DebugMono.Init();
        }

        public static void Serialize<T>(T o, string ModPath)
        {
            TextWriter textWriter = new StreamWriter(ModPath);
            new XmlSerializer(typeof(T)).Serialize(textWriter, o);
            textWriter.Close();
        }
        public static T Deserialize<T>(string ModPath)
        {
            TextReader textWriter = new StreamReader(ModPath);
            T t = (T)new XmlSerializer(typeof(T)).Deserialize(textWriter);
            textWriter.Close();
            return t;
        }
        public static void AddRecipes(string ModID, List<菜单和食物> list)
        {
            StringBuilder modinitlog = new StringBuilder();
            modinitlog.AppendLine("[MystiaIzakayaBasemod]开始加载" + ModID);
            modinitlog.AppendLine("//////////////////////////");
            try
            {
                foreach (var 菜单和食物 in list)
                {
                    bool hasfood = DataBaseLanguage.Foods.TryAdd(菜单和食物.ID, new GameData.CoreLanguage.ObjectLanguageBase(菜单和食物.Name, 菜单和食物.Description, 菜单和食物.FoodSprite));
                    bool hassell = DataBaseCore.Foods.TryAdd(菜单和食物.ID, new Sellable(菜单和食物.ID, 菜单和食物.Money, 菜单和食物.Level, 菜单和食物.Tags.ToArray(), 菜单和食物.BanTags.ToArray(), Sellable.SellableType.Food, new Il2CppSystem.Collections.Generic.List<int>(), false));
                    bool hasrec = DataBaseCore.Recipes.TryAdd(菜单和食物.ID, new Recipe(菜单和食物.ID, 菜单和食物.ID, 菜单和食物.CookerType, 菜单和食物.CookTime, 菜单和食物.Ingredients.ToArray()));
                    bool hasdlcmap = DataBaseCore.RecipesMapping.TryAdd(菜单和食物.ID, ModID);
                    bool _continue = true;
                    for (int i = GameData.RunTime.Common.RunTimeStorage.Recipes.Count - 1; i > -1; i--)
                    {
                        if (GameData.RunTime.Common.RunTimeStorage.Recipes[i] == 菜单和食物.ID)
                        {
                            _continue = false;
                            break;
                        }
                    }
                    if (_continue)
                    {
                        GameData.RunTime.Common.RunTimeStorage.Recipes.Add(菜单和食物.ID);
                        MonoSingleton<ReceivedObjectDisplayerController>.Instance.NotifyRecipe(菜单和食物.ID);
                    }
                    if (!hasfood || !hassell || !hasrec || !hasdlcmap)
                    {
                        modinitlog.AppendLine("Error!“" + ModID + "”模组的菜单“" + 菜单和食物.Name + "”加载出现问题!原因是ID冲突");
                    }
                    else
                    {
                        modinitlog.AppendLine("“" + ModID + "”模组的菜单“" + 菜单和食物.Name + "”已成功加载");
                    }
                }
            }
            catch (Exception ex)
            {
                modinitlog.AppendLine(ex.ToString());
            }
            finally
            {
                modinitlog.AppendLine("//////////////////////////");
                modinitlog.AppendLine("[MystiaIzakayaBasemod]已成功加载" + ModID);
                UnityEngine.Debug.Log(modinitlog.ToString());
            }
        }
        static 扩展计划Manager()
        {
            BaseModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            RecipeModsPath = BepInEx.Paths.GameRootPath + "/RecipeMods";
            Mods = new List<RecipeMod>();
            InitRecipeMods();
        }
        private static void InitRecipeMods()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(RecipeModsPath);
            foreach (DirectoryInfo dirInfo in directoryInfo.GetDirectories())
            {

                InitRecipeMods(dirInfo);

            }
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Name.EndsWith(".xml"))
                {
                    RecipeMod mod = Deserialize<RecipeMod>(fileInfo.FullName);
                    foreach (var 菜单和食物 in mod.菜单和食物list)
                    {
                        菜单和食物.foodspritepath = fileInfo.DirectoryName + 菜单和食物.foodspritepath;
                    }
                    Mods.Add(mod);
                }
            }

        }
        private static void InitRecipeMods(DirectoryInfo dir)
        {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                InitRecipeMods(d);
            }
            foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
            {
                if (fileInfo.Name.EndsWith(".xml"))
                {
                    RecipeMod mod = Deserialize<RecipeMod>(fileInfo.FullName);
                    foreach (var 菜单和食物 in mod.菜单和食物list)
                    {
                        菜单和食物.foodspritepath = fileInfo.DirectoryName + 菜单和食物.foodspritepath;
                    }
                    Mods.Add(mod);
                }
            }
        }
        public static void LoadAssets(GameObject root)
        {
            foreach (var file in new DirectoryInfo(BepInEx.Paths.GameRootPath + "/Unity").GetFiles())
            {
                AssetBundle value = AssetBundle.LoadFromFile(file.FullName);
                foreach (var x in value.LoadAllAssets())
                {

                    var gameObject = UnityEngine.Object.Instantiate(x, root.transform);
                    
                }
            }
        }
        public static Sprite GetArtWork(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return GameData.CoreLanguage.ObjectLanguageBase.defaultNull;
            }
            else
            {
                Texture2D texture2D = new Texture2D(2, 2)
                {
                    filterMode = FilterMode.Point
                };
                ImageConversion.LoadImage(texture2D, File.ReadAllBytes(FilePath));
                texture2D.filterMode = FilterMode.Point;
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 48f * (texture2D.height / 50));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
                value.name = fileNameWithoutExtension;
                texture2D.name = fileNameWithoutExtension;
                return value;
            }
        }
        public static List<RecipeMod> Mods;
        public static readonly string RecipeModsPath;
        public static readonly string BaseModPath;

    }
}
#endif