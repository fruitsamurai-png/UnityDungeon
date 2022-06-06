using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPointer : MonoBehaviour
{
    public Transform targetposition;
    public float HideDistance=100.0f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        GameObject pcg = GameObject.Find("PCGSpawner");
        bool activated = pcg.GetComponent<PCG>().portalenabled;
        if(activated)
        {
            targetposition = GameObject.Find("Portal").GetComponent<Transform>();
            var dir = targetposition.position - transform.position;
            GameObject hero = GameObject.Find("Hero");
            transform.position = hero.GetComponent<Transform>().position + new Vector3(2, 0);
            if (dir.magnitude < HideDistance)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}
