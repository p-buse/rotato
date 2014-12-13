using UnityEngine;
using System.Collections;

public class MusicTheme : MonoBehaviour {

    public enum LevelTheme { farm, city, lab, space, escape, none };
    public LevelTheme theme;
    public bool restartOnLoad;
}
