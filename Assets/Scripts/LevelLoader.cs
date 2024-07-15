using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    static LevelLoader Singleton;

    [SerializeField] SceneRef menu;
    [SerializeField] GameObject[] destroyOnLoad;
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
            DestroyImmediate(gameObject);
            return;
        }
        Singleton = this;


        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        DontDestroyOnLoad(gameObject);
        LoadMenu();
    }

    public static void LoadLevel(string sceneName, bool reload = true)
    {
        Singleton.StartCoroutine(LoadLevelCoroutine(sceneName, reload));
    }
    public static IEnumerator LoadLevelCoroutine(string sceneName, bool reload)
    {
        // invalid name
        if (string.IsNullOrEmpty(sceneName))
        {
            yield break;
        }

        sceneName = sceneName.ToLower();

        // level already loaded and dont want to reload
        if (!reload && IsLevelLoaded(sceneName))
        {
            yield break;
        }

        // EsceneLoadingTransition.Show(!string.IsNullOrEmpty(unloadedLevelName));

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        LoadedLevels.Add(sceneName);

        Debug.Log($"Level loaded: {sceneName}");
        onLoadLevel?.Invoke(sceneName);
    }

    public static void UnloadLevel(string levelName)
    {
        Singleton.StartCoroutine(UnloadLevelCoroutine(levelName));
    }


    public static IEnumerator UnloadLevelCoroutine(string levelName)
    {

        // Comienza la descarga de la escena de manera asíncrona
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(levelName);

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

    public static bool IsLevelLoaded(string levelName)
    {
        // get scene object (This is case insensitive)
        Scene scene = SceneManager.GetSceneByName(levelName);

        // invalid scene
        if (!scene.IsValid())
            return false;

        return scene.isLoaded;
    }



    static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //EsceneLoadingTransition.Show(false);

        onLoadedLevel.Invoke(scene.name);
        if (scene.name == Singleton.menu.SceneName || scene.name.ToLower() == "kitchen")
            EsceneLoadingTransition.Show(false);
    }

    static void OnSceneUnloaded(Scene scene)
    {
        onUnloadedLevel.Invoke(scene.name);
    }


    public static void LoadMenu()
    {
        EsceneLoadingTransition.Show(true);
        Singleton.Invoke(nameof(DelayedLoadMenu), 2f);
    }

    void DelayedLoadMenu()
    {
        // load menu
        LoadLevel(Singleton.menu.SceneName);

        // unload gameplay
        foreach (var level in Singleton.gameplayLevels)
        {
            UnloadLevel(level.SceneName);
        }
    }

    public static void LoadGameplayLevels()
    {
        EsceneLoadingTransition.Show(true);
        Singleton.Invoke(nameof(DelayedLoadGameplayLevels), 2f);
    }

    void DelayedLoadGameplayLevels()
    {
        // load gameplay
        foreach (var level in Singleton.gameplayLevels)
        {
            LoadLevel(level.SceneName);
        }

        // unload menu
        UnloadLevel(Singleton.menu.SceneName);
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
