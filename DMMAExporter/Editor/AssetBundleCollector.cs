using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace DMMAExporter
{
    public static class AssetBundleCollector
    {
        private static void SaveAssetPath(HashSet<string> assetBundle, GameObject gameObject, Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (assetPath == null || assetPath == "")
            {
                Debug.LogWarning("Failed to save \"" + obj?.ToString() + "\"", gameObject);
                return;
            }

            Debug.Log("Path to add: " + assetPath);
            assetBundle.Add(assetPath);
        }

        private static void CollectMaterial(HashSet<string> assetBundle, GameObject gameObject, Material material)
        {
            var shader = material.shader;
            for(int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var texture = material.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                    SaveAssetPath(assetBundle, gameObject, texture);
                }
            }

            SaveAssetPath(assetBundle, gameObject, material);
        }

        private static void CollectRecursive(HashSet<string> assetBundle, Transform transform)
        {
            var gameObject = transform.gameObject;
            var components = gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component is SkinnedMeshRenderer skinnedMesh)
                {
                    foreach (var material in skinnedMesh.sharedMaterials)
                        CollectMaterial(assetBundle, gameObject, material);
                    SaveAssetPath(assetBundle, gameObject, skinnedMesh.sharedMesh);
                }

                if (component is MeshRenderer mesh)
                {
                    foreach (var material in mesh.sharedMaterials)
                        CollectMaterial(assetBundle, gameObject, material);
                    SaveAssetPath(assetBundle, gameObject, mesh);
                }

                if (component is Animator animator)
                {
                    SaveAssetPath(assetBundle, gameObject, animator.avatar);
                }
            }

            for (int i = 0; i < transform.childCount; i++)
                CollectRecursive(assetBundle, transform.GetChild(i));
        }

        public static string[] Collect(GameObject gameObject, IEnumerable<string> startingList = null)
        {
            var assetBundle = new HashSet<string>(startingList ?? Enumerable.Empty<string>());
            CollectRecursive(assetBundle, gameObject.transform);
            return assetBundle.ToArray();
        }
    }
}