using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

namespace SceneConductor
{
    public class SceneLoader : MonoBehaviour
    {
        public SceneList sceneList;
        async void Start()
        {
            if (sceneList != null)
            {
                await sceneList.UnloadAllScenes();
                await sceneList.LoadScenesOnStart();
            }
        }
        
    }
}