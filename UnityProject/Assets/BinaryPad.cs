using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BinaryPad : Pad {

    [SyncVar] private int input = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject onMe = SomethingOnMe(0);
        if (onMe != null)   {
            if (onMe.tag == "Player") {
                if (onMe.GetComponent<PlayerMovement>().isLocalPlayer)
                    if (Input.GetKeyDown(KeyCode.E))
                        input = 1;
                    else if (Input.GetKeyDown(KeyCode.Q))
                        input = -1;
            }
        } else {
            input = 0;
        }
    }

    public int Inp
    {
        get
        {
            return input;
        }
    }

    public override void Reset()
    {
        base.Reset();
        input = 0;
    }
}
