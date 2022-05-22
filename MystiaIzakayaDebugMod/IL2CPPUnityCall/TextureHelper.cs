using System;
using System.IO;
using System.Linq;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

/*==========================
* 创建时间: 2022/5/26 20:28:59
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.IL2CPPUnityCall
{
	/// <summary>
	/// 实现保存图片
	/// </summary>
    public static class TextureHelper
    {
        internal delegate IntPtr d_EncodeToPNG(IntPtr tex);

        internal delegate void d_Blit2(IntPtr source, IntPtr dest);

        internal delegate IntPtr d_CreateSprite(IntPtr texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, int meshType, ref Vector4 border, bool generateFallbackPhysicsShape);

        internal delegate void d_CopyTexture_Region(IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, IntPtr dst, int dstElement, int dstMip, int dstX, int dstY);
		public static void SaveTextureAsPNG(this Texture2D texture, string fullPathWithFilename)
		{
			Texture2D texture2D = texture;
			if (!IsReadable(texture2D) || texture2D.format != TextureFormat.ARGB32)
			{
				texture2D = CopyToARGB32(texture2D);
			}
			byte[] array = EncodeToPNG(texture2D);
			if (array == null || !array.Any())
			{
				throw new Exception("The data from calling EncodeToPNG on the provided Texture was null or empty.");
			}
			string directoryName = Path.GetDirectoryName(fullPathWithFilename);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(Path.Combine(directoryName, fullPathWithFilename), array);
			if (texture2D != texture)
			{
				UnityEngine.Object.Destroy(texture2D);
			}
		}

		public static Texture2D NewTexture2D(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, 1, linear: false, IntPtr.Zero);
        }

        public static Texture2D NewTexture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
        {
            return new Texture2D(width, height, textureFormat, (!mipChain) ? 1 : (-1), linear: false, IntPtr.Zero);
        }

        public static void Blit(Texture tex, RenderTexture rt)
        {
            ICallManager.GetICall<d_Blit2>("UnityEngine.Graphics::Blit2")(tex.Pointer, rt.Pointer);
        }

        public static byte[] EncodeToPNG(Texture2D tex)
        {
            IntPtr intPtr = ICallManager.GetICall<d_EncodeToPNG>("UnityEngine.ImageConversion::EncodeToPNG")(tex.Pointer);
            return (byte[])((intPtr == IntPtr.Zero) ? null : new Il2CppStructArray<byte>(intPtr));
        }

        public static Sprite CreateSprite(Texture2D texture)
        {
            return CreateSpriteImpl(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero, 100f, 0u, Vector4.zero);
        }

        public static Sprite CreateSprite(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, Vector4 border)
        {
            return CreateSpriteImpl(texture, rect, pivot, pixelsPerUnit, extrude, border);
        }
		public static Texture2D CopyTexture(Texture source, Rect dimensions = default(Rect), int cubemapFace = 0, int dstX = 0, int dstY = 0)
		{
			TextureFormat textureFormat = TextureFormat.ARGB32;
			Texture2D texture2D = source.TryCast<Texture2D>();
			if ((object)texture2D != null)
			{
				textureFormat = texture2D.format;
			}
			else
			{
				Cubemap cubemap = source.TryCast<Cubemap>();
				if ((object)cubemap != null)
				{
					textureFormat = cubemap.format;
				}
			}
			Texture2D texture2D2 = NewTexture2D((int)dimensions.width, (int)dimensions.height, textureFormat, mipChain: false);
			texture2D2.filterMode = FilterMode.Point;
			return CopyTexture(source, texture2D2, dimensions, cubemapFace, dstX, dstY);
		}

		public static Texture2D CopyTexture(Texture source, Texture2D destination, Rect dimensions = default(Rect), int cubemapFace = 0, int dstX = 0, int dstY = 0)
		{
			try
			{
				if (source.TryCast<Texture2D>() == null && source.TryCast<Cubemap>() == null)
				{
					throw new NotImplementedException("TextureHelper.Copy does not support Textures of type 'Texture'.");
				}
				if (dimensions == default(Rect))
				{
					dimensions = new Rect(0f, 0f, source.width, source.height);
				}
				Texture2D texture2D = source.TryCast<Texture2D>();
				if ((object)texture2D != null)
				{
					return CopyToARGB32(texture2D, dimensions, dstX, dstY);
				}
				CopyTexture(source, cubemapFace, 0, 0, 0, source.width, source.height, destination, 0, 0, dstX, dstY);
				return destination;
			}
			catch 
			{
				return null;
			}
		}
		public static Texture2D CopyToARGB32(Texture2D origTex, Rect dimensions = default(Rect), int dstX = 0, int dstY = 0)
		{
			if (dimensions == default(Rect) && origTex.format == TextureFormat.ARGB32 && IsReadable(origTex))
			{
				return origTex;
			}
			if (dimensions == default(Rect))
			{
				dimensions = new Rect(0f, 0f, origTex.width, origTex.height);
			}
			FilterMode filterMode = origTex.filterMode;
			RenderTexture active = RenderTexture.active;
			origTex.filterMode = FilterMode.Point;
			RenderTexture temporary = RenderTexture.GetTemporary(origTex.width, origTex.height, 0, RenderTextureFormat.ARGB32);
			temporary.filterMode = FilterMode.Point;
			RenderTexture.active = temporary;
			Blit(origTex, temporary);
			Texture2D texture2D = NewTexture2D((int)dimensions.width, (int)dimensions.height);
			texture2D.ReadPixels(dimensions, dstX, dstY);
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			RenderTexture.active = active;
			origTex.filterMode = filterMode;
			return texture2D;
		}
		public static bool IsReadable(Texture2D tex)
		{
			try
			{
				tex.GetPixel(0, 0);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public static Sprite CreateSpriteImpl(Texture texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, Vector4 border)
        {
            IntPtr intPtr = ICallManager.GetICall<d_CreateSprite>("UnityEngine.Sprite::CreateSprite_Injected")(texture.Pointer, ref rect, ref pivot, pixelsPerUnit, extrude, 1, ref border, generateFallbackPhysicsShape: false);
            return (intPtr == IntPtr.Zero) ? null : new Sprite(intPtr);
        }

        public static Texture CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
        {
            ICallManager.GetICall<d_CopyTexture_Region>("UnityEngine.Graphics::CopyTexture_Region")(src.Pointer, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst.Pointer, dstElement, dstMip, dstX, dstY);
            return dst;
        }
    }
}
