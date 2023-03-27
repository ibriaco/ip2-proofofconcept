using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using UnityEngine.UI;
using Firebase.Storage;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<String> fruitsLearned;
    public List<String> fruitsUnlearned;
    public List<String> itemsLearned;
    public List<String> itemsUnlearned;
    private static readonly Random random = new Random();
    public bool sceneName = random.Next(0, 2) == 0; //fruits or school
    public bool sceneType = random.Next(0, 2) == 0; //active or passive
    public int skipper = 0;
    // get the parent path of this file
    //private static string parent_path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    // create logs path which is ../Resources/
    //private string logs_path = Path.Combine(parent_path, "Resources");

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fruitsLearned = new List<String>(); //[1,2,3]
        fruitsUnlearned = new List<String>();
        itemsLearned = new List<String>();
        itemsUnlearned = new List<String>();

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSkipper()
    {
        skipper++;
    }
}
