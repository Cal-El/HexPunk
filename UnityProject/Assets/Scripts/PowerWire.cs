using UnityEngine;
using System.Collections;

public class PowerWire : MonoBehaviour {

    private PressurePad p;
    private Renderer r;
    private Material origMat;
    public Material poweredMat;
	
    void Start() {
        p = transform.parent.GetComponent<PressurePad>();
        r = GetComponent<Renderer>();
        origMat = r.material;
    }

	// Update is called once per frame
	void Update () {
        poweredMat.mainTextureOffset = new Vector2((poweredMat.mainTextureOffset.x + Time.deltaTime / 20) % 1, (poweredMat.mainTextureOffset.y + Time.deltaTime / 20) % 1);
        if (p != null) {
            if (p.Powered && p.Activated) {
                r.material = poweredMat;
            } else {
                r.material = origMat;
            }
        } else {
            r.material.color = Color.red;
        }
	}
}
