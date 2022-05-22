using System;
using System.IO;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

/*==========================
* 创建时间: 2022/5/26 20:05:12
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.IL2CPPUnityCall
{
	public class AssetBundle : UnityEngine.Object
	{
		internal delegate IntPtr d_LoadFromFile(IntPtr path, uint crc, ulong offset);

		private delegate IntPtr d_LoadFromMemory(IntPtr binary, uint crc);

		public delegate IntPtr d_GetAllLoadedAssetBundles_Native();

		internal delegate IntPtr d_LoadAssetWithSubAssets_Internal(IntPtr _this, IntPtr name, IntPtr type);

		internal delegate IntPtr d_LoadAsset_Internal(IntPtr _this, IntPtr name, IntPtr type);

		internal delegate void d_Unload(IntPtr _this, bool unloadAllLoadedObjects);

		public readonly IntPtr m_bundlePtr = IntPtr.Zero;

		static AssetBundle()
		{
			ClassInjector.RegisterTypeInIl2Cpp<AssetBundle>();
		}

		[HideFromIl2Cpp]
		public static AssetBundle LoadFromFile(string path)
		{
			IntPtr intPtr = ICallManager.GetICallUnreliable<d_LoadFromFile>(new string[2] { "UnityEngine.AssetBundle::LoadFromFile_Internal", "UnityEngine.AssetBundle::LoadFromFile" })(IL2CPP.ManagedStringToIl2Cpp(path), 0u, 0uL);
			return (intPtr != IntPtr.Zero) ? new AssetBundle(intPtr) : null;
		}

		[HideFromIl2Cpp]
		public static AssetBundle LoadFromMemory(byte[] binary, uint crc = 0u)
		{
			IntPtr intPtr = ICallManager.GetICallUnreliable<d_LoadFromMemory>(new string[2] { "UnityEngine.AssetBundle::LoadFromMemory_Internal", "UnityEngine.AssetBundle::LoadFromMemory" })(((Il2CppStructArray<byte>)binary).Pointer, crc);
			return (intPtr != IntPtr.Zero) ? new AssetBundle(intPtr) : null;
		}

		[HideFromIl2Cpp]
		public static AssetBundle[] GetAllLoadedAssetBundles()
		{
			IntPtr intPtr = ICallManager.GetICall<d_GetAllLoadedAssetBundles_Native>("UnityEngine.AssetBundle::GetAllLoadedAssetBundles_Native")();
			return (intPtr != IntPtr.Zero) ? ((AssetBundle[])new Il2CppReferenceArray<AssetBundle>(intPtr)) : null;
		}

		public AssetBundle(IntPtr ptr)
			: base(ptr)
		{
			m_bundlePtr = ptr;
		}

		[HideFromIl2Cpp]
		public UnityEngine.Object[] LoadAllAssets()
		{
			IntPtr intPtr = ICallManager.GetICall<d_LoadAssetWithSubAssets_Internal>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal")(m_bundlePtr, IL2CPP.ManagedStringToIl2Cpp(""), UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.Object>().Pointer);
			return (intPtr != IntPtr.Zero) ? ((UnityEngine.Object[])new Il2CppReferenceArray<UnityEngine.Object>(intPtr)) : new UnityEngine.Object[0];
		}
		public static void InstantiateAllAssets(GameObject root,string dicroot)
		{
			foreach (var file in new DirectoryInfo(dicroot).GetFiles())
			{
				AssetBundle value = AssetBundle.LoadFromFile(file.FullName);
				foreach (var x in value.LoadAllAssets())
				{

					var gameObject = UnityEngine.Object.Instantiate(x, root.transform);

				}
			}
		}

		[HideFromIl2Cpp]
		public T LoadAsset<T>(string name) where T : UnityEngine.Object
		{
			IntPtr intPtr = ICallManager.GetICall<d_LoadAsset_Internal>("UnityEngine.AssetBundle::LoadAsset_Internal")(m_bundlePtr, IL2CPP.ManagedStringToIl2Cpp(name), UnhollowerRuntimeLib.Il2CppType.Of<T>().Pointer);
			return (intPtr != IntPtr.Zero) ? new UnityEngine.Object(intPtr).TryCast<T>() : null;
		}

		[HideFromIl2Cpp]
		public void Unload(bool unloadAllLoadedObjects)
		{
			ICallManager.GetICall<d_Unload>("UnityEngine.AssetBundle::Unload")(m_bundlePtr, unloadAllLoadedObjects);
		}
	}

}
