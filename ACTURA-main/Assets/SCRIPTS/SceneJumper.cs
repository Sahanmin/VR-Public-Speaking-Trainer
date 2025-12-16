using UnityEngine;

public class SceneJumper : MonoBehaviour
{
    // Call this method to load a scene by its build index
    public void JumpToScene(int sceneIndex)
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
#else
            Application.LoadLevel(sceneIndex);
#endif
    }
}