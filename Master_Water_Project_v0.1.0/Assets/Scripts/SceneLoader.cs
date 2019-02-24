//Writer: Xingrong

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    [SerializeField]
    public int sceneNum;

    public void SceneSelected (int num)
    { 
        sceneNum = num;
        Debug.Log("SCENE NUMBER" + sceneNum);
	}

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneNum);
        Debug.Log("SCENE N" + sceneNum);
    }
}
