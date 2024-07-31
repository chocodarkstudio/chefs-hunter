using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    static LevelLoader Singleton;

    [SerializeField] SceneRef menu;
    [SerializeField] SceneRef[] gameplayLevels;

    public static readonly List<string> LoadedLevels = new();

    // events
    /// <summary> Scene that has been started to Load. </summary>
    public static readonly UnityEvent<string> onLoadLevel = new();
    /// <summary> Scene that has been started to Unload. </summary>
    public static readonly UnityEvent<string> onUnloadLevel = new();

    /// <summary> Scene that has been completely Loaded. </summary>
    public static readonly UnityEvent<string> onLoadedLevel = new();
    /// <summary> Scene that has been completely Unloaded. </summary>
    public static readonly UnityEvent<string> onUnloadedLevel = new();


    private void Awake()
    {
        if (Singleton != null)
        {
            //DestroyImmediate(gameObject);
            Debug.LogWarning($"Another instance of {nameof(LevelLoader)} already exists!! Make sure only one exists");
            return;
        }
        Singleton = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public static void LoadLevel(string sceneName, bool reload = true, bool makeActive = false)
    {
        Singleton.StartCoroutine(LoadLevelCoroutine(sceneName, reload, makeActive));
    }
    public static IEnumerator LoadLevelCoroutine(string sceneName, bool reload, bool makeActive)
    {
        // invalid scene
        if (string.IsNullOrEmpty(sceneName))
            yield break;

        // level already loaded and dont want to reload
        if (!reload && IsLevelLoaded(sceneName))
        {
            yield break;
        }

        // EsceneLoadingTransition.Show(!string.IsNullOrEmpty(unloadedLevelName));

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        LoadedLevels.Add(sceneName);

        if (makeActive)
        {
            // get scene object (This is case insensitive)
            Scene scene = SceneManager.GetSceneByName(sceneName);

            // invalid scene
            if (!scene.IsValid())
                yield break;

            SceneManager.SetActiveScene(scene);
        }

        Debug.Log($"Level loaded: {sceneName}");
        onLoadLevel?.Invoke(sceneName);
    }

    public static void UnloadLevel(string levelName)
    {
        Singleton.StartCoroutine(UnloadLevelCoroutine(levelName));
    }


    public static IEnumerator UnloadLevelCoroutine(string levelName)
    {
        AsyncOperation asyncOperation = null;
        try
        {
            // Comienza la descarga de la escena de manera asíncrona
            asyncOperation = SceneManager.UnloadSceneAsync(levelName);
        }
        catch (ArgumentException)
        {
            Debug.Log($"Scene to unload is invalid: {levelName}");
        }


        // Esperar hasta que la operación de descarga esté completa
        if (asyncOperation != null)
        {
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

        // Eliminar el nivel de la lista de niveles cargados
        LoadedLevels.Remove(levelName);

        // Invocar el evento de descarga de nivel
        if (onUnloadLevel != null)
        {
            onUnloadLevel.Invoke(levelName);
        }
    }



    static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //EsceneLoadingTransition.Show(false);

        onLoadedLevel.Invoke(scene.name);
    }

    static void OnSceneUnloaded(Scene scene)
    {
        onUnloadedLevel.Invoke(scene.name);
    }


    public static void LoadMenu(float waitSeconds = 2f)
    {
        EsceneLoadingTransition.Show(true);
        Singleton.StartCoroutine(Singleton.DelayedLoadMenu(waitSeconds, callback: () =>
        {
            EsceneLoadingTransition.Show(false);
        }));
    }

    IEnumerator DelayedLoadMenu(float waitSeconds, Action callback = null)
    {
        yield return new WaitForSeconds(waitSeconds);

        // load menu
        yield return LoadLevelCoroutine(Singleton.menu.SceneName, reload: false, makeActive: Singleton.menu.MakeActiveOnLoad);

        // unload gameplay
        foreach (var level in Singleton.gameplayLevels)
        {
            yield return UnloadLevelCoroutine(level.SceneName);
        }

        callback?.Invoke();
    }

    public static void LoadGameplayLevels(float waitSeconds = 2f)
    {
        EsceneLoadingTransition.Show(true);
        Singleton.StartCoroutine(Singleton.DelayedLoadGameplayLevels(waitSeconds, callback: () =>
        {
            EsceneLoadingTransition.Show(false);
        }));
    }

    IEnumerator DelayedLoadGameplayLevels(float waitSeconds, Action callback = null)
    {
        yield return new WaitForSeconds(waitSeconds);

        // load gameplay
        foreach (var level in Singleton.gameplayLevels)
        {
            yield return LoadLevelCoroutine(level.SceneName, reload: false, makeActive: level.MakeActiveOnLoad);
        }

        // unload menu
        yield return UnloadLevelCoroutine(Singleton.menu.SceneName);

        callback?.Invoke();
    }



    public static bool IsLevelLoaded(string levelName)
    {
        // get scene object (This is case insensitive)
        Scene scene = SceneManager.GetSceneByName(levelName);

        // invalid scene
        if (!scene.IsValid())
            return false;

        return scene.isLoaded;
    }


    public static GameObject CreateOnLevel(GameObject prefab, Vector3 pos = default, Quaternion rot = default, Transform parent = null)
    {
        // invalid prefab
        if (prefab == null)
            return null;

        GameObject obj = GameObject.Instantiate(prefab, parent);

        MoveToLevelScene(obj, "Main");

        // set the obj in given position
        obj.transform.position = pos;

        return obj;
    }


    public static bool MoveToLevelScene(GameObject go, string sceneName)
    {
        // get scene object
        Scene? scene = SceneManager.GetSceneByName(sceneName);

        // not valid scene
        if (!scene.HasValue)
            return false;

        SceneManager.MoveGameObjectToScene(go, scene.Value);
        return true;
    }


}
