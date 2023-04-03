using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using Firebase.Storage;
using System.Text;
using System.Threading.Tasks;

public class Backpack : MonoBehaviour
{
    public Image[] inventorySlots;
    public bool[] isSlotEmpty;
    private bool isAudioPlaying = false;
    public int maxFruitsInPassiveMode = 4;
    private List<GameObject> fruits;
    private List<int> pickedFruitsIndex;
    private List<GameObject> items;
    private List<int> pickedItemsIndex;
    private GameManager gameManager;
    private int counter = 0;
    private static readonly System.Random random = new System.Random();
    public bool sceneType = random.Next(0, 2) == 0;
    StorageReference csv_ref;
    StorageReference storageReference;

    void Start()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://integratedproject2-eladda.appspot.com");
        sceneType = random.Next(0, 2) == 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        fruits = new List<GameObject>();
        pickedFruitsIndex = new List<int>();
        items = new List<GameObject>();
        pickedItemsIndex = new List<int>();
        isSlotEmpty = new bool[inventorySlots.Length];
        for (int i = 0; i < isSlotEmpty.Length; i++)
        {
            isSlotEmpty[i] = true;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Fruit"))
            { 
                fruits.Add(obj);
                gameManager.fruitsUnlearned.Add(obj.name);
            }
            else if(obj.CompareTag("Item"))
            {
                items.Add(obj);
                gameManager.itemsUnlearned.Add(obj.name);
            }
        }

        if (!sceneType && items.Count == 0)
        {
            for (int i = 0; i < maxFruitsInPassiveMode; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, fruits.Count);
                while (pickedFruitsIndex.Contains(randomIndex))
                {
                    randomIndex = UnityEngine.Random.Range(0, fruits.Count);
                }
                pickedFruitsIndex.Add(randomIndex);
            }
            for (int i = 0; i < fruits.Count; i++)
            {
                if (!pickedFruitsIndex.Contains(i))
                {
                    Color tempColor = fruits[i].GetComponent<Image>().color;
                    fruits[i].GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 0.25f);
                    fruits[i].GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }
        else if(!sceneType && fruits.Count == 0)
        {
            for (int i = 0; i < maxFruitsInPassiveMode; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, items.Count);
                while (pickedItemsIndex.Contains(randomIndex))
                {
                    randomIndex = UnityEngine.Random.Range(0, items.Count);
                }
                pickedItemsIndex.Add(randomIndex);
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (!pickedItemsIndex.Contains(i))
                {
                    Color tempColor = items[i].GetComponent<Image>().color;
                    items[i].GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 0.25f);
                    items[i].GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        counter++;
        if (other.CompareTag("Fruit"))
        {
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
                writeLog(other.gameObject.name, "Picked");
                gameManager.fruitsLearned.Add(other.gameObject.name);
                isSlotEmpty[emptySlotIndex] = false;
                StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                isAudioPlaying = true;
            }
        }
        else if (other.CompareTag("Item"))
        {
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
                writeLog(other.gameObject.name, "Picked");
                gameManager.itemsLearned.Add(other.gameObject.name);
                isSlotEmpty[emptySlotIndex] = false;
                StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                isAudioPlaying = true;
            }
        }
    }

    IEnumerator PlayAudio(string pathforaudio)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(pathforaudio);
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        if(SceneManager.GetActiveScene().name.Contains("School"))
            SceneManager.LoadScene("FruitsLearning");
        else
            SceneManager.LoadScene("SchoolLearning");
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

        string fruitName = other.gameObject.name.ToLower();

        audioSource.clip = Resources.Load<AudioClip>("audio/" + fruitName);

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        inventorySlots[emptySlotIndex].sprite = other.GetComponent<Image>().sprite;
        Destroy(other.gameObject);
        isAudioPlaying = false;

        if(counter == 4 && SceneManager.GetActiveScene().name.Contains("School") && gameManager.skipper == 0)
        {
            gameManager.updateSkipper();
            ListAdjustment(gameManager.itemsUnlearned, gameManager.itemsLearned);
            StartCoroutine(PlayAudio("audio/good_school"));
            
        }
        else if(counter == 4 && SceneManager.GetActiveScene().name.Contains("Fruit") && gameManager.skipper == 0)
        {
            gameManager.updateSkipper();
            ListAdjustment(gameManager.fruitsUnlearned, gameManager.fruitsLearned);
            StartCoroutine(PlayAudio("audio/good_fruits"));
            
        }
        else if (counter == 4 && gameManager.skipper == 1)
        {
            gameManager.updateSkipper();
            ListAdjustment(gameManager.fruitsUnlearned, gameManager.fruitsLearned);
            SceneManager.LoadScene("TransitionToSchool");
        }
    }

    private void writeLog(string target_name, string successfull_action)
    {
        csv_ref = storageReference.Child("csv/write.csv");
        
        var scene = SceneManager.GetActiveScene().name.Contains("Fruits") ? "Fruits" : "School";
        var learning_mode = SceneManager.GetActiveScene().name.Contains("Passive") ? "Passive" : "Active";
        
            csv_ref.GetBytesAsync(10000).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to get file contents: " + task.Exception);
            }
            else
            {
                byte[] existingBytes = task.Result;
                string newContent = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + ";" + scene + ";" + learning_mode + ";" +
                target_name + ";" + successfull_action + ";None\n";
                byte[] newBytes = Encoding.ASCII.GetBytes(newContent);
                byte[] updatedBytes = new byte[existingBytes.Length + newBytes.Length];
                Array.Copy(existingBytes, 0, updatedBytes, 0, existingBytes.Length);
                Array.Copy(newBytes, 0, updatedBytes, existingBytes.Length, newBytes.Length);

                csv_ref.PutBytesAsync(updatedBytes).ContinueWith(uploadTask =>
                {
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


    public bool IsAudioPlaying()
    {
        return isAudioPlaying;
    }

    private void ListAdjustment(List<String> orgList, List<String> toRemove)
    {
        foreach (var x in toRemove)
        {
            orgList.Remove(x);
        }
    }
}
