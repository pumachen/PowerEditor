using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PowerEditor.AssetManagement
{
    public static class AssetDatabaseUtils
    {
        public static string InProjectPathToFileSystemPath(string pathInProj)
        {
            string dataPath = Application.dataPath;
            return Path.Combine(dataPath, "../" + pathInProj);
        }

        public static string FileSystemPathToInProjectPath(string pathFS)
        {
            return pathFS.Replace(Application.dataPath, "Assets");
        }

        public static void CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            string parentDir = Path.GetDirectoryName(path);
            string folderName = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parentDir) && !AssetDatabase.IsValidFolder(parentDir))
            {
                CreateFolder(parentDir);
            }
            AssetDatabase.CreateFolder(parentDir, folderName);
        }

        public static void CreateOrOverrideAsset<T>(T asset, string path) where T : Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
            {
                CreateFolder(Path.GetDirectoryName(path));
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                string tmpPath = $"{path}_{guid}.tmp";
                AssetDatabase.CreateAsset(asset, tmpPath);
                string tmpPathFS = InProjectPathToFileSystemPath(tmpPath);
                string pathFS = InProjectPathToFileSystemPath(path);
                using (StreamReader sr = new StreamReader(File.OpenRead(tmpPathFS)))
                {
                    string rawData = sr.ReadToEnd();
                    using (StreamWriter sw = File.CreateText(pathFS))
                    {
                        sw.Write(rawData);
                    }
                }
                AssetDatabase.DeleteAsset(tmpPath);
                AssetDatabase.Refresh();
            }
        }

        public static AssetImporter GetAssetImporter(this Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            return AssetImporter.GetAtPath(assetPath);
        }

        public static string GetAssetGUID(Object assetObject)
        {
            string path = AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            else
            {
                return AssetDatabase.AssetPathToGUID(path);
            }
        }

        public static T LoadAssetByGUID<T>(string guid) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
                return null;
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}