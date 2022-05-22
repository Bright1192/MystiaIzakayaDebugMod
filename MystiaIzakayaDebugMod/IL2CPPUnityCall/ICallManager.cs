using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;

/*==========================
* 创建时间: 2022/5/26 20:03:35
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.IL2CPPUnityCall
{
	/// <summary>
	/// 用于实现一些Unity底层(比如一些标有[MethodImpl(MethodImplOptions.InternalCall)]的Unity底层方法)的调用
	/// </summary>
	public static class ICallManager
	{
		private static readonly Dictionary<string, Delegate> iCallCache = new Dictionary<string, Delegate>();

		private static readonly Dictionary<string, Delegate> unreliableCache = new Dictionary<string, Delegate>();

		public static T GetICall<T>(string signature) where T : Delegate
		{
			if (iCallCache.ContainsKey(signature))
			{
				return (T)iCallCache[signature];
			}
			IntPtr intPtr = IL2CPP.il2cpp_resolve_icall(signature);
			if (intPtr == IntPtr.Zero)
			{
				throw new MissingMethodException("Could not find any iCall with the signature '" + signature + "'!");
			}
			Delegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(intPtr, typeof(T));
			iCallCache.Add(signature, delegateForFunctionPointer);
			return (T)delegateForFunctionPointer;
		}

		public static T GetICallUnreliable<T>(params string[] possibleSignatures) where T : Delegate
		{
			string text = possibleSignatures.First();
			if (unreliableCache.ContainsKey(text))
			{
				return (T)unreliableCache[text];
			}
			foreach (string name in possibleSignatures)
			{
				IntPtr intPtr = IL2CPP.il2cpp_resolve_icall(name);
				if (intPtr != IntPtr.Zero)
				{
					T val = (T)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(T));
					unreliableCache.Add(text, val);
					return val;
				}
			}
			throw new MissingMethodException("Could not find any iCall from list of provided signatures starting with '" + text + "'!");
		}
	}

}
