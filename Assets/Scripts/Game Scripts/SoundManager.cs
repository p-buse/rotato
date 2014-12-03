using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	GameManager gameManager;

	void Awake()
	{
		this.gameManager = GetComponent<GameManager>();
	}

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


    public void PlayClip(string clipName, float volume)
    {
        bool playedClip = false;
        foreach (Clip c in clipList)
        {
            if (c.name.Equals(clipName))
            {
                AudioSource.PlayClipAtPoint(c.RandomClip(), gameManager.player.transform.position, volume);
                playedClip = true;
            }
        }
        if (playedClip == false)
        {
            Debug.LogError("couldn't find audio clip: " + clipName);
        }
    }
}
