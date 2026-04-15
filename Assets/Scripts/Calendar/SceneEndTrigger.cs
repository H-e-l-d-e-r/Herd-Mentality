using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEndTrigger : MonoBehaviour
{
    public string NameNextScene;
  
    public void OnSceneEnd()
    {
        GameManager.Instance.NextDay();

        SceneManager.LoadScene(NameNextScene);
    }
}
