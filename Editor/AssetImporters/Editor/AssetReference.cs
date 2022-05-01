using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PowerEditor.AssetImporters
{
	[Serializable]
	public abstract class AssetReference
	{
		[SerializeField]
		protected string assetPath;
		protected Object asset
		{
			get => AssetDatabase.LoadAssetAtPath<Object>(assetPath);
		}

		[SerializeField]
		protected string type;
		public static implicit operator Object(AssetReference assetReference)
		{
			return assetReference.asset;
		}

		public static implicit operator string(AssetReference assetReference)
		{
			return assetReference.assetPath;
		}
	}
	[Serializable]
	public class AssetReference<T> : AssetReference, ISerializationCallbackReceiver where T : Object
	{
		public new T asset => base.asset as T;
		public string path => assetPath;

		public static implicit operator T(AssetReference<T> assetReference)
		{
			return assetReference.asset;
		}

		public void OnBeforeSerialize()
		{
			type = typeof(T).AssemblyQualifiedName;
		}
		
		public void OnAfterDeserialize() {}
	}
}
