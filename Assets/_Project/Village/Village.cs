using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    private void Start()
    {
        CloseVillage();
    }
    
    public void OpenVillage() => gameObject.SetActive(true);
    public void CloseVillage() => gameObject.SetActive(false);
}
