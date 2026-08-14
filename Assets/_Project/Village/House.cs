using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class House :  MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public UnityEvent<string> onClick;
    [SerializeField] public UnityEvent<string, int> onClickWithTime;
    [SerializeField] public Clock clock;
    [SerializeField] public string houseOccupant;
    [SerializeField] public Material material;

    private GameObject nameText;
    private bool isPointerOnSprite;
    private Coroutine scaleCoroutine;
    private Vector3 baseScale = Vector3.one;
    private bool isScaleCached = false;
    private UnityEngine.UI.Image img;

    private void Start()
    {
        gameObject.GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.1f;
        nameText = transform.Find("Name").gameObject;
        nameText.SetActive(false);
    }

    public void OnEnable()
    {
        transform.localScale = baseScale;
        if (nameText != null) nameText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.TryGetComponent(out img))
        {
            img.material = material;
        }
        else
        {
            Debug.LogWarning("Image not found on house!");
        }

        nameText.SetActive(true);
        isPointerOnSprite = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.TryGetComponent<UnityEngine.UI.Image>(out var img)) img.material = null;

        nameText.SetActive(false);
        isPointerOnSprite = false;
    }

    private void scale(Vector2 targetScale)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(scaleAnimation(targetScale, 0.1f));
    }

    private IEnumerator scaleAnimation(Vector2 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(targetScale.x, targetScale.y, startScale.z);
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isScaleCached)
        {
            baseScale = transform.localScale;
            isScaleCached = true;
        }
        
        scale(new Vector2(baseScale.x * 1.1f, baseScale.y * 1.1f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerOnSprite)
        {
            transform.localScale = baseScale;
            if (img.material != null) img.material = null;

            onClick.Invoke(houseOccupant);
            onClickWithTime.Invoke(houseOccupant, clock.timeProgress);
            isPointerOnSprite = false;
        }
        else
        {
            scale(new Vector2(baseScale.x, baseScale.y));
        }
    }
}
