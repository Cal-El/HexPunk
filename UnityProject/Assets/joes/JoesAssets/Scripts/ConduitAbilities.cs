using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ConduitAbilities : NetworkBehaviour {

    private bool isAxisInUse1 = false;
    private bool isAxisInUse2 = false;
    private bool isAxisDown1 = false;
    private bool isAxisDown2 = false;

    // Use this for initialization
    void Start () {
	    
	}

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButtonDown("Ability 1")) Ability1();
        if (GetAxisDown1("Ability 2")) Ability2();
        else if (Input.GetButtonDown("Ability 3")) Ability3();
        else if (GetAxisDown2("Ability 4")) Ability4();

    }

    private void Ability1()
    {
        GetComponent<MeshRenderer>().material.color = Color.black;
        Debug.Log("Ability 1");
    }

    private void Ability2()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        Debug.Log("Ability 2");
    }

    private void Ability3()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        Debug.Log("Ability 3");
    }

    private void Ability4()
    {
        GetComponent<MeshRenderer>().material.color = Color.yellow;
        Debug.Log("Ability 4");
    }

    private bool GetAxisDown1(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse1 == false)
            {
                isAxisDown1 = true;
                isAxisInUse1 = true;
            } else
            {
                isAxisDown1 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse1 = false;
            isAxisDown1 = false;
        }
        return isAxisDown1;
    }

    private bool GetAxisDown2(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse2 == false)
            {
                isAxisDown2 = true;
                isAxisInUse2 = true;
            }
            else
            {
                isAxisDown2 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse2 = false;
            isAxisDown2 = false;
        }
        return isAxisDown2;
    }
}
