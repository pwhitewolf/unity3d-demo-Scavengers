using UnityEngine;
using System.Collections;

public class _SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;

    public static _SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;
    
    // Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
	}
	
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {

    }

	// Update is called once per frame
	void Update () {
	
	}
}
