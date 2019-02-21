//Writer: Xingrong

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    [SerializeField]
    public int sceneNum;

    public void SceneSelected (int num)
    { 
        sceneNum = num;
	}

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneNum);
    }
}
