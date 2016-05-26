using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DestructibleObject : NetworkBehaviour
{

    public GameObject particleEffect;
    private bool _isDestroyed = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDmg(float dmg)
    {
        if (particleEffect != null)
        {
            GameObject.Instantiate(particleEffect, this.transform.position, this.transform.rotation);
        }
        Destruct();
    }

    private void Destruct()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;

        NetworkServer.Destroy(gameObject);
    }
}
