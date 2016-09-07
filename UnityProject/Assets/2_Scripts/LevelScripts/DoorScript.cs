using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DoorScript : NetworkBehaviour {

    [SyncVar]
    private bool open = false;
    public float timeToOpen = 2.0f;

    [System.Serializable] public struct DOOR {
        public Transform obj;
        public Vector3 openedPos;
        public Vector3 closedPos;
    }

    public DOOR[] doors;
    public Renderer[] doorFrames;

    [SerializeField] private Material lockedMat;
    [SerializeField] private Material unlockedMat;
    private float openess = 0.0f;
	

    void Start()
    {
        if (open)
        {
            UnlockDoor();
        }
        else
        {
            LockDoor();
        }
    }

	// Update is called once per frame
	void Update () {
        if (open)
            openess = Mathf.Clamp(openess + Time.deltaTime / timeToOpen, 0, 1);
        else
            openess = Mathf.Clamp(openess - Time.deltaTime / timeToOpen, 0, 1);

        for (int i = 0; i < doors.Length; i++)
            doors[i].obj.localPosition = Vector3.Lerp(doors[i].closedPos, doors[i].openedPos, openess);
	}

    public void UnlockDoor()
    {
        if (unlockedMat != false)
        {
            foreach (Renderer t in doorFrames)
            {
                t.material = unlockedMat;
            }
        }
        open = true;
    }

    public void LockDoor()
    {
        if (lockedMat != false)
        {
            foreach (Renderer t in doorFrames)
            {
                t.material = lockedMat;
            }
        }
        open = false;

    }
}
