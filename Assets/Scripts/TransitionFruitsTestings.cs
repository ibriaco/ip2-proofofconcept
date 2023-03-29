using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionFruitsTestings : MonoBehaviour
{
    IEnumerator Start()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        // Load the next scene
        SceneManager.LoadScene("FruitsTesting");
    }
}
