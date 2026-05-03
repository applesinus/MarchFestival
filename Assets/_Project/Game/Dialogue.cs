using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private void Start()
    {
        OpenDialogue();
    }
    
    public void OpenDialogue() => gameObject.SetActive(true);
    public void CloseDialogue() => gameObject.SetActive(false);
}
