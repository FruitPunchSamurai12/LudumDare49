using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;

    public MeshRenderer rendererForHighlight;


    public void Interact()
    {
        onInteract?.Invoke();
    }

    public void Highlight(bool highlight) {
        if(rendererForHighlight != null) {
            rendererForHighlight.material.SetFloat("Highlight", highlight?1:0);
        }
    }
}
