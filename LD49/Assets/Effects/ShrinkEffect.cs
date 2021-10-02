using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinkable : MonoBehaviour
{
    // Collider should sit in a separate subobject we can easily scale
    public GameObject colliderSubObject;

    // Actual object
    public GameObject graphics;

    // Copy of the object with the ghost shader on it!
    public GameObject expandGraphics;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
