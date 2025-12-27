#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

// -----------------------------------------------------------
// 1. The Logic Helper (Handles finding and loading scenes)
// -----------------------------------------------------------
public static class SceneSwitcherLogic
{
    public static bool FetchAllScenes
    {
        get => EditorPrefs.GetBool("SceneSwitcher_FetchAllScenes", false);
        set => EditorPrefs.SetBool("SceneSwitcher_FetchAllScenes", value);
    }

    public static string[] GetScenePaths()
    {
        if (FetchAllScenes)
        {
            // Fetch all scenes in project
            return Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
        }
        else
        {
            // Only enabled scenes in Build Settings
            return EditorBuildSettings.scenes
                .Where(s => s.enabled && File.Exists(s.path))
                .Select(s => s.path)
                .ToArray();
        }
    }

    public static void LoadScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}

// -----------------------------------------------------------
// 2. The Toggle Button ("All Scenes")
// -----------------------------------------------------------
[EditorToolbarElement(id, typeof(SceneView))]
public class SceneSwitcherToggle : EditorToolbarToggle
{
    public const string id = "SceneSwitcher/Toggle";

    public SceneSwitcherToggle()
    {
        text = "All Scenes";
        tooltip = "If ON, searches the whole project. If OFF, uses Build Settings only.";
        
        // Sync state with EditorPrefs
        value = SceneSwitcherLogic.FetchAllScenes;

        // update pref when clicked
        this.RegisterValueChangedCallback(evt => {
            SceneSwitcherLogic.FetchAllScenes = evt.newValue;
        });
    }
}

// -----------------------------------------------------------
// 3. The Dropdown Menu (Scene Selector)
// -----------------------------------------------------------
[EditorToolbarElement(id, typeof(SceneView))]
public class SceneSwitcherDropdown : EditorToolbarDropdown
{
    public const string id = "SceneSwitcher/Dropdown";

    public SceneSwitcherDropdown()
    {
        tooltip = "Select a scene to load";
        clicked += ShowMenu;
        
        // Subscribe to scene changes to update the label
        EditorSceneManager.activeSceneChangedInEditMode += UpdateLabel;
        UpdateLabel(default, EditorSceneManager.GetActiveScene());
    }

    ~SceneSwitcherDropdown()
    {
        EditorSceneManager.activeSceneChangedInEditMode -= UpdateLabel;
    }

    private void UpdateLabel(UnityEngine.SceneManagement.Scene prev, UnityEngine.SceneManagement.Scene current)
    {
        text = string.IsNullOrEmpty(current.path) ? "Untitled" : current.name;
    }

    private void ShowMenu()
    {
        var menu = new GenericMenu();
        string[] paths = SceneSwitcherLogic.GetScenePaths();
        string currentPath = EditorSceneManager.GetActiveScene().path;

        if (paths.Length == 0)
        {
            menu.AddDisabledItem(new GUIContent("No scenes found"));
        }

        foreach (var path in paths)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            bool isCurrent = path == currentPath;

            // Add the item to the menu
            menu.AddItem(new GUIContent(name), isCurrent, () => {
                SceneSwitcherLogic.LoadScene(path);
            });
        }

        menu.ShowAsContext();
    }
}

// -----------------------------------------------------------
// 4. The Overlay Container (Groups them together)
// -----------------------------------------------------------
[Overlay(typeof(SceneView), "Scene Switcher", true)]
public class SceneSwitcherOverlay : ToolbarOverlay
{
    // Register the elements we defined above
    SceneSwitcherOverlay() : base(
        SceneSwitcherToggle.id,
        SceneSwitcherDropdown.id
    )
    { }
}
#endif