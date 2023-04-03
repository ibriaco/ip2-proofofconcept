using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class CanvasController : MonoBehaviour
{
    public Canvas canvas;
    public AudioSource backgroundAudio;
    private string nextSceneName = "FruitsLearning";
    public GameObject alarmChild;
    public bool canProceed = false;
    private GameManager gameManager;
    private int currentChildIndex = 0;
    private int maxChildIndex;
    private GameObject[] children;
    private float timeUntilNextChild = 5f;
    private float timePassed = 0f;
    AudioSource audioSource;


    void Start()
    {
        //string filePath = Path.Combine(Application.persistentDataPath, "interaction_log.csv");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        children = new GameObject[canvas.transform.childCount];

        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            children[i] = canvas.transform.GetChild(i).gameObject;
            if (children[i].name == "Alarm")
            {
                Button alarmButton = children[i].GetComponentInChildren<Button>();
                alarmButton.onClick.AddListener(() => AllowProceed());
            }
        }
        maxChildIndex = children.Length - 1;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= timeUntilNextChild)
        {
            ChangeChild();
            timePassed = 0f;
        }
    }

    void ChangeChild()
    {
        if (children[currentChildIndex].name == "Alarm")
        {
            if (canProceed)
            {
                //Debug.Log("if can proceed");
                children[currentChildIndex].SetActive(false);
                currentChildIndex++;
                if (currentChildIndex > maxChildIndex)
                {
                    SceneManager.LoadScene(nextSceneName);
                    return;
                }
                children[currentChildIndex].SetActive(true);
            }
        }
        else
        {
            children[currentChildIndex].SetActive(false);
            currentChildIndex++;
            if (currentChildIndex > maxChildIndex && canProceed == true)
            {
                //Debug.Log("prima del if-else");
                if (gameManager.sceneName)
                    SceneManager.LoadScene("FruitsLearning");
                else
                    SceneManager.LoadScene("SchoolLearning");
                return;
            }
            children[currentChildIndex].SetActive(true);
        }
    }


    IEnumerator PlayAudio()
    {
        audioSource.clip = Resources.Load<AudioClip>("audio/teddygettingready");
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    public void AllowProceed()
    {
        StartCoroutine(PlayAudio());
        canProceed = true;
    }
}
