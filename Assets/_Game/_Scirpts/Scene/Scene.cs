using UnityEngine;
using UnityEngine.SceneManagement;


public class Scene : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
}
