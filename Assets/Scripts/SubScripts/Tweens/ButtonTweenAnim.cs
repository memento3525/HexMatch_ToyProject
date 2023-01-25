using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTweenAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public float shirinkScale = 0.9f;
    public bool soundOff = false;

    private const float toShirinkTime = 0.2f;
    private const float toNormalTime = 0.1f;
    private Vector3 shirinkVector;

    private readonly Ease openEasy = Ease.OutBack;
    private readonly Ease closeEasy = Ease.InBack;

    private bool isSetup = false;
    private Selectable selectable;

    private Vector3 startScale = Vector3.one;
    private Tweener tweener;

    private void Awake()
    {
        InitialSetup();
    }

    private void InitialSetup()
    {
        if (isSetup) return;
        isSetup = true;

        shirinkVector = startScale * shirinkScale;
        selectable = GetComponent<Selectable>();
    }

    private bool IsSelectable()
    {
        if (selectable == null)
            return true;
        else
            return selectable.interactable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsSelectable() == false)
            return;

        CallKill();
        ToShrink();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsSelectable() == false)
            return;

        CallKill();
        ToNormal();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelectable() == false)
            return;

        //if (soundOff == false)
         //   SoundManager.Inst.PlayButtonSound();
    }

    public void ToShrink()
    {
        tweener = transform.DOScale(shirinkVector, toShirinkTime)
            .SetEase(openEasy)
            .SetUpdate(true);
    }

    public void ToNormal()
    {
        tweener = transform.DOScale(startScale, toNormalTime)
           .SetEase(closeEasy)
           .SetUpdate(true);
    }

    public void CallKill()
    {
        if (tweener != null && tweener.IsActive())
            tweener.Kill();
    }

    private void OnEnable()
    {
        InitialSetup();

        CallKill();
        transform.localScale = startScale;
    }

    private void OnDisable()
    {
        CallKill();
    }
}
