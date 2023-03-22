using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using UnityEngine.UI;

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
    private AudioSource _source;
    private GameObject wrongObject;
    public Dictionary<String, Vector3> initial_positions = new Dictionary<String, Vector3>();
    private int counter = 0;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            counter++;
            if (other.name.Equals(gameManager.itemsLearned[0]))
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
                    isSlotEmpty[emptySlotIndex] = false;
                    StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                    isAudioPlaying = true;
                }
            }
            else
            {
                Invoke("ResetPosition", 2.0f);
            }
        }
        else
        {
            counter++;
            if (other.name.Equals(gameManager.fruitsLearned[0]))
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
                    isSlotEmpty[emptySlotIndex] = false;
                    StartCoroutine(WaitAndReplace(other, emptySlotIndex));
                    isAudioPlaying = true;
                }
            }
            else
            {
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
            SceneManager.LoadScene("FruitsTesting");
        }
        else
            Application.Quit();
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