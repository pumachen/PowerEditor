using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using Object = UnityEngine.Object;

namespace PowerEditor.AssetImporters
{
	public abstract class CompositeAsset : ScriptableObject
	{
		public CompositeResult[] compositeResults = new CompositeResult[0];
		public abstract IEnumerable<CompositeResult> Composite(ScriptedImporter importer, AssetImportContext ctx);

		public Object this[string identifier]
		{
			get
			{
				CompositeResult result = compositeResults.FirstOrDefault((result) => string.Compare(identifier, result.identifier) == 0);
				return result?.obj;
			}
		}

		public abstract void OnInspectorGUI(SerializedObject serializedObject, ScriptedImporter importer);

		public virtual IEnumerable<string> dependentArtifacts { get { yield break; } }
		public virtual IEnumerable<string> dependentSourceAssets { get { yield break; } }

		public static CompositeAsset Parse(string content)
		{
			using (StringReader sr = new StringReader(content))
			{
				string typeName = sr.ReadLine();
				Type type = Type.GetType(typeName);
				string json = sr.ReadToEnd();
				CompositeAsset compositeAsset = ObjectFactory.CreateInstance(type) as CompositeAsset;
				JsonUtility.FromJsonOverwrite(json, compositeAsset);
				return compositeAsset;
			}
		}

		public static void Parse(string content, CompositeAsset compositeAsset)
		{
			using (StringReader sr = new StringReader(content))
			{
				string typeName = sr.ReadLine();
				Type type = Type.GetType(typeName);
				string json = sr.ReadToEnd();
				MethodInfo fronJsonInternal = typeof(JsonUtility).GetMethod("FromJsonInternal", BindingFlags.Static | BindingFlags.NonPublic);
				fronJsonInternal.Invoke(null, new object[] {json, compositeAsset, type});
			}
		}

		public static string Parse(CompositeAsset compositeAsset)
		{
			using (StringWriter sw = new StringWriter())
			{
				Type type = compositeAsset.GetType();
				sw.WriteLine(type.AssemblyQualifiedName);
				string json = JsonUtility.ToJson(compositeAsset, true);
				sw.Write(json);
				return sw.ToString();
			}
		}

		[Serializable]
		public class CompositeResult
		{
			public string identifier;
			public Object obj;

			public CompositeResult(string identifier, Object obj)
			{
				this.identifier = identifier;
				this.obj = obj;
			}
		}
	}
}