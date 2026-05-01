using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    public void Block() => gameObject.SetActive(true);
    public void Unblock() => gameObject.SetActive(false);
}
