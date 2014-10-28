using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    [System.Serializable]
    public class Clip

    {
        public string name;
        public List<AudioClip> clips;
        public AudioClip RandomClip()
        {
            return clips[Random.Range(0,clips.Count)];
        }
    }

    public List<Clip> clipList;
    Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void PlayClip(string clipName)
    {
        bool playedClip = false;
        foreach (Clip c in clipList)
        {
            if (c.name.Equals(clipName))
            {
                AudioSource.PlayClipAtPoint(c.RandomClip(), player.transform.position);
                playedClip = true;
            }
        }
        if (playedClip == false)
        {
            Debug.LogError("couldn't find audio clip: " + clipName);
        }
    }
}
