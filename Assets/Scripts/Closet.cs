using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using UnityEngine.UI;
using Firebase.Storage;
using System.Text;

public class Closet : MonoBehaviour
{
    public Image[] inventorySlots;
    public bool[] isSlotEmpty;
    private bool isAudioPlaying = false;
    private GameManager gameManager;
    private int attempts = 0;
    private GameObject [] items;
    private GameObject[] fruits;
    private string audioPath;
    private string repeatAudio;
    private AudioSource _source;
    private GameObject wrongObject;
    public Dictionary<String, Vector3> initial_positions = new Dictionary<String, Vector3>();
    private float timeUntilNextChild = 5f;
    private float timePassed = 0f;
    private int counter = 0;
    StorageReference csv_ref;
    StorageReference storageReference;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://integratedproject2-eladda.appspot.com");
        if (SceneManager.GetActiveScene().name.Contains("School"))
        {
            items = GameObject.FindGameObjectsWithTag("Item");
            foreach (var i in items)
            {
                initial_positions.Add(i.name, i.transform.position);
            }
            Shuffle(gameManager.itemsLearned);
        }
        else
        {
            fruits = GameObject.FindGameObjectsWithTag("Fruit");
            foreach (var f in fruits)
            {
                initial_positions.Add(f.name, f.transform.position);
            }
            Shuffle(gameManager.fruitsLearned);
        }

        _source = gameObject.AddComponent<AudioSource>();

        isSlotEmpty = new bool[inventorySlots.Length];
        for (int i = 0; i < isSlotEmpty.Length; i++)
        {
            isSlotEmpty[i] = true;
        }

