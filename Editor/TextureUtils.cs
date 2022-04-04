using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace PowerEditor
{
	public static class Textureutils
	{
		public enum TexturePropertyMask
		{
			Resolution = 1,
			Format = 2,
			GraphicsFormat = 4,
			MipmapCount = 8,
			Readable = 16,
			All = 31
		};
		public static bool CompareTextureFormat(TexturePropertyMask propertyMask = TexturePropertyMask.All, params Texture2D[] textures)
		{
			if (textures.Length < 2 && textures[0] != null)
			{
				return true;
			}

			if (textures[0] == null)
				return false;
			Texture2D template = textures[0];
			int width = template.width;
			int height = template.height;
			TextureFormat format = template.format;
			GraphicsFormat graphicsFormat = template.graphicsFormat;
			int mipmapCount = template.mipmapCount;
			bool readable = template.isReadable;
			int mask = (int)propertyMask;
			for (int i = 1; i < textures.Length; ++i)
			{
				Texture2D texture = textures[i];
				if (texture == null)
					return false;
				if ((mask & (int) TexturePropertyMask.Resolution) > 0)
					if (texture.width != width || texture.height != height)
						return false;
				if ((mask & (int) TexturePropertyMask.Format) > 0)
					if (texture.format != format)
						return false;
				if ((mask & (int) TexturePropertyMask.GraphicsFormat) > 0)
					if (texture.graphicsFormat != graphicsFormat)
						return false;
				if ((mask & (int) TexturePropertyMask.MipmapCount) > 0)
					if (texture.mipmapCount != mipmapCount)
						return false;
				if ((mask & (int) TexturePropertyMask.Readable) > 0)
					if (texture.isReadable != readable)
						return false;
			}	

			return true;
		}
	}
}