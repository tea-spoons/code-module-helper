
namespace TeaSpoons.CodeModuleHelper.Editor
{
    using TeaSpoons.PackageCore.Editor;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Reflection;

    public class UnfinishedCodeWindow : EditorWindow
    {
        [MenuItem(Menus.RootItem + "Unfinished Code")]
        private static void Open()
        {
            var window = GetWindow<UnfinishedCodeWindow>();
            var icon = EditorGUIUtility.IconContent("d_DebuggerDisabled").image;
            window.titleContent = new GUIContent("Unfinished Code", icon);
        }

        private static Texture prototypeIcon;
        private static Texture draftIcon;

        private Dictionary<MonoScript, CodeStatus> taggedTypes;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            taggedTypes = new();
            var types = TypeCache.GetTypesWithAttribute<UnfinishedAttribute>();
            var typeSet = new HashSet<System.Type>(types);

            var guids = AssetDatabase.FindAssets("t:MonoScript");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                var type = monoScript.GetClass();
                if (monoScript != null && typeSet.Contains(type))
                {
                    var attribute = type.GetCustomAttribute<UnfinishedAttribute>();
                    taggedTypes.Add(monoScript, attribute.Status);
                }
            }

            prototypeIcon ??= EditorGUIUtility.IconContent("console.erroricon.sml").image;
            draftIcon ??= EditorGUIUtility.IconContent("console.warnicon.sml").image;
        }

        private void OnGUI()
        {
            GUILayout.Label($"Detected unfinished classes: {taggedTypes.Count}");
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft
            };

            if (taggedTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("This window lists all types that have the [Unfinished] attribute.", MessageType.Info);
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                foreach (var entry in taggedTypes)
                {
                    GUILayout.BeginHorizontal();
                    var content = new GUIContent(entry.Key.GetClass().Name, entry.Value == CodeStatus.Draft ? draftIcon : prototypeIcon);
                    if (GUILayout.Button(content, buttonStyle))
                    {
                        Selection.activeObject = entry.Key;
                    }
                    if (GUILayout.Button("Open", GUILayout.Width(60)))
                    {
                        AssetDatabase.OpenAsset(entry.Key);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
        }
    }
}
