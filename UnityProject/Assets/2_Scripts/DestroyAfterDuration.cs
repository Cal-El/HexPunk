using UnityEngine;
using System.Collections;

public class DestroyAfterDuration : MonoBehaviour {

    public float duration = 0.1f;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration) Destroy(gameObject);
    }
}
