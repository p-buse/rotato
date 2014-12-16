using UnityEngine;
using System.Collections;

public class ReturnToMenu : MonoBehaviour {

	public int sceneToLoad = 0;

    void Start()
    {
        GameData.instance.ChangeUnlockedLevel(Application.loadedLevel + 1);
        Application.LoadLevel(sceneToLoad);
    }
}
