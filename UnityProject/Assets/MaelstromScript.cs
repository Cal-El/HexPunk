using UnityEngine;
using System.Collections;

public class MaelstromScript : MonoBehaviour {
    
    [SerializeField]
    private Collider area;
    [SerializeField]
    private float DoT = 5;
    [SerializeField]
    private float duration = 3;
    [SerializeField]
    private float spinSpeed = -360;

    // Use this for initialization
    void Start () {
        Destroy(this.gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, Time.deltaTime * spinSpeed, 0);

        foreach (GameObject g in Megamanager.GetAllCharacters()) {
            if (g.transform != transform.parent && area.bounds.Contains(g.transform.position)) {
                g.SendMessage("TakeDmg", DoT * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
            }
        }
	}
}
