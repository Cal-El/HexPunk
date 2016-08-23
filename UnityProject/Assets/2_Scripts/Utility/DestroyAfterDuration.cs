using UnityEngine;
using System.Collections;

public class DestroyAfterDuration : MonoBehaviour {

    public ParticleSystem p;
    public float duration = 0.1f;
    private float timer;
     
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration) {
            if(p != null)
            {
                p.enableEmission = false;
                p.loop = false;
                transform.parent = null;
                Destroy(this.gameObject, 5);
            }
            else
            {
                Destroy(gameObject);
            }
        } 
    }
}
