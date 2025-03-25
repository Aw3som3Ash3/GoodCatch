using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoader : MonoBehaviour
{
   
    Scene scene;
    [SerializeField]
    List<string> allScenes;
    [SerializeField]
    PlayerController playerController;
    Camera cam;
    [SerializeField]
    UIDocument mainUI;
    public Action AllScenesLoaded;
#if UNITY_EDITOR
    //[SerializeField]
    //LevelSetup sceneSetup;
    //private void SceneClosed(Scene scene, bool removingScene)
    //{
    //    if (this.scene == scene)
    //    {
            
    //        EditorSceneManager.sceneOpened -= EditorLoadScene;
    //        EditorSceneManager.sceneClosing -= SceneClosed;
    //        //sceneSetup.sceneSetup = EditorSceneManager.GetSceneManagerSetup();

    //    }
    //}

    //private void EditorLoadScene(Scene scene, OpenSceneMode mode)
    //{
    //    scene = EditorSceneManager.GetActiveScene();
    //    if (sceneSetup.sceneSetup.Length > 0 && scene.path == sceneSetup.sceneSetup[0].path)
    //    {
    //        EditorSceneManager.RestoreSceneManagerSetup(sceneSetup.sceneSetup);
    //    }


    //}
    //private void OnValidate()
    //{
    //    scene=EditorSceneManager.GetActiveScene();
    //    EditorSceneManager.sceneOpened += EditorLoadScene;
    //    EditorSceneManager.sceneClosing += SceneClosed;
    //    allScenes.Clear();
    //    foreach(var scene in sceneSetup.sceneSetup)
    //    {
    //        allScenes.Add(scene.path);
    //    }
    //}

#endif

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
        for(int i = 0; i < allScenes.Count; i++)
        {
            if (SceneManager.GetSceneByName(allScenes[i]).IsValid())
            {
                continue;
            }
            yield return SceneManager.LoadSceneAsync(allScenes[i], LoadSceneMode.Additive);
        }
        yield return new WaitForEndOfFrame();
        ScenesLoaded();

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
