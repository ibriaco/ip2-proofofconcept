using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class DragObject : MonoBehaviour, IDragHandler
{
    public AudioSource backpackAudioSource;

   // private static string parent_path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    //private string logs_path = Path.Combine(parent_path, "Resources");

    public void OnDrag(PointerEventData eventData)
    {
        if (!backpackAudioSource.isPlaying && GetComponent<Image>().color.a >= 1)
        {
            /*var scene = SceneManager.GetActiveScene().name.Contains("Fruits") ? "Fruits" : "School";
            var learning_mode = SceneManager.GetActiveScene().name.Contains("Passive") ? "Passive" : "Active";
            learning_mode = SceneManager.GetActiveScene().name.Contains("Testing") ? "Testing" : learning_mode;
            var lines = System.IO.File.ReadAllLines("Assets/Resources/interaction_log.csv");
            var last_line = lines[lines.Length - 1];
            if (!(last_line.Contains(scene) && last_line.Contains(gameObject.name) && last_line.Contains("Picked")))
            {
                StreamWriter csv_writer = new StreamWriter("Assets/Resources/interaction_log.csv", true);
                csv_writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + ";" + scene + ";" + learning_mode + ";" + gameObject.name + ";" + "Dragged;Ciao");
                csv_writer.Close();
            }*/
            
            transform.position = Input.mousePosition;
        }
    }
}
