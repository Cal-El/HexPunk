using UnityEngine;
using System.Collections;

public class MaelstromScript : MonoBehaviour
{

    [SerializeField]
    private Collider area;
    [SerializeField]
    private float DoT = 5;
    [SerializeField]
    private float duration = 3;
    [SerializeField]
    private float spinSpeed = -360;

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject != null) transform.Rotate(0, Time.deltaTime * spinSpeed, 0);

        foreach (Character g in Megamanager.GetAllCharacters())
        {
            if (g != null)
            {
                if (g.transform != null)
                {
                    if (g.transform != transform.parent)
                    {
                        if (area.bounds.Contains(g.transform.position))
                        {
                            g.TakeDmg(DoT * Time.deltaTime, Character.DamageType.Standard, transform.parent.GetComponent<PlayerStats>());
                        }
                    }
                }
            }
        }
    }
}
