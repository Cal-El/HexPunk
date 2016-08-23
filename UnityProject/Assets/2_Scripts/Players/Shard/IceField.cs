using UnityEngine;
using System.Collections;

public class IceField : MonoBehaviour {
    [HideInInspector]
    public float range =1;
    public float duration = 0.1f;
    public AnimationCurve growth;
    private float timer;
    

    void Start()
    {
        timer = 0;
        range *= 2;
        transform.localScale = new Vector3(range, 0, range);
    }

    // Update is called once per frame
    void Update ()
    {
        timer += Time.deltaTime;
        transform.localScale = new Vector3(range, growth.Evaluate(timer), range);
        if (timer > duration)
        { 
            Destroy(gameObject);

        }
	}
}
