using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace PowerEditor.AssetManagement
{
    public static class MetaUtils
    {
        public static void RegisterAssetDependency(this AssetImporter importer, params Object[] dependentAssets)
        {
            IEnumerable<string> assetGUIDs = dependentAssets
                .Select(obj => AssetDatabaseUtils.GetAssetGUID(obj))
                .Where(guid => !string.IsNullOrEmpty(guid));
            HashSet<string> guidSet = new HashSet<string>(assetGUIDs);
            if (importer.TryGetUserData("Dependencies", out string[] guids))
            {
                guidSet.UnionWith(guids);
            }
            importer.SetUserData("Dependencies", guidSet.ToArray());
            /*foreach (var assetImporter in guidSet.Select(guid => AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid))))
            {
            }*/
        }

        public static void RemoveAssetDependency(this AssetImporter importer, string GUID)
        {
            if (importer.TryGetUserData("Dependencies", out string[] guids))
            {
                guids = guids.Where(guid => guid != GUID).ToArray();
                importer.SetUserData("Dependencies", guids);
            }
        }

        public static string[] GetDependencies(this AssetImporter importer)
        {
            if (!importer.TryGetUserData("Dependencies", out string[] references))
            {
                return null;
            }
            return references;
        }

        public static bool TryGetUserData<T>(this AssetImporter importer, string key, out T value) where T : class
        {
            Dictionary<string, string> userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(importer.userData);
            value = default;
            if(userData != null && userData.TryGetValue(key, out string json))
            {
                if (typeof(T).IsArray)
                {
                    value = JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    value = JsonUtility.FromJson<T>(json);
                }
                return true;
            }
            return false;
        }
        
        public static void SetUserData<T>(this AssetImporter importer, string key, T value)
        {
            Dictionary<string, string> userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(importer.userData);
            if (userData == null)
                userData = new Dictionary<string, string>();
            if (typeof(T).IsArray)
            {
                userData[key] = JsonConvert.SerializeObject(value);
            }
            else
            {
                userData[key] = JsonUtility.ToJson(value);
            }
            string json = JsonConvert.SerializeObject(userData);
            Debug.Log(json);
            importer.userData = json;
        }
    }
}