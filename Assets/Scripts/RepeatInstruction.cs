using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RepeatInstruction : MonoBehaviour
{
    private string audioPath;
    private AudioSource _source;
    private GameManager gameManager;
    private bool isAudioPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        _source = gameObject.AddComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Button muteButton = gameObject.GetComponent<Button>();
        muteButton.onClick.AddListener(() => OnClick());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (SceneManager.GetActiveScene().name.Contains("School"))
            audioPath = "audio/" + "Drag_the_" + gameManager.itemsLearned[0].ToLower() + "_to_the_locker";
        else
            audioPath = "audio/" + "put_the_" + gameManager.fruitsLearned[0].ToLower() + "_on_the_plate";

        _source.PlayOneShot((AudioClip)Resources.Load(audioPath));
        isAudioPlaying = false;
    }
}
