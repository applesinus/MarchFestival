using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public void OpenDialogue() => gameObject.SetActive(true);
    public void CloseDialogue() => gameObject.SetActive(false);
}