        PlayInstruction();
    }

    

    private void PlayInstruction()
    {
        if (SceneManager.GetActiveScene().name.Contains("School"))
            audioPath = "audio/" + "Drag_the_" + gameManager.itemsLearned[0].ToLower() + "_to_the_locker";
        else
            audioPath = "audio/" + "put_the_" + gameManager.fruitsLearned[0].ToLower() + "_on_the_plate";

        _source.PlayOneShot((AudioClip)Resources.Load(audioPath));
        isAudioPlaying = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        wrongObject = other.gameObject;
        if (SceneManager.GetActiveScene().name.Contains("School"))
        {
            if (other.name.Equals(gameManager.itemsLearned[0]))
            {
                attempts = 0;
                counter++;
                Debug.Log(counter);
                int emptySlotIndex = -1;
                for (int i = 0; i < isSlotEmpty.Length; i++)
                {
                    if (isSlotEmpty[i])
                    {
                        emptySlotIndex = i;
                        break;
                    }
                }
                if (emptySlotIndex != -1)
                {
                    writeLog(other.name, "Correct_target", other.name);
                    isSlotEmpty[emptySlotIndex] = false;
                    StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                    isAudioPlaying = true;
                }
            }
            else if (!other.name.Equals(gameManager.itemsLearned[0]) && attempts == 0)
            {
                writeLog(other.name, "Wrong_target", gameManager.itemsLearned[0]);
                attempts++;
                repeatAudio = "audio/" + "that_is_not_" + gameManager.itemsLearned[0].ToLower();
                _source.PlayOneShot((AudioClip)Resources.Load(repeatAudio));
                isAudioPlaying = false;
                Invoke("ResetPosition", 2.0f);
            }
            else if (!other.name.Equals(gameManager.itemsLearned[0]) && attempts > 0)
            {
                writeLog(other.name, "Wrong_target", gameManager.itemsLearned[0]);
                attempts++;
                repeatAudio = "audio/" + "that_is_not_" + gameManager.itemsLearned[0].ToLower();
                _source.PlayOneShot((AudioClip)Resources.Load(repeatAudio));
                isAudioPlaying = false;
                Invoke("ResetPosition", 2.0f);
            }
        }
        else
        {
            if (other.name.Equals(gameManager.fruitsLearned[0]))
            {
                counter++;
                Debug.Log(counter);
                int emptySlotIndex = -1;
                for (int i = 0; i < isSlotEmpty.Length; i++)
                {
                    if (isSlotEmpty[i])
                    {
                        emptySlotIndex = i;
                        break;
                    }
                }
                if (emptySlotIndex != -1)
                {
                    writeLog(other.name, "Correct_target", other.name);
                    isSlotEmpty[emptySlotIndex] = false;
                    StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                    isAudioPlaying = true;
                }
            }
            else if (!other.name.Equals(gameManager.fruitsLearned[0]) && attempts == 0)
            {
                writeLog(other.name, "Wrong_target", gameManager.fruitsLearned[0]);
                attempts++;
                repeatAudio = "audio/" + "that_is_not_" + gameManager.fruitsLearned[0].ToLower();
                _source.PlayOneShot((AudioClip)Resources.Load(repeatAudio));
                isAudioPlaying = false;
                Invoke("ResetPosition", 2.0f);
            }
            else if (!other.name.Equals(gameManager.fruitsLearned[0]) && attempts > 0)
            {
                writeLog(other.name, "Wrong_target", gameManager.fruitsLearned[0]);
                attempts++;
                repeatAudio = "audio/" + "that_is_not_" + gameManager.fruitsLearned[0].ToLower();
                _source.PlayOneShot((AudioClip)Resources.Load(repeatAudio));
                isAudioPlaying = false;
                Invoke("ResetPosition", 2.0f);
            }
        }
    }

    private void ResetPosition()
    {
        wrongObject.transform.position = initial_positions[wrongObject.name];
    }


    IEnumerator WaitAndReplace(Collider2D other, int emptySlotIndex)
    {
        
        yield return new WaitForSeconds(2);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("audio/twinkle");
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        inventorySlots[emptySlotIndex].enabled = true;
        inventorySlots[emptySlotIndex].preserveAspect = true;
        inventorySlots[emptySlotIndex].sprite = other.GetComponent<Image>().sprite;
        Destroy(other.gameObject);
        isAudioPlaying = false;
        if (SceneManager.GetActiveScene().name.Contains("School"))
            gameManager.itemsLearned.RemoveAt(0);

        else
            gameManager.fruitsLearned.RemoveAt(0);

        if (counter == 4 && SceneManager.GetActiveScene().name.Contains("School"))
        {
            SceneManager.LoadScene("TransitionToLunch");
        }
        else if (counter == 4 && SceneManager.GetActiveScene().name.Contains("Fruits"))
        {
            Application.Quit();
        } 
        else
            PlayInstruction();
    }

    private void writeLog(string target_name, string successfull_action, string target_of_act)
    {
        csv_ref = storageReference.Child("csv/write.csv");

        var scene = SceneManager.GetActiveScene().name.Contains("Fruits") ? "Fruits" : "School";
        gameManager.dragTracer.Add(DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + ";" + scene + ";" + "Testing" + ";" +
                             target_name + ";" + successfull_action + ";" + target_of_act + "\n");
        var learning_mode = SceneManager.GetActiveScene().name.Contains("Passive") ? "Passive" : "Active";
        csv_ref.GetBytesAsync(10000).ContinueWith(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to get file contents: " + task.Exception);
            }
            else
            {
                byte[] existingBytes = task.Result;
                string newContent = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + ";" + scene + ";" + "Testing" + ";" +
                             target_name + ";" + successfull_action + ";" + target_of_act + "\n";
                

                byte[] newBytes = Encoding.ASCII.GetBytes(newContent);
                byte[] updatedBytes = new byte[existingBytes.Length + newBytes.Length];
                Array.Copy(existingBytes, 0, updatedBytes, 0, existingBytes.Length);
                Array.Copy(newBytes, 0, updatedBytes, existingBytes.Length, newBytes.Length);

                // Upload the updated byte array to the file
                csv_ref.PutBytesAsync(updatedBytes).ContinueWith(uploadTask => {
                    if (uploadTask.IsFaulted || uploadTask.IsCanceled)
                    {
                        Debug.Log("Failed to update file: " + uploadTask.Exception);
                    }
                    else
                    {
                        Debug.Log("File updated successfully");
                    }
                });
            }
        });
        
    }


    private void Shuffle(List<String> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
