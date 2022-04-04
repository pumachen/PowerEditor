using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace PowerEditor
{
	//[CustomEditor(typeof(Texture2D))]
    public class AssetInspector : Editor
    {
	    private Texture target => base.target as Texture;
	    public override void OnInspectorGUI()
	    {
		    AssetImporter importer = target.GetAssetImporter();
		    if (importer != null)
		    {
			    string[] references = importer.GetDependencies();
			    EditorGUILayout.LabelField("Dependencies");
			    foreach (var asset in references
				    .Select(guid => AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid))))
			    {
				    EditorGUILayout.ObjectField(asset, asset.GetType(), false);
			    }

			    if (importer.TryGetUserData("PCAParams", out PCACompressor.PCAParams pcaParams))
			    {
				    EditorGUILayout.Vector3Field("NormalA", pcaParams.A.normal);
				    EditorGUILayout.FloatField("DistA", pcaParams.A.distance);
				    EditorGUILayout.Vector3Field("NormalB", pcaParams.B.normal);
				    EditorGUILayout.FloatField("DistB", pcaParams.B.distance);
				    EditorGUILayout.Vector4Field("DecodeA", pcaParams.A.decodeParams);
				    EditorGUILayout.Vector4Field("DecodeB", pcaParams.B.decodeParams);
			    }
		    }

		    float width = EditorGUIUtility.currentViewWidth;
		    GUILayout.Box(target, GUILayout.Width(width), GUILayout.Height(width));
		    base.OnInspectorGUI();
	    }
	}
}