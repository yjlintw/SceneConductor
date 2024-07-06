using System.Collections;
using System.Collections.Generic;
using SceneConductor.Helper;
using UnityEditor;
using UnityEngine;

namespace SceneConductor.Editor
{
    [CustomEditor(typeof(SceneList))]
    public class SceneListEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SceneList sceneList = (SceneList)target;
            
            if (GUILayout.Button("Load All On Start Scenes"))
            {
                sceneList.UnloadAllScenesEditor();
                sceneList.LoadScenesOnStartEditor();
            }

            
            DrawDefaultInspector();

            if (GUILayout.Button("Update Scene List"))
            {
                sceneList.UpdateSceneList();
                EditorUtility.SetDirty(sceneList);
                ScenarioEnumGenerator.GenerateScenarioEnum(sceneList);
            }

        }
    }
}