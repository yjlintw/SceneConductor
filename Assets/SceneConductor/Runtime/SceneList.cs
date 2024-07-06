using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SceneConductor.Helper;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneConductor
{
    [CreateAssetMenu(fileName = "SceneList", menuName = "SceneConductor/SceneList", order = 1)]
    public class SceneList : ScriptableObject
    {
        [System.Serializable]
        public class Scenario
        {
            public string alias;
            public List<ScenarioScene> scenes = new List<ScenarioScene>();
        }

        [System.Serializable]
        public class ScenarioScene
        {
            public string scenePath;
        }

        public List<SceneInfo> scenes = new List<SceneInfo>();
        public Scenario preLoadScenes;
        public List<Scenario> scenarios = new List<Scenario>();
        public Scenario postLoadScenes;

        private void OnValidate()
        {
            UpdateSceneList();
        }

        public void UpdateSceneList()
        {
            var buildScenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();

            scenes = buildScenes.Select(scenePath => new SceneInfo
            {
                scenePath = scenePath,
                sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath),
                loadOnStart = scenes.Any(s =>
                    s.sceneName == System.IO.Path.GetFileNameWithoutExtension(scenePath) && s.loadOnStart)
            }).ToList();
        }

        public async Task UnloadAllScenes()
        {
            var scenesToUnload = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt)
                .Where(scene => scene.isLoaded && scene.name != SceneManager.GetActiveScene().name)
                .ToList();

            foreach (var scene in scenesToUnload)
            {
                var unloadSceneOperation = SceneManager.UnloadSceneAsync(scene);
                while (!unloadSceneOperation.isDone)
                {
                    await Task.Yield();
                }
            }
        }

        public async Task LoadScenesOnStart()
        {
            foreach (var sceneInfo in scenes.Where(s => s.loadOnStart))
            {
                await LoadScene(sceneInfo.scenePath);
            }
        }

        public async Task LoadScene(string scenePath)
        {
            if (!SceneManager.GetSceneByPath(scenePath).isLoaded)
            {
                var loadSceneOperation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                while (!loadSceneOperation.isDone)
                {
                    await Task.Yield();
                }
            }
        }

        public async Task LoadScenario(int scenarioIndex)
        {
            if (scenarioIndex < 0 || scenarioIndex >= scenarios.Count)
            {
                Debug.LogError($"Invalid scenario index: {scenarioIndex}");
                return;
            }

            Scenario scenario = scenarios[scenarioIndex];
            await LoadScenarioInternal(scenario);
        }

        public async Task LoadScenario(Enum scenarioEnum)
        {
            string scenarioAlias = scenarioEnum.ToString();

            Scenario scenario = scenarios.FirstOrDefault(s => s.alias == scenarioAlias);

            if (scenario == null)
            {
                Debug.LogError($"Scenario with alias \"{scenarioAlias}\" not found.");
                return;
            }

            await LoadScenarioInternal(scenario);
        }

        private async Task LoadScenarioInternal(Scenario scenario)
        {
            var targetScenePaths = new HashSet<string>(scenario.scenes.Select(s => s.scenePath));
            var currentlyLoadedScenes = new HashSet<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    currentlyLoadedScenes.Add(scene.path);
                }
            }

            var alwaysLoadedScenes = new HashSet<string>(preLoadScenes.scenes.Select(s => s.scenePath)
                .Concat(postLoadScenes.scenes.Select(s => s.scenePath)));

            // Unload scenes not in the target scenario and not in the always-loaded scenes
            foreach (var scenePath in currentlyLoadedScenes)
            {
                if (!targetScenePaths.Contains(scenePath) && !alwaysLoadedScenes.Contains(scenePath))
                {
                    var unloadSceneOperation = SceneManager.UnloadSceneAsync(scenePath);
                    while (!unloadSceneOperation.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }

            // Load scenes to be loaded before the scenario
            foreach (var scene in preLoadScenes.scenes)
            {
                if (!currentlyLoadedScenes.Contains(scene.scenePath))
                {
                    var loadSceneOperation = SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);
                    while (!loadSceneOperation.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }

            // Load scenes in the target scenario that are not yet loaded
            foreach (var scene in scenario.scenes)
            {
                if (!currentlyLoadedScenes.Contains(scene.scenePath))
                {
                    var loadSceneOperation = SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);
                    while (!loadSceneOperation.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }

            // Load scenes to be loaded after the scenario
            foreach (var scene in postLoadScenes.scenes)
            {
                if (!currentlyLoadedScenes.Contains(scene.scenePath))
                {
                    var loadSceneOperation = SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);
                    while (!loadSceneOperation.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }
        }
        // public async Task LoadScenario(int scenarioIndex)
        // {
        //     if (scenarioIndex < 0 || scenarioIndex >= scenarios.Count)
        //     {
        //         Debug.LogError("Scenario index out of range");
        //         return;
        //     }
        //
        //     var targetScenario = scenarios[scenarioIndex];
        //     var targetScenePaths = new HashSet<string>(targetScenario.scenes.Select(s => s.scenePath));
        //     var currentlyLoadedScenes = new HashSet<string>();
        //
        //     for (int i = 0; i < SceneManager.sceneCount; i++)
        //     {
        //         var scene = SceneManager.GetSceneAt(i);
        //         if (scene.isLoaded)
        //         {
        //             currentlyLoadedScenes.Add(scene.path);
        //         }
        //     }
        //
        //     var alwaysLoadedScenes = new HashSet<string>(preLoadScenes.scenes.Select(s => s.scenePath)
        //         .Concat(postLoadScenes.scenes.Select(s => s.scenePath)));
        //     
        //     // Unload scenes not in the target scenario
        //     foreach (var scenePath in currentlyLoadedScenes)
        //     {
        //         if (!targetScenePaths.Contains(scenePath) && !alwaysLoadedScenes.Contains(scenePath))
        //         {
        //             var unloadSceneOperation = SceneManager.UnloadSceneAsync(scenePath);
        //             while (!unloadSceneOperation.isDone)
        //             {
        //                 await Task.Yield();
        //             }
        //         }
        //     }
        //
        //     // Load scenes in the target scenario that are not yet loaded
        //     foreach (var scene in targetScenario.scenes)
        //     {
        //         if (!currentlyLoadedScenes.Contains(scene.scenePath))
        //         {
        //             var loadSceneOperation = SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);
        //             while (!loadSceneOperation.isDone)
        //             {
        //                 await Task.Yield();
        //             }
        //         }
        //     }
        // }
        //
#if UNITY_EDITOR
        public void UnloadAllScenesEditor()
        {
            var scenesToUnload = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt)
                .Where(scene => scene.isLoaded && scene.name != SceneManager.GetActiveScene().name)
                .ToList();

            foreach (var scene in scenesToUnload)
            {
                EditorSceneManager.CloseScene(scene, true);
            }

        }

        public void LoadScenesOnStartEditor()
        {
            foreach (var sceneInfo in scenes.Where(s => s.loadOnStart))
            {
                LoadSceneEditor(sceneInfo.scenePath);
            }
        }

        public void LoadSceneEditor(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }
#endif
    }
}