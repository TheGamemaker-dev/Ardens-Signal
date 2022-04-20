using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class MenuItems
{
    [MenuItem("Tools/Play From Start")]
    static void Play()
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/Start.unity");
        EditorApplication.isPlaying = true;
    }
}
