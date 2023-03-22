using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    private string nextSceneName = "SchoolTesting";
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            StartCoroutine(WaitAndTransition());
        }
        
    }

    IEnumerator WaitAndTransition()
    { 
        yield return new WaitForSeconds(2);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("audio/twinkle");
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
