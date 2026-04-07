using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEndTrigger : MonoBehaviour
{
    public string NameNextScene;
  
    public void OnSceneEnd()
    {
        DayManager.Instance.DayUpdate();

        SceneManager.LoadScene(NameNextScene);
    }
}
