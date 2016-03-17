using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public GameObject myPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Find Target Location
        Vector3 targetPos = new Vector3(
            myPlayer.transform.position.x - 7.25f,
            transform.position.y,
            myPlayer.transform.position.z - 7.25f
            );

        transform.position = targetPos;
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5);
	}
}
