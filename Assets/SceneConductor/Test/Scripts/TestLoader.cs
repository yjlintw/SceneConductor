using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneConductor.Example
{
    public class TestLoader : MonoBehaviour
    {
        public SceneList sceneList;

        // Update is called once per frame
        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                await sceneList.LoadScenario(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                await sceneList.LoadScenario(TestSceneListScenarioAlias.GameStage2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                await sceneList.LoadScenario(2);
            }

        }
    }
}