#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FullscreenGameView
{
    private static EditorWindow _fullscreenWindow;
    
    // Set your desired resolution here
    private const int TargetWidth = 1920;
    private const int TargetHeight = 1080;

    static FullscreenGameView()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EditorApplication.isPlaying && Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreen();
        }
    }

    [MenuItem("Window/General/Fixed 1080p Window (F11) %F11")]
    public static void ToggleFullscreen()
    {
        if (_fullscreenWindow != null)
        {
            _fullscreenWindow.Close();
            _fullscreenWindow = null;
            return;
        }

        Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
        _fullscreenWindow = (EditorWindow)ScriptableObject.CreateInstance(gameViewType);
        
        _fullscreenWindow.ShowPopup();

        // --- NEW LOGIC START ---
        
        // Get your monitor's resolution
        Resolution monitorRes = Screen.currentResolution;
        
        // Calculate the X and Y position to center the window
        float xPos = (monitorRes.width - TargetWidth) / 2f;
        float yPos = (monitorRes.height - TargetHeight) / 2f;

        // Ensure we don't spawn off-screen if the monitor is smaller than 1080p
        if (xPos < 0) xPos = 0;
        if (yPos < 0) yPos = 0;

        // Apply the fixed 1920x1080 size
        _fullscreenWindow.position = new Rect(xPos, yPos, TargetWidth, TargetHeight);
        
        // --- NEW LOGIC END ---

        _fullscreenWindow.Focus();
    }
}
#endif