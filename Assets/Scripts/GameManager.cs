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
using System.Text;
using Firebase.Storage;
using System.Threading.Tasks;

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
    private string path;
    public List<string> dragTracer;
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

    void Start()
    {
        fruitsLearned = new List<String>();
        fruitsUnlearned = new List<String>();
        itemsLearned = new List<String>();
        itemsUnlearned = new List<String>();
        dragTracer = new List<string>();


        path = "Assets/Resources/write.csv";
        //string fileName = "write.csv";
        //string path = Path.Combine(Application.persistentDataPath, fileName);
        string header = "Date;Scene;Learning_modality;Click_target;Successful_action;Target_object\n";

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        StorageReference storageReference = storage.GetReferenceFromUrl("gs://integratedproject2-eladda.appspot.com");

        StorageReference csv_ref = storageReference.Child("csv/write.csv");
        csv_ref.GetDownloadUrlAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // File does not exist, so create it and add header row
                File.WriteAllText(path, header);
                Debug.Log("File created and header added.");
            }
            else
            {
                // File already exists, so do nothing
                Debug.Log("File already exists.");
            }

            // Upload the file to Firebase Storage
            byte[] csv_bytes = Encoding.ASCII.GetBytes(File.ReadAllText(path));
            csv_ref.PutBytesAsync(csv_bytes).ContinueWith(uploadTask => {
                if (uploadTask.IsFaulted || uploadTask.IsCanceled)
                {
                    Debug.Log(uploadTask.Exception.ToString());
                }
                else
                {
                    Debug.Log("Finished uploading CSV file...");
                }
            });
        });

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
