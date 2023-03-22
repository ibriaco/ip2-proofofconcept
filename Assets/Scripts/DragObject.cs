using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler
{
    public AudioSource backpackAudioSource;

    public void OnDrag(PointerEventData eventData)
    {
        if (!backpackAudioSource.isPlaying && GetComponent<Image>().color.a >= 1)
        {
            transform.position = Input.mousePosition;
        }
    }
}
