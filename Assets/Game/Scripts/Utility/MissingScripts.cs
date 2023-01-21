using System.Linq;
using UnityEngine;
using UnityEditor;

public static class MissingScripts 
{
    [MenuItem("Menu/Find Missing Scripts in Project")]
    static void FindMissingScriptsInProject()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();
        foreach (var path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            foreach (var component in prefab.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log("Prefab found with missing script: " + path, prefab);
                    break;
                }
            }
        }
    }

    [MenuItem("Menu/Find Missing Scripts in Scene")]
    static void FindMissingScriptsInScene()
    {
        foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>(true))
        {
            foreach (var component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log("GameObject found with missing script: " + gameObject.name, gameObject);
                    break;
                }
            }
        }
    }
}
