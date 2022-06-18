using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace PowerEditor.AssetManagement
{
	public class AssetPostProcessorUtils : AssetPostprocessor
	{
		public static Action<ModelImporter, AssetImportContext> onPreprocessAnimation;
		public static Action<GameObject, AnimationClip, ModelImporter, AssetImportContext> onPostprocessAnimation;
		public static Action<AudioImporter, AssetImportContext> onPreprocessAudio;
		public static Action<AudioClip, AudioImporter, AssetImportContext> onPostprocessAudio;
		public static Action<Cubemap, TextureImporter, AssetImportContext> onPostprocessCubemap;
		public static Action<Material, AssetImporter, AssetImportContext> onPostprocessMaterial;
		public static Action<ModelImporter, AssetImportContext> onPreprocessModel;
		public static Action<GameObject, ModelImporter, AssetImportContext> onPostprocessModel;
		public static Action<GameObject, AssetImporter, AssetImportContext> onPostprocessPrefab;
		public static Action<Texture2D, Sprite[], TextureImporter, AssetImportContext> onPostprocessSprites;
		public static Action<TextureImporter, AssetImportContext> onPreprocessTexture;
		public static Action<Texture2D, TextureImporter, AssetImportContext> onPostprocessTexture;
		public static Action<Texture2DArray, AssetImporter, AssetImportContext> onPostprocessTexture2DArray;
		public static Action<Texture3D, AssetImporter, AssetImportContext> onPostprocessTexture3D;


		private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
		{
			onPostprocessAnimation?.Invoke(root, clip, assetImporter as ModelImporter, context);
		}

		private void OnPostprocessAudio(AudioClip clip)
		{
			onPostprocessAudio?.Invoke(clip, assetImporter as AudioImporter, context);
		}

		private void OnPostprocessCubemap(Cubemap texture)
		{
			onPostprocessCubemap?.Invoke(texture, assetImporter as TextureImporter, context);
		}

		private void OnPostprocessMaterial(Material material)
		{
			onPostprocessMaterial?.Invoke(material, assetImporter, context);
		}

		private void OnPostprocessModel(GameObject model)
		{
			onPostprocessModel?.Invoke(model, assetImporter as ModelImporter, context);
		}

		private void OnPostprocessPrefab(GameObject prefab)
		{
			onPostprocessPrefab?.Invoke(prefab, assetImporter, context);
		}

		private void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
		{
			onPostprocessSprites?.Invoke(texture, sprites, assetImporter as TextureImporter, context);
		}

		private void OnPostprocessTexture(Texture2D texture)
		{
			onPostprocessTexture?.Invoke(texture, assetImporter as TextureImporter, context);
		}

		private void OnPostprocessTexture2DArray(Texture2DArray texture)
		{
			onPostprocessTexture2DArray?.Invoke(texture, assetImporter, context);
		}

		private void OnPostprocessTexture3D(Texture3D texture)
		{
			onPostprocessTexture3D?.Invoke(texture, assetImporter, context);
		}

		private void OnPreprocessAnimation()
		{
			onPreprocessAnimation?.Invoke(assetImporter as ModelImporter, context);
		}

		private void OnPreprocessAudio()
		{
			onPreprocessAudio?.Invoke(assetImporter as AudioImporter, context);
		}

		private void OnPreprocessModel()
		{
			onPreprocessModel?.Invoke(assetImporter as ModelImporter, context);
		}

		private void OnPreprocessTexture()
		{
			onPreprocessTexture?.Invoke(assetImporter as TextureImporter, context);
		}
	}
}
