using UnityEngine;
using System.Collections;

public class ReturnToMenu : MonoBehaviour {

    void Start()
    {
        GameData.instance.ChangeUnlockedLevel(Application.loadedLevel + 1);
        Application.LoadLevel(0);
    }
}
