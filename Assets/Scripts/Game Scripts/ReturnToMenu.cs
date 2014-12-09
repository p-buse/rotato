using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameData))]
public class ReturnToMenu : MonoBehaviour {
    GameData gameData;

    void Start()
    {
        gameData = GetComponent<GameData>();
        gameData.ChangeUnlockedLevel(Application.loadedLevel + 1);
        Application.LoadLevel(0);
    }
}
