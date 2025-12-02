using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    private static int loadingSceneIndex = 1;

    public float currentLoadingProgress {private set; get;}
    
    [Header("Settings")]
    public float minLoadTime = 1.0f;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static void LoadSceneStatic(int sceneIndex)
    {
        Instance.LoadScene(sceneIndex);
    }
    
    public static void LoadSceneStatic(string sceneName)
    {
        Instance.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        //For safety purpose
        if (sceneIndex != loadingSceneIndex)
        {
            Instance.StartCoroutine(Instance.LoadAsyncScene(sceneIndex));
        }
    }
    
    public void LoadScene(string sceneName)
    {
        //For safety purpose
        if (sceneName != SceneManager.GetSceneByBuildIndex(loadingSceneIndex).name)
        {
            Instance.StartCoroutine(Instance.LoadAsyncScene(sceneName));
        }
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(loadingSceneIndex);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;


        currentLoadingProgress = 0;
        float timer = 0f;


        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            float fakeProgress = Mathf.Clamp01(timer / minLoadTime);

            float realProgress = Mathf.Clamp01(asyncLoad.progress / .9f);

            currentLoadingProgress = Mathf.Min(fakeProgress, realProgress);


            //Check condition to exit load
            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                currentLoadingProgress = 1f;
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    IEnumerator LoadAsyncScene(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(loadingSceneIndex);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;
        
        
        currentLoadingProgress = 0;
        float timer = 0f;
        
        
        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;
            
            float fakeProgress = Mathf.Clamp01(timer / minLoadTime);
            
            float realProgress = Mathf.Clamp01(asyncLoad.progress / .9f);
            
            currentLoadingProgress = Mathf.Min(fakeProgress, realProgress);
            
            
            //Check condition to exit load
            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                currentLoadingProgress = 1f;
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
}
