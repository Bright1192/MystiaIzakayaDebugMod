using System;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

/*==========================
* 创建时间: 2022/5/26 20:10:46
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.IL2CPPUnityCall
{
    public static class Provider
    {
        internal delegate IntPtr d_LayerToName(int layer);

        internal delegate IntPtr d_FindObjectsOfTypeAll(IntPtr type);

        internal delegate void d_GetRootGameObjects(int handle, IntPtr list);

        internal delegate int d_GetRootCountpublic(int handle);
        public static AssetReference SaveScene = new AssetReference("06ba7d38ea131694180b13f2f0b60f43");
        public static AssetReference MainScene = new AssetReference("11ebc89e124308f41a3763edd2133648");
        public static AssetReference DayScene = new AssetReference("85111e8d700c3f240879e16b8dda69f1");

        public static void DoSave(int addday=0)
        {
            GameData.RunTime.Common.RunTimePlayerData.AddDay(addday);
            SaveScene.LoadScene();
        }
        public static string LayerToName(int layer)
        {
            d_LayerToName iCall = ICallManager.GetICall<d_LayerToName>("UnityEngine.LayerMask::LayerToName");
            return IL2CPP.Il2CppStringToManaged(iCall(layer));
        }

        public static UnityEngine.Object[] FindObjectsOfTypeAll(Type type)
        {
            return (UnityEngine.Object[])new Il2CppReferenceArray<UnityEngine.Object>(ICallManager.GetICallUnreliable<d_FindObjectsOfTypeAll>(new string[2] { "UnityEngine.Resources::FindObjectsOfTypeAll", "UnityEngine.ResourcesAPIpublic::FindObjectsOfTypeAll" })(Il2CppType.From(type).Pointer));
        }
        public static UnityEngine.Object[] FindObjectsOfTypeAll<T>()
        {
            return FindObjectsOfTypeAll(typeof(T));
        }
        public static GameObject[] GetRootGameObjects(Scene scene)
        {
            if (!scene.isLoaded || scene.handle == -1)
            {
                return new GameObject[0];
            }
            int rootCount = GetRootCount(scene.handle);
            if (rootCount < 1)
            {
                return new GameObject[0];
            }
            Il2CppSystem.Collections.Generic.List<GameObject> list = new Il2CppSystem.Collections.Generic.List<GameObject>(rootCount);
            ICallManager.GetICall<d_GetRootGameObjects>("UnityEngine.SceneManagement.Scene::GetRootGameObjectspublic")(scene.handle, list.Pointer);
            return (GameObject[])list.ToArray();
        }

        public static int GetRootCount(Scene scene)
        {
            return GetRootCount(scene.handle);
        }

        public static int GetRootCount(int sceneHandle)
        {
            return ICallManager.GetICall<d_GetRootCountpublic>("UnityEngine.SceneManagement.Scene::GetRootCountpublic")(sceneHandle);
        }
    }
}
