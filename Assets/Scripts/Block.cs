using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 프리팹에 직접 붙이는 스크립트
/// </summary>
public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Elements")]
    public SpriteListSO blockSprites;

    public Node.Type type;
    public Point point;
    public Vector2 targetPos;
    public Vector2 targetOffset = Vector2.zero;

    RectTransform rect;
    RectTransform shakeTrans;
    RectTransform childRect;

    public bool onMove = false;
    private bool onOffsetMove = false;

    public bool isAlive = false;
    Image img;

    public Vector2 lastMoveDir;

    private void Awake()
    {
        shakeTrans = transform.GetChild(0).GetComponent<RectTransform>();

        childRect = shakeTrans.GetChild(0).GetComponent<RectTransform>();
        img = childRect.GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void InitSetup(Node.Type type, Point p)
    {
        if (type == Node.Type.Blank || type == Node.Type.Hole)
        {
            Debug.Log("빈칸");
            gameObject.SetActive(false);
            return;
        }

        isAlive = true;
        this.type = type;
        img.sprite = blockSprites[(int)type - 1];

        SetPoint(p);
    }

    public void SetPoint(Point p)
    {
        point = p;
        ResetPosition();
        transform.name = $"Block ({point.x}, {point.y})";
        gameObject.SetActive(true);
    }

    public void ResetPosition()
    {
        MovePositionTo(point.GetAnchorPosition());
        MoveOffsetTo(Vector2.zero);
    }

    public void SetQuickPosition(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }

    public void MovePositionTo(Vector2 move)
    {
        targetPos = move;
        onMove = true;

    }

    public void MoveOffsetTo(Vector2 move)
    {
        targetOffset = move;
        onOffsetMove = true;
    }

    private void Update()
    {
        if(onMove)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * 20f);

            if (Vector2.Distance(rect.anchoredPosition, targetPos) > 1f)
            {
                lastMoveDir = (targetPos - rect.anchoredPosition).normalized * St.HALF_CELL * 0.15f;
            }
            else
            {
                rect.anchoredPosition = targetPos;
                onMove = false;

                shakeTrans.DOPunchAnchorPos(lastMoveDir, 0.15f, 1, 0.7f);
            }
        }

        if(onOffsetMove)
        {
            childRect.anchoredPosition = Vector2.Lerp(childRect.anchoredPosition, targetOffset, Time.deltaTime * 12f);
            if (Vector2.Distance(childRect.anchoredPosition, targetOffset) < 2f)
            {
                childRect.anchoredPosition = targetOffset;
                onOffsetMove = false;
            }
        }
    }

    #region 조작계
    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Inst.PlaySound("press");

        if (onMove) return;
        transform.SetAsLastSibling();
        BlockMoveCtrl.Inst.MoveBlock(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onMove) return;
        BlockMoveCtrl.Inst.DropPiece();
    }
    #endregion

    public void Kill()
    {
        isAlive = false;
        gameObject.SetActive(false);
    }

    public void Revive()
    {
        isAlive = true;
        gameObject.SetActive(true);
    }

    public bool IsCanKilledBlock()
    {
        return Node.IsColorBlock(type);
    }

    private void OnDisable()
    {
        shakeTrans.DOKill();
        shakeTrans.anchoredPosition = Vector2.zero; 
        childRect.anchoredPosition = Vector2.zero;
    }
}
