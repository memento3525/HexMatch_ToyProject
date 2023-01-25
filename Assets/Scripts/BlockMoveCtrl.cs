using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ������ ���� ������ �����̴� �븮��.
/// </summary>
public class BlockMoveCtrl : MonoBehaviour
{
    public static BlockMoveCtrl Inst;
    GameManager gameManager;

    Block movingBlock;
    Point newPoint;
    Vector2 dragStart;

    Canvas canvas;
    float dragThreshold;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void InitialSetup(Canvas canvas)
    {
        Inst = this;
        this.canvas = canvas;
        dragThreshold = St.CELL * canvas.scaleFactor;
    }

    /// <summary>
    /// ��Ŀ�� �Ǿ����� �������� �������� �� �ִ�.
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            dragThreshold = St.CELL * canvas.scaleFactor;
        }
        else
        {
            if (movingBlock != null)
                movingBlock.ResetPosition();

            movingBlock = null;
            newPoint = null;
        }
    }

    void Update()
    {
        if (movingBlock == null) 
            return;

        Vector2 dir = (Vector2)Input.mousePosition - dragStart;
        Vector2 nDir = dir.normalized;

        newPoint = Point.Clone(movingBlock.point);
        Point add = Point.Zero;
        if(dir.magnitude > dragThreshold) // ���� ������
            add = AngleToDirPoint(nDir);

        newPoint.Add(add);

        Vector2 offset = Vector2.zero;

        if (!newPoint.Equals(movingBlock.point))
            offset = add.GetAnchorPosition() * 0.5f;

        movingBlock.MoveOffsetTo(offset);
    }

    private Point AngleToDirPoint(Vector2 dirVector)
    {
        float angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg + 180f;
        if (angle > 300f) return Point.LeftUp;
        else if (angle > 240f) return Point.Up;
        else if (angle > 180f) return Point.RightUp;
        else if (angle > 120f) return Point.RightDown;
        else if (angle > 60f) return Point.Down;
        else return Point.LeftDown;
    }

    public void MoveBlock(Block piece)
    {
        if (movingBlock != null) return;

        movingBlock = piece;
        dragStart = Input.mousePosition;
    }

    public void DropPiece()
    {
        if (newPoint == null || movingBlock == null) return;

        if (!newPoint.Equals(movingBlock.point))
        {
            SoundManager.Inst.PlaySound("slide");
            gameManager.FlipBlocks(movingBlock.point, newPoint, true);
        }
        else
            gameManager.ResetBlockPosition(movingBlock);

        movingBlock = null;
        newPoint = null;
    }
}
