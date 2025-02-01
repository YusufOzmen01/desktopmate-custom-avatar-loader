using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DMMAExporter
{
    public static class Exporter
    {
        public static void Export(GameObject model, bool dryrun)
        {
            var tempBuildDir = "Assets/DMMAExporter/temp";
            Debug.Log("tempBuildDir: " + tempBuildDir);
            var tempDir = Path.GetTempPath() + Guid.NewGuid() + "/";
            if (!dryrun)
            {
                if (Directory.Exists(tempBuildDir))
                    AssetDatabase.DeleteAsset(tempBuildDir);
                AssetDatabase.CreateFolder("Assets/DMMAExporter", "temp");

                Directory.CreateDirectory(tempDir);
            }

            var prefabPath = tempBuildDir + "/TempAvatar.prefab";

            if (!dryrun)
            {
                PrefabUtility.SaveAsPrefabAsset(model, prefabPath);
                AssetDatabase.Refresh();
            }
            else Debug.Log("Would create prefab at \"" + prefabPath + "\"");

            var assetBundle = AssetBundleCollector.Collect(model, new string[] { prefabPath });
            if (dryrun)
            {
                Debug.Log("AssetBundle:");
                foreach (var item in assetBundle)
                {
                    Debug.Log("Item: " + item);
                    Debug.Log("GUID: " + AssetDatabase.AssetPathToGUID(item));
                }
            }
            var destination = EditorUtility.SaveFilePanel($"Save {model.name} Model", "", model.name, "dmma");
            if (!dryrun)
            {
                BuildPipeline.BuildAssetBundles(
                    tempDir,
                    new AssetBundleBuild[]{
                        new AssetBundleBuild {
                            assetNames = assetBundle,
                            addressableNames = new string[]{},
                            assetBundleName = "Avatar",
                            assetBundleVariant = "dmma"
                        },
                    },
                    BuildAssetBundleOptions.UncompressedAssetBundle,
                    BuildTarget.StandaloneWindows64
                );

                if (File.Exists(destination)) File.Delete(destination);
                else Directory.CreateDirectory(new FileInfo(destination).Directory.FullName);
                File.Move(tempDir + "Avatar.dmma", destination);

                // Yes, I'm sleeping the Unity main thread. But I'm just waiting for the file
                // explorer to refresh and notice the new model, and the user shouldn't notice
                // 0.3 seconds.
                var task = Task.Delay(300);
                task.Wait();

                EditorUtility.RevealInFinder(destination);
                AssetDatabase.DeleteAsset(tempBuildDir);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log($"Would create asset bundle and save at {destination}.");
            }

        }
    }
}