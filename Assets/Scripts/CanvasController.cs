using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public Canvas canvas;
    public AudioSource backgroundAudio;
    private string nextSceneName = "FruitsLearning";
    public GameObject alarmChild;
    private bool canProceed = false;
    private GameManager gameManager;
    private int currentChildIndex = 0;
    private int maxChildIndex;
    private GameObject[] children;

    private float timeUntilNextChild = 5f;
    private float timePassed = 0f;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        children = new GameObject[canvas.transform.childCount];
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            children[i] = canvas.transform.GetChild(i).gameObject;
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
        if (children[currentChildIndex] == alarmChild)
        {
            if (canProceed)
            {
                children[currentChildIndex].SetActive(false);
                currentChildIndex++;
                if (currentChildIndex > maxChildIndex)
                {
                    SceneManager.LoadScene(nextSceneName);
                    return;
                }
                children[currentChildIndex].SetActive(true);
                canProceed = false;
            }
        }
        else
        {
            children[currentChildIndex].SetActive(false);
            currentChildIndex++;
            if (currentChildIndex > maxChildIndex)
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath, "interaction_log.csv")))
                {
                    StreamWriter csv_writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "interaction_log.csv"), true);
                    csv_writer.WriteLine("Date;Scene;Learning_modality;Click_target;Action;Target_object");
                    csv_writer.Flush();
                    csv_writer.Close();
                }

                if (gameManager.sceneName)
                    SceneManager.LoadScene("FruitsLearning");
                else
                    SceneManager.LoadScene("SchoolLearning");
                return;
            }
            children[currentChildIndex].SetActive(true);
        }
    }

    public void AllowProceed()
    {
        canProceed = true;
    }
}
