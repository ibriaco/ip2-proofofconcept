using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BikeScript : MonoBehaviour
{
    public Vector3 pointA; 
    public Vector3 pointB;
    public Vector3 pointC;
    public Vector3 pointD;
    public Vector3 pointE;
    public float speed = 0.45f;
    private GameObject kindergarten;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool hasMoved = false;
    private int counter = 0;

    void Start()
    {
        StartCoroutine(PlayAudio());
    }

    private void Awake()
    {
        pointA = GameObject.FindWithTag("Item").transform.position;
        pointB = GameObject.Find("PointB").transform.position;
        pointC = GameObject.Find("PointC").transform.position;
        pointD = GameObject.Find("PointD").transform.position;
        pointE = GameObject.Find("PointE").transform.position;
        kindergarten = GameObject.Find("Kindergarten");
    }


    IEnumerator PlayAudio()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        Invoke("CheckForMovement", 5f);
    }


    void CheckForMovement()
    {
        if (!hasMoved)
        {
            kindergarten.GetComponent<BoxCollider2D>().enabled = false;
            kindergarten.GetComponent<BoxCollider2D>().isTrigger = false;
            StartCoroutine(MoveFromAtoB());
        }
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            hasMoved = true;
        }
    }

    IEnumerator MoveFromAtoB()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(pointA, pointB, t);
            t += speed * Time.deltaTime;
            yield return null;
        }
        transform.position = pointB;
        StartCoroutine(MoveFromBtoC());
    }

    IEnumerator MoveFromBtoC()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(pointB, pointC, t);
            t += speed * Time.deltaTime;
            yield return null;
        }
        transform.position = pointC;
        StartCoroutine(MoveFromCtoD());
    }
    IEnumerator MoveFromCtoD()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(pointC, pointD, t);
            t += speed * Time.deltaTime;
            yield return null;
        }
        transform.position = pointD;
        StartCoroutine(MoveFromDtoE());
    }

    IEnumerator MoveFromDtoE()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(pointD, pointE, t);
            t += speed * Time.deltaTime;
            yield return null;
        }
        transform.position = pointE;
        yield return new WaitForSeconds(5f);
        if (transform.position == pointE)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            transform.position = pointA;
            kindergarten.GetComponent<BoxCollider2D>().enabled = true;
            kindergarten.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
