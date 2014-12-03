using UnityEngine;
using System.Collections;

public class LoadLevelFromXML : MonoBehaviour {

	public TextAsset level;
    public bool loadLevel;
    LevelEditor levelEditor;
    Camera cam;

    void Awake()
    {
        levelEditor = GetComponent<LevelEditor>();
        this.cam = FindObjectOfType<Camera>();
    }


    void Start()
    {
        if (loadLevel)
        {
            levelEditor.LoadLevelFromTextAsset(level);
			cam.orthographicSize = 10; //TODO it's a temp hack until loading camera position from the level editor works
			cam.gameObject.AddComponent<DeadlyCamera>();
        }
    }
}
