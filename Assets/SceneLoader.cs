using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    LevelSetup sceneSetup;
    Scene scene;
    Scene allScenes;
    [SerializeField]
    PlayerController playerController;
    Camera cam;
    [SerializeField]
    UIDocument mainUI;
    public Action AllScenesLoaded;
    private void SceneClosed(Scene scene, bool removingScene)
    {
        if (this.scene == scene)
        {
            
            EditorSceneManager.sceneOpened -= EditorLoadScene;
            EditorSceneManager.sceneClosing -= SceneClosed;
            //sceneSetup.sceneSetup = EditorSceneManager.GetSceneManagerSetup();

        }
    }

    private void EditorLoadScene(Scene scene, OpenSceneMode mode)
    {
        scene = EditorSceneManager.GetActiveScene();
        if (sceneSetup.sceneSetup.Length > 0 && scene.path == sceneSetup.sceneSetup[0].path)
        {
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup.sceneSetup);
        }


    }
    private void OnValidate()
    {
        scene=EditorSceneManager.GetActiveScene();
        EditorSceneManager.sceneOpened += EditorLoadScene;
        EditorSceneManager.sceneClosing += SceneClosed;
    }



    private void Awake()
    {
        cam = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        mainUI.rootVisualElement.Q("LoadingScreen").visible = true;
        playerController.gameObject.SetActive(false);

        //cam.enabled = false;
        StartCoroutine(LoadWorld());
        //Invoke("LoadWorld", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadWorld()
    {
        
        for (int i=0;i<sceneSetup.sceneSetup.Length;i++)
        {
            string name = Path.GetFileNameWithoutExtension(sceneSetup.sceneSetup[i].path);
            
            if (name == SceneManager.GetActiveScene().name)
            {
                continue;
            }
            bool sceneAlreadyLoaded=false;
            var scene = SceneManager.GetSceneByPath(sceneSetup.sceneSetup[i].path);
            if (scene.name==name)
            {
                sceneAlreadyLoaded=true;
            }
           
            //for (int j = 0; j < SceneManager.loadedSceneCount; j++) 
            //{
            //    if (SceneManager.GetSceneAt(j).name == name)
            //    {
            //        sceneAlreadyLoaded = true;
            //        break;
            //    }
            //}
            if (sceneAlreadyLoaded|| name == null)
            {
                continue;
            }
            yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        ScenesLoaded();
        yield return new WaitForSecondsRealtime(0.25f);

    }

    void ScenesLoaded()
    {
        playerController.gameObject.SetActive(true);
        //cam.enabled = true;
        mainUI.rootVisualElement.Remove(mainUI.rootVisualElement.Q("LoadingScreen"));
        AllScenesLoaded?.Invoke();
        Time.timeScale = 1.0f;

    }
}
