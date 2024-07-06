using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace SceneConductor.Helper
{
    public static class ScenarioEnumGenerator
    {
        private const string EnumFolder = "Assets/SceneConductor/Generated/GeneratedEnums/";

        [MenuItem("Tools/Generate All Scenario Enums")]
        public static void GenerateAllScenarioEnums()
        {
            string[] sceneListPaths = AssetDatabase.FindAssets("t:SceneList");

            foreach (var sceneListPath in sceneListPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneListPath);
                SceneList sceneList = AssetDatabase.LoadAssetAtPath<SceneList>(assetPath);

                if (sceneList != null)
                {
                    GenerateScenarioEnum(sceneList);
                }
            }
        }

        public static void GenerateScenarioEnum(SceneList sceneList)
        {
            if (sceneList == null)
            {
                Debug.LogError("SceneList is null.");
                return;
            }

            string enumName = $"{sceneList.name}ScenarioAlias";
            string enumFilePath = EnumFolder + enumName + ".cs";

            StringBuilder enumBuilder = new StringBuilder();
            enumBuilder.AppendLine($"public enum {enumName}");
            enumBuilder.AppendLine("{");

            foreach (var scenario in sceneList.scenarios)
            {
                string enumEntry = scenario.alias.Replace(" ", "_"); // Replace spaces with underscores
                enumBuilder.AppendLine($"    {enumEntry},");
            }

            enumBuilder.AppendLine("}");

            // Write enum to file
            Directory.CreateDirectory(EnumFolder); // Ensure directory exists
            File.WriteAllText(enumFilePath, enumBuilder.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"Scenario enum \"{enumName}\" generated successfully at {enumFilePath}");
        }
    }
}