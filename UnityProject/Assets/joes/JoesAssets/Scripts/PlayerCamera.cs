using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public GameObject myPlayer;
    private Camera camera;
    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        startPos = this.gameObject.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        //Find Target Location
        Vector3 targetPos = new Vector3(
            myPlayer.transform.position.x + startPos.x,
            transform.position.y,
            myPlayer.transform.position.z + startPos.z
            );

        transform.position = targetPos;
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5);
	}
}
