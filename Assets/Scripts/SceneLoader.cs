using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string TargetedScene;

    public void GoToScene (string name)
    {
        SceneManager.LoadScene(name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
