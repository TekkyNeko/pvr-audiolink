#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Reflection;

#if PVR_CCK_WORLDS
using PVR.PSharp.Editor;
using PVR.PSharp;
using BehaviourType = PVR.PSharp.PSharpBehaviour;
using PVR.CCK.Worlds.Components;
#else
using BehaviourType = UnityEngine.MonoBehaviour;
#endif
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;


namespace AudioLink.Editor
{
	[CustomEditor(typeof(AudioLink))]
	public class AudioLinkEditor : UnityEditor.Editor
	{
		private readonly static GUIContent DisableReadbackButtonContent = EditorGUIUtility.TrTextContent("Disable readback", "Disables asynchronous readback, which is required for audio-reactive PSharp scripts to function. This feature comes with a slight performance penalty.");
		private readonly static GUIContent EnableReadbackButtonContent = EditorGUIUtility.TrTextContent("Enable readback", "Enables asynchronous readback, which is required for audio-reactive PSharp scripts to function. This feature comes with a slight performance penalty.");

		public void OnEnable()
		{
			AudioLink audioLink = (AudioLink)target;
			if (audioLink.audioData2D == null)
			{
				audioLink.audioData2D =
					AssetDatabase.LoadAssetAtPath<Texture2D>(
						AssetDatabase.GUIDToAssetPath("b07c8466531ac5e4e852f3e276e4baca"));
				Debug.Log(nameof(AudioLink) + ": restored audioData2D reference");
			}
		}

		public override void OnInspectorGUI()
		{
			AudioLink audioLink = (AudioLink)target;
#if UDONSHARP
			if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
#endif
#if PVR_CCK_WORLDS
			PVR_PSharpBehaviour_Editor.DrawPSharpHeaderUI();
			PVR_PSharpBehaviour_Editor.DrawPSharpLine();
#endif
			if (Camera.main == null)
			{
				EditorGUILayout.HelpBox("The current scene might be missing a main camera, this could cause issues with the AudioLink camera.", MessageType.Warning);
			}

			if (audioLink.audioSource == null)
			{
				EditorGUILayout.HelpBox("No audio source assigned. AudioLink will not work.", MessageType.Warning);
			}

#if PVR_CCK_WORLDS
			EditorGUILayout.Space();
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button(new GUIContent("Required: Add scripts to P# Includes", "In order for AudioLink to work on P#, you need to add the scripts to the world descriptor. Click this button to add them.")))
			{
				AddIncludes();
			}
			GUI.backgroundColor = Color.white;
#endif
			EditorGUILayout.Space();
			if (GUILayout.Button(new GUIContent("Link all sound reactive objects to this AudioLink instance",
					"Links all scripts with 'audioLink' parameter to this object.")))
			{
				LinkAll();
			}
			EditorGUILayout.Space();
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			if (audioLink.audioDataToggle)
			{
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button(DisableReadbackButtonContent))
				{
					audioLink.DisableReadback();
					EditorUtility.SetDirty(audioLink);
				}
			}
			else
			{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button(EnableReadbackButtonContent))
				{
					audioLink.EnableReadback();
					EditorUtility.SetDirty(audioLink);
				}
			}
		}

#if PVR_CCK_WORLDS
		public void AddIncludes()
		{
			AddIncludes(GetComponentsInScene<PVR_WorldDescriptor>().FirstOrDefault());
		}

		public static void AddIncludes(PVR_WorldDescriptor worldDescriptor)
		{
			if (worldDescriptor != null)
			{
				List<string> includes = new List<string>(worldDescriptor.psharpIncludes);

				if (!worldDescriptor.psharpIncludes.Contains("Packages/com.llealloo.audiolink/Runtime/Scripts/AudioLink.PlayerAPI.cs"))
					worldDescriptor.psharpIncludes.Add("Packages/com.llealloo.audiolink/Runtime/Scripts/AudioLink.PlayerAPI.cs");

				if (!worldDescriptor.psharpIncludes.Contains("Packages/com.llealloo.audiolink/Runtime/Scripts/AudioLink.DataAPI.cs"))
					worldDescriptor.psharpIncludes.Add("Packages/com.llealloo.audiolink/Runtime/Scripts/AudioLink.DataAPI.cs");

				EditorUtility.SetDirty(worldDescriptor);
				Debug.Log("AudioLink added to P# Includes.");
			}
			else
			{
				Debug.LogWarning("No World Descriptor found in the current scene. Please add one to include AudioLink scripts.");

			}
		}
#endif
		public void LinkAll()
		{
			LinkAll(target as AudioLink);
		}

		public static void LinkAll(AudioLink target)
		{
			BehaviourType[] allBehaviours =
#if UNITY_2021_3_OR_NEWER
				FindObjectsByType<BehaviourType>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
#else
				FindObjectsOfType<BehaviourType>(true);
#endif

			// this handles all reasonable cases of referencing audiolink
			// (it doesn't handle referencing it multiple times in one monobehaviour, or referencing it as it's Base type)
			foreach (BehaviourType behaviour in allBehaviours)
			{
				FieldInfo fieldInfo = behaviour.GetType().GetField("audioLink");
				// in case the field isn't called "audioLink"
				if (fieldInfo == null)
				{
					foreach (FieldInfo field in behaviour.GetType().GetFields())
					{
						if (field.FieldType == typeof(AudioLink))
						{
							fieldInfo = field;
							break;
						}
					}
				}

				if (fieldInfo != null && fieldInfo.FieldType == typeof(AudioLink))
				{
					fieldInfo.SetValue(behaviour, target);
					EditorUtility.SetDirty(behaviour);

					if (PrefabUtility.IsPartOfPrefabInstance(behaviour))
					{
						PrefabUtility.RecordPrefabInstancePropertyModifications(behaviour);
					}
				}
			}
		}
		private static T[] GetComponentsInScene<T>(bool includeInactive = true) where T : Component
		{
			var stage = PrefabStageUtility.GetCurrentPrefabStage();
			GameObject[] roots = stage == null ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects() : new[] { stage.prefabContentsRoot };
			List<T> objects = new List<T>();
			foreach (GameObject root in roots) objects.AddRange(root.GetComponentsInChildren<T>(includeInactive));
			return objects.ToArray();
		}
	}
}
#endif
