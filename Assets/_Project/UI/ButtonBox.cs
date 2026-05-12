using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonBox : MonoBehaviour
{
    [Header("Output Events")]
    [SerializeField] public UnityEvent OnClick;

    [Header("Animation")]
    [SerializeField] public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] public float duration = 0.1f;
    [SerializeField] public Transform button;

    [Header("Sound")]
    [SerializeField] public AudioClip buttonDown;
    [SerializeField] public AudioClip buttonUp;

    private Vector3 baseScale;
    private Vector3 affectedScale;
    
    private float timer = 1f;
    private Vector3 startScale;
    private Vector3 endScale;

    private void Start()
    {
        baseScale = button.localScale;
        affectedScale = baseScale * 0.8f;
    }


    public void OnMouseDown()
    {
        Animation(baseScale, affectedScale);
    }

    public void OnMouseUp()
    {
        Animation(affectedScale, baseScale);
    }

    public void OnMouseUpAsButton()
    {
        OnClick.Invoke();
    }


    private void Animation(Vector3 start, Vector3 end)
    {
        startScale = start;
        endScale = end;
        timer = 0f;
    }

    private void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime / duration;
            button.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(timer));
        }
    }

    public void ChangeText(string text) => GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
}
