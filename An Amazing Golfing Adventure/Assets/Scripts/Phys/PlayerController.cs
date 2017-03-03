using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject PowerIndicator;
    public GameObject XIndicator;
    public CameraOrbit Orbiter;
    public float PowerLimit;
    public float SpeedSoundLimit;

    public float Power;

    public bool Playable;
    public float PlayableSeconds;

    public Vector3 LastPositionRTS;

    public AudioSource RollingCarpet;
    public AudioSource BounceCarpet;
    public AudioSource BounceBrick;
    public AudioSource BounceConcrete;

    public Game game;

    void Update()
    {
        RaycastHit info;
        if (IsGrounded(out info))
        {
            if (info.collider.gameObject.tag == "Carpet")
            {
                if (IsStationary())
                    Playable = true;
                else
                    Playable = false;

                if (!RollingCarpet.isPlaying)
                    RollingCarpet.Play();
                RollingCarpet.volume = Mathf.Clamp01(GetComponent<Rigidbody>().velocity.magnitude / SpeedSoundLimit);
            }
            else
            {
                RollingCarpet.Stop();
                Playable = false;
            }
        }
        else
        {
            RollingCarpet.Stop();
            Playable = false;
        }

        if(Playable)
        {
            PlayableSeconds += Time.deltaTime;
            if (PlayableSeconds >= 0.2)
            {
                LastPositionRTS = transform.localPosition;
                GetComponent<MeshRenderer>().materials[0].SetColor("_OutlineColor", Color.grey);

                if (Input.GetMouseButton(0))
                {
                    PowerIndicator.SetActive(true);
                    Power += Input.GetAxisRaw("Mouse Y");
                    if (Power <= 0)
                    {
                        Power = 0;
                    }
                    if (Power >= PowerLimit)
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
        }
        else
        {
            PlayableSeconds = 0;
            GetComponent<MeshRenderer>().materials[0].SetColor("_OutlineColor", Color.black);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(Power >= 0 && IsStationary())
            {
                GetComponent<Rigidbody>().AddForce(XIndicator.transform.forward * Mathf.Pow(3.6f * Power, 0.88f) * 10);
                Power = 0;
                PowerIndicator.SetActive(false);
            }
        }

        PowerIndicator.transform.localScale = new Vector3(0.05f, 0.05f, Power / 40);
        PowerIndicator.transform.rotation = XIndicator.transform.rotation;
        PowerIndicator.transform.position = transform.position;
    }

    void OnTriggerEnter(Collider col)
    {
        print(col.gameObject.tag);
        if (col.gameObject.tag == "Hole")
        {
            ResetVelocityAndPosition();
            game.NextHole();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.gameObject.tag == "Brick")
        {
            BounceBrick.volume = Mathf.Clamp01(GetComponent<Rigidbody>().velocity.magnitude / SpeedSoundLimit);
            BounceBrick.Play();
        }
        if (col.collider.gameObject.tag == "Concrete")
        {
            BounceConcrete.volume = Mathf.Clamp01(GetComponent<Rigidbody>().velocity.magnitude / SpeedSoundLimit);
            BounceConcrete.Play();
        }
        if (col.collider.gameObject.tag == "OoB")
        {
            if(GetComponent<Rigidbody>().velocity.magnitude < 2)
            {
                ResetVelocityAndPosition();
                game.ResetPosition(gameObject, LastPositionRTS);
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.gameObject.tag == "OoB")
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < 2)
            {
                ResetVelocityAndPosition();
                game.ResetPosition(gameObject, LastPositionRTS);
            }
        }
    }

    void ResetVelocityAndPosition()
    {
        transform.localPosition = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    bool IsStationary()
    {
        return (GetComponent<Rigidbody>().velocity.magnitude == 0);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    bool IsGrounded(out RaycastHit hit)
    {
        return Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 0.1f);
    }
}
