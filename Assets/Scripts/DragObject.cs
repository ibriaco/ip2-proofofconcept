using System;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AudioSource backpackAudioSource;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!backpackAudioSource.isPlaying && GetComponent<Image>().color.a >= 1 && !SceneManager.GetActiveScene().name.Contains("Transition"))
        {
            Debug.Log("Drag started");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!backpackAudioSource.isPlaying && GetComponent<Image>().color.a >= 1)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        WriteLog();
        Debug.Log("Drag ended");
    }

    private async void WriteLog()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageReference = storage.GetReferenceFromUrl("gs://integratedproject2-eladda.appspot.com");
        StorageReference csv_ref = storageReference.Child("csv/write.csv");
        var scene = SceneManager.GetActiveScene().name.Contains("Fruits") ? "Fruits" : "School";
        var learning_mode = SceneManager.GetActiveScene().name.Contains("Passive") ? "Passive" : "Active";
        learning_mode = SceneManager.GetActiveScene().name.Contains("Testing") ? "Testing" : learning_mode;

        string newContent = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + ";" + scene + ";" + learning_mode + ";" + gameObject.name + ";" + "Dragged;Null\n";

        await Task.Run(() =>
        {
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            StorageReference storageReference = storage.GetReferenceFromUrl("gs://integratedproject2-eladda.appspot.com");
            StorageReference csv_ref = storageReference.Child("csv/write.csv");

            csv_ref.GetBytesAsync(10000).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("Failed to get file contents: " + task.Exception);
                    return;
                }

                byte[] existingBytes = task.Result;
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
                        Debug.Log("File updated successfully logging dragged");
                    }
                });
            });
        });
    }
}