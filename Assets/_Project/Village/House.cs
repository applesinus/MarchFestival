using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class House : MonoBehaviour
{
    [SerializeField] public UnityEvent<string> onClick;
    [SerializeField] public string houseOccupant;

    private void OnMouseUpAsButton() => onClick.Invoke(houseOccupant);
}
