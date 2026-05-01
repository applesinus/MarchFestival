using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MovableScreen : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] public UnityEvent onAnimationStart;
    [SerializeField] public UnityEvent onAnimationEnd;
    [SerializeField] public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] public float duration = 1f;

    private float timer = 1f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool isAnimating = false;

    public void ScrollUp() => Animation(transform.localPosition, transform.localPosition - Vector3.up*720f);
    public void ScrollDown() => Animation(transform.localPosition, transform.localPosition + Vector3.up*720f);

    private void Animation(Vector3 start, Vector3 end)
    {
        startPosition = start;
        endPosition = end;
        timer = 0f;

        onAnimationStart.Invoke();
        isAnimating = true;
    }

    private void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime / duration;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(timer));
        }
        else if (isAnimating)
        {
            onAnimationEnd.Invoke();
            isAnimating = false;
        }
    }
}
