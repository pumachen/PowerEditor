using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PowerEditor.AssetImporters
{
    [CustomPropertyDrawer(typeof(AssetReference), true)]
    public class AssetReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, new GUIContent(property.name), property);
            Type type = Type.GetType(property.FindPropertyRelative("type").stringValue);
            string path = property.FindPropertyRelative("assetPath").stringValue;
            Object obj = EditorGUI.ObjectField(position, label, AssetDatabase.LoadAssetAtPath<Object>(path), type);
            property.FindPropertyRelative("assetPath").stringValue = AssetDatabase.GetAssetPath(obj);
            EditorGUI.EndProperty();
        }
    }
}