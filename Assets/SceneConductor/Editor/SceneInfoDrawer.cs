using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SceneConductor
{
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        private Color highlightColor = new Color(0.4f, 0.4f, 0.6f); // Light blue color

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var loadOnStartProp = property.FindPropertyRelative("loadOnStart");
            var sceneNameProp = property.FindPropertyRelative("sceneName");
            var scenePathProp = property.FindPropertyRelative("scenePath");

            // Highlight background if loadOnStart is true
            if (loadOnStartProp.boolValue)
            {
                EditorGUI.DrawRect(position, highlightColor);
            }

            float halfWidth = position.width / 2;
            float checkBoxWidth = 20.0f;

            // First Line: Load on Start, Scene Name
            Rect loadOnStartRect = new Rect(position.x + 5, position.y, checkBoxWidth , EditorGUIUtility.singleLineHeight);
            Rect sceneNameRect = new Rect(position.x + checkBoxWidth + 5, position.y, position.width - checkBoxWidth - 5,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(loadOnStartRect, loadOnStartProp, new GUIContent(""));
            EditorGUI.LabelField(sceneNameRect, new GUIContent(sceneNameProp.stringValue));

            // Second Line: Load, Unload, Remove buttons
            Rect buttonsRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width,
                EditorGUIUtility.singleLineHeight);

            float buttonWidth = buttonsRect.width / 3 - 5;

            if (GUI.Button(new Rect(buttonsRect.x, buttonsRect.y, buttonWidth, buttonsRect.height), "Load"))
            {
                EditorSceneManager.OpenScene(scenePathProp.stringValue, OpenSceneMode.Additive);
            }

            if (GUI.Button(new Rect(buttonsRect.x + buttonWidth + 5, buttonsRect.y, buttonWidth, buttonsRect.height),
                    "Unload"))
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneNameProp.stringValue), false);
            }

            if (GUI.Button(
                    new Rect(buttonsRect.x + 2 * (buttonWidth + 5), buttonsRect.y, buttonWidth, buttonsRect.height),
                    "Remove"))
            {
                EditorSceneManager.CloseScene(SceneManager.GetSceneByName(sceneNameProp.stringValue), true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }
    }
}