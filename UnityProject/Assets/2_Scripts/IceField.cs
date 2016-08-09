using UnityEngine;
using System.Collections;

public class IceField : MonoBehaviour {
    public float duration = 0.1f;
    private float timer;

    // Update is called once per frame
    void Update ()
    {
        timer += Time.deltaTime;
        if (timer < duration / 20) transform.localScale += Vector3.up * Time.deltaTime * 50;
        if (timer > duration)Destroy(gameObject);
	}
}
