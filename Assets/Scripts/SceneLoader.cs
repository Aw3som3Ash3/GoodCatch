using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
public class SceneLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    LevelSetup editorSceneSetup;
#endif
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
       
        this.scene = EditorSceneManager.GetActiveScene();
        if (editorSceneSetup.sceneSetup.Length > 0 && scene.path == editorSceneSetup.sceneSetup[0].path)
        {
            EditorSceneManager.RestoreSceneManagerSetup(editorSceneSetup.sceneSetup);
        }


    }
    private void OnValidate()
    {
        scene=EditorSceneManager.GetActiveScene();
        EditorSceneManager.sceneOpened += EditorLoadScene;
        EditorSceneManager.sceneClosing += SceneClosed;
        foreach(var scene in editorSceneSetup.sceneSetup)
        {
            allScenes.Add(scene.path);
        }
    }

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
        
        for (int i=0;i<allScenes.Count;i++)
        {
            string name = Path.GetFileNameWithoutExtension(allScenes[i]);
            
            if (name == SceneManager.GetActiveScene().name)
            {
                continue;
            }
            bool sceneAlreadyLoaded=false;
            var scene = SceneManager.GetSceneByPath(allScenes[i]);
            if (scene.name==name)
            {
                sceneAlreadyLoaded=true;
            }
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
