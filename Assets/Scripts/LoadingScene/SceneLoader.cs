using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    
    public float CurrentLoadingProgress { get; private set; }

    [Header("Settings")]
    public float minLoadTime = 1.0f;            // Minimum wait time
    public Canvas defaultTransitionCanvas;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Static Access Points ---
    public static void LoadScene(int sceneIndex) => Instance.LoadTargetScene(sceneIndex).Forget();
    public static void LoadScene(string sceneName) => Instance.LoadTargetScene(sceneName).Forget();

    public static void LoadSceneWithTransitionCanvas(int sceneIndex, Canvas transitionCanvas) =>
        Instance.LoadTargetScene(sceneIndex, transitionCanvas).Forget();
    public static void LoadSceneWithTransitionCanvas(string sceneName, Canvas transitionCanvas) =>
        Instance.LoadTargetScene(sceneName, transitionCanvas).Forget();

    // --- Main Logic ---
    private async UniTaskVoid LoadTargetScene(object sceneIdentifier,Canvas transitionCanvas)
    {
        Canvas transitionInstance = Instantiate(transitionCanvas);
        
        DontDestroyOnLoad(transitionInstance.gameObject);

        PlayableDirector transitionTimeline = transitionInstance.GetComponent<PlayableDirector>();
        
        await UniTask.Yield();
        
        // 1. Play Intro Animation (Fade In / Close Curtain)
        if (transitionTimeline != null)
        {
            transitionTimeline.time = 0; // Reset timeline to start
            transitionTimeline.Play();
        }

        // Wait for the Intro animation to finish covering the screen
        await UniTask.Delay(TimeSpan.FromSeconds(transitionTimeline.duration));

        // 2. Pause Timeline (Hold the black screen)
        if (transitionTimeline != null)
        {
            transitionTimeline.Pause();
        }

        // 3. Start Loading Async
        AsyncOperation asyncLoad;
        
        // Determine if we are loading by Index or Name
        if (sceneIdentifier is int index)
            asyncLoad = SceneManager.LoadSceneAsync(index);
        else
            asyncLoad = SceneManager.LoadSceneAsync((string)sceneIdentifier);

        // Prevent scene from switching immediately
        asyncLoad.allowSceneActivation = false;

        CurrentLoadingProgress = 0;
        float timer = 0f;

        // 4. Loading Loop
        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            // Calculate Progress
            float fakeProgress = Mathf.Clamp01(timer / minLoadTime);
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Unity stops at 0.9

            CurrentLoadingProgress = Mathf.Min(fakeProgress, realProgress);

            // Check completion conditions
            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                CurrentLoadingProgress = 1f;
                asyncLoad.allowSceneActivation = true;
            }

            await UniTask.Yield(); // Wait for next frame
        }

        // 5. Outro
        // Wait one frame to ensure the new Scene's Awake/Start methods run
        await UniTask.Yield();

        if (transitionTimeline != null)
        {
            transitionTimeline.Resume();
        
            // Calculate remaining time properly
            double remainingTime = transitionTimeline.duration - transitionTimeline.time;
        
            // Wait for Outro to finish
            // Use Math.Max to prevent errors if calculation is negative
            await UniTask.Delay(
                TimeSpan.FromSeconds(Math.Max(0, remainingTime)), 
                ignoreTimeScale: false, 
                cancellationToken: CancellationToken.None
            );
        
            transitionTimeline.Stop();
        }
        
        if (transitionInstance != null)
        {
            Destroy(transitionInstance.gameObject);
        }
    }
    private async UniTaskVoid LoadTargetScene(object sceneIdentifier)
    {

        // Start Loading Async
        AsyncOperation asyncLoad;
        
        // Determine if we are loading by Index or Name
        if (sceneIdentifier is int index)
            asyncLoad = SceneManager.LoadSceneAsync(index);
        else
            asyncLoad = SceneManager.LoadSceneAsync((string)sceneIdentifier);

        // Prevent scene from switching immediately
        asyncLoad.allowSceneActivation = false;

        CurrentLoadingProgress = 0;
        float timer = 0f;

        // 4. Loading Loop
        while (!asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            // Calculate Progress
            float fakeProgress = Mathf.Clamp01(timer / minLoadTime);
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Unity stops at 0.9

            CurrentLoadingProgress = Mathf.Min(fakeProgress, realProgress);

            // Check completion conditions
            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                CurrentLoadingProgress = 1f;
                asyncLoad.allowSceneActivation = true;
            }

            await UniTask.Yield(); // Wait for next frame
        }

        // 5. Outro
        // Wait one frame to ensure the new Scene's Awake/Start methods run
        await UniTask.Yield();

    }
    
}
