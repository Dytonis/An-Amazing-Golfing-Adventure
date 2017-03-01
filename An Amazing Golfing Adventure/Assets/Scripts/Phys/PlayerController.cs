using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject PowerIndicator;
    public GameObject XIndicator;
    public CameraOrbit Orbiter;
    public float PowerLimit;

    public float Power;

    void Update()
    {
        if(IsStationary())
        {
            if(Input.GetMouseButton(0))
            {
                PowerIndicator.SetActive(true);
                Power += Input.GetAxisRaw("Mouse Y");
                if(Power <= 0)
                {
                    Power = 0;
                }
                if(Power >= PowerLimit)
                {
                    Power = PowerLimit;
                }

                Orbiter.EnabledY = false;
            }
            else
            {
                Orbiter.EnabledY = true;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(Power >= 0 && IsStationary())
            {
                GetComponent<Rigidbody>().AddForce(XIndicator.transform.forward * Power * 250);
                Power = 0;
                PowerIndicator.SetActive(false);
            }
        }

        PowerIndicator.transform.localScale = new Vector3(0.5f, 0.1f, Power / 2);
        PowerIndicator.transform.rotation = XIndicator.transform.rotation;
        PowerIndicator.transform.position = transform.position;
    }

    bool IsStationary()
    {
        return (GetComponent<Rigidbody>().velocity.magnitude == 0);
    }
}
