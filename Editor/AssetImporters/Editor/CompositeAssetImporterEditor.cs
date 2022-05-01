using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using Object = UnityEngine.Object;

namespace PowerEditor.AssetImporters
{
	[CustomEditor(typeof(CompositeAssetImporter))]
	[CanEditMultipleObjects]
	public class CompositeAssetImporterEditor : ScriptedImporterEditor
	{
		new CompositeAssetImporter target => base.target as CompositeAssetImporter;

		protected override Type extraDataType => target.compositeAsset.GetType();

		protected override void InitializeExtraDataInstance(Object extraTarget, int targetIndex)
		{
			CompositeAsset compositeAsset = extraTarget as CompositeAsset;
			string path = ((AssetImporter)targets[targetIndex]).assetPath;
			if (File.Exists(path))
			{
				string content = File.ReadAllText(path);
				CompositeAsset.Parse(content, compositeAsset);
			}
		}
		
		public override void OnInspectorGUI()
		{
			extraDataSerializedObject.Update();
			Type type = target.compositeAsset.GetType();
			MethodInfo onInspectorGUI = type.GetMethod("OnInspectorGUI");
			onInspectorGUI?.Invoke(
				target.compositeAsset, 
				new object[]
				{
					extraDataSerializedObject, 
					target
				});

			extraDataSerializedObject.ApplyModifiedProperties();
			ApplyRevertGUI();
		}

		protected override void Apply()
		{
			base.Apply();
			for (int i = 0; i < targets.Length; ++i)
			{
				File.WriteAllText(
					target.assetPath, 
					CompositeAsset.Parse((CompositeAsset)extraDataTargets[i]));
			}
		}
	}
}