using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SceneConductor.Editor
{
    [CustomPropertyDrawer(typeof(SceneList.ScenarioScene))]
    public class ScenarioSceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var scenePathProp = property.FindPropertyRelative("scenePath");
            var sceneList = (SceneList)property.serializedObject.targetObject;

            var sceneNames = sceneList.scenes.Select(s => s.sceneName).ToArray();
            var scenePaths = sceneList.scenes.Select(s => s.scenePath).ToArray();
            var selectedSceneIndex = System.Array.IndexOf(scenePaths, scenePathProp.stringValue);

            EditorGUI.BeginChangeCheck();
            selectedSceneIndex = EditorGUI.Popup(position, label.text, selectedSceneIndex, sceneNames);
            if (EditorGUI.EndChangeCheck() && selectedSceneIndex >= 0 && selectedSceneIndex < sceneList.scenes.Count)
            {
                scenePathProp.stringValue = scenePaths[selectedSceneIndex];
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}