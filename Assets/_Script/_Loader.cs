using UnityEngine;
using System.Collections;

public class _Loader : MonoBehaviour {

    public GameObject gameManager;

	// Use this for initialization
	void Awake () {
        if (_GameManager.instance == null)
            Instantiate(gameManager);
	}
	
	
}
