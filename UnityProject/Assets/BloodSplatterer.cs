using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodSplatterer : MonoBehaviour {

    public static BloodSplatterer bs;
    
    public struct bloodSplat {
        public GameObject blood;
        public SpriteRenderer bSprite;
        public float startTime;
    }

    private List<bloodSplat> bloodSplats;
    public GameObject[] bloodDecals;


	// Use this for initialization
	void Start () {
        bs = this;
        bloodSplats = new List<bloodSplat>();

    }
	
	// Update is called once per frame
	void Update () {
        List<bloodSplat> removeList = new List<bloodSplat>();
	    foreach(bloodSplat b in bloodSplats) {
            b.bSprite.color = new Color(1, 1, 1, 1 - ((Time.time - b.startTime) / 5));
            if (b.bSprite.color.a <= 0) {
                Destroy(b.blood);
                removeList.Add(b);
            }
        }
        foreach (bloodSplat b in removeList) {
            bloodSplats.Remove(b);
        }
    }

    public static void MakeBlood(Vector3 position) {
        position.y = 0.1f;
        if(bs.bloodDecals != null && bs.bloodDecals.Length > 0) {
            bloodSplat b;
            int i = Random.Range(0, bs.bloodDecals.Length - 1);
            b.blood = Instantiate(bs.bloodDecals[i], position, bs.bloodDecals[i].transform.rotation) as GameObject;
            b.blood.transform.Rotate(0, Random.Range(0, 360), 0, Space.World);
            b.bSprite = b.blood.GetComponent<SpriteRenderer>();
            b.startTime = Time.time;
            bs.bloodSplats.Add(b);
        }
    }
}
