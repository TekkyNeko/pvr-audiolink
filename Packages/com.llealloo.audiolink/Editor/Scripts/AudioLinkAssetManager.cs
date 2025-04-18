﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AudioLink.Editor
{
    [InitializeOnLoad]
    public class AudioLinkAssetManager
    {
        private const string audioLinkReimportedKey = "AUDIOLINK_REIMPORTED";

        static AudioLinkAssetManager()
        {
            // Skip if we've already checked for the canary file during this Editor Session
            if (!SessionState.GetBool(audioLinkReimportedKey, false))
            {
                // Check for canary file in Library - package probably needs a reimport after a Library wipe
                string canaryFilePath = Path.Combine("Library", audioLinkReimportedKey);
                if (File.Exists(canaryFilePath))
                {
                    SessionState.SetBool(audioLinkReimportedKey, true);
                }
                else
                {
                    ReimportPackage();
                    File.WriteAllText(canaryFilePath, audioLinkReimportedKey);
                }
            }
        }

        private static void ReimportPackage()
        {
            AssetDatabase.ImportAsset(Path.Combine("Packages", "com.llealloo.audiolink"), ImportAssetOptions.ImportRecursive);
            SessionState.SetBool(audioLinkReimportedKey, true);
        }

#if !AUDIOLINK_STANDALONE
        [MenuItem("Tools/AudioLink/Open AudioLink Example Scene")]
        public static void OpenExampleScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string baseAssetsPath = "Samples/AudioLink/2.1.0";
                string packagePath = "Packages/com.llealloo.audiolink/Samples~/AudioLinkExampleScene";
                string assetsPath = Path.Combine("Assets", baseAssetsPath, "AudioLinkExampleScene");
                if (!Directory.Exists(Path.Combine(Application.dataPath, baseAssetsPath, "AudioLinkExampleScene")))
                {
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, baseAssetsPath));
                    FileUtil.CopyFileOrDirectory(packagePath, assetsPath);
                    AssetDatabase.Refresh();
                }

                EditorSceneManager.OpenScene(Path.Combine(assetsPath, "AudioLink_ExampleScene.unity"));
            }
        }
#endif

        private const string _audioLinkPath = "Packages/com.llealloo.audiolink/Runtime/PVRAudioLink.prefab";
        private const string _audioLinkControllerPath = "Packages/com.llealloo.audiolink/Runtime/PVRAudioLinkController.prefab";
        private const string _audioLinkAvatarPath = "Packages/com.llealloo.audiolink/Runtime/AudioLinkAvatar.prefab";

        [MenuItem("Tools/AudioLink/Add AudioLink Prefab to Scene", false)]
        [MenuItem("GameObject/AudioLink/Add AudioLink Prefab to Scene", false, 49)]
        public static void AddAudioLinkToScene()
        {
            GameObject audiolink = null;

#if PVR_CCK_WORLDS // PVR World        
            var alInstance = GetComponentsInScene<AudioLink>().FirstOrDefault();
            audiolink = alInstance != null ? alInstance.gameObject : AddPrefabInstance(_audioLinkPath);

            var alcInstance = GetComponentsInScene<AudioLinkController>().FirstOrDefault();
            if (alcInstance == null) AddPrefabInstance(_audioLinkControllerPath);
#else // PVR Avatar, originally Standalone
            var alInstance = GetComponentsInScene<AudioLink>().FirstOrDefault();
            audiolink = alInstance != null ? alInstance.gameObject : AddPrefabInstance(_audioLinkAvatarPath);
#endif

            if (audiolink != null)
            {
                AudioLinkEditor.LinkAll(audiolink.GetComponent<AudioLink>());
                EditorGUIUtility.PingObject(audiolink);
            }
        }

        private static GameObject AddPrefabInstance(string assetPath)
        {
            var sourceAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject instance = null;
            if (sourceAsset != null)
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(sourceAsset);
                Undo.RegisterCreatedObjectUndo(instance, "Undo create prefab instance");
            }

            return instance;
        }

        private static T[] GetComponentsInScene<T>(bool includeInactive = true) where T : Component
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            GameObject[] roots = stage == null ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects() : new[] { stage.prefabContentsRoot };
            List<T> objects = new List<T>();
            foreach (GameObject root in roots) objects.AddRange(root.GetComponentsInChildren<T>(includeInactive));
            return objects.ToArray();
        }

        private static GameObject[] GetGameObjectsInScene(string name, bool includeInactive = true)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            GameObject[] roots = stage == null ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects() : new[] { stage.prefabContentsRoot };
            List<Transform> objects = new List<Transform>();
            foreach (GameObject root in roots) objects.AddRange(root.GetComponentsInChildren<Transform>(includeInactive));
            return objects.Where(t => t.gameObject.name == name).Select(t => t.gameObject).ToArray();
        }
    }
}
