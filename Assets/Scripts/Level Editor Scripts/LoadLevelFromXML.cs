using UnityEngine;
using System.Collections;

public class LoadLevelFromXML : MonoBehaviour {

	public TextAsset level;
    public bool loadLevel;
    LevelEditor levelEditor;

    void Awake()
    {
        levelEditor = GetComponent<LevelEditor>();
        Camera cam = FindObjectOfType<Camera>();
        cam.orthographicSize = 10; //TODO it's a temp hack until loading camera position from the level editor works
        cam.gameObject.AddComponent<DeadlyCamera>();
    }


    void Start()
    {
        if (loadLevel)
        {
            levelEditor.LoadLevelFromTextAsset(level);
        }
    }
}
