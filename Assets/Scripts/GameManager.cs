using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI 레퍼런스")]
    public RectTransform backBoard;
    public RectTransform gameBoard;
    public RectTransform killedBoard;

    [Header("스크립터블 오브젝트")]
    [SerializeField] GameSetupSO gameSetupSO;

    [Header("프리팹")]
    [SerializeField] Slot slotPrefab;
    [SerializeField] PoolObject blockPrefab;

    private int width;
    private int height;
    private Node[,] board;
    private Point generationPoint = new(5, 5); // 생산 좌표

    private Factory blockFactory;

    private readonly List<BlockFlipCtrl> flippedBlocks = new();
    private readonly List<Block> onMovingBlocks = new();
    private readonly List<Block> finishedUpdating = new();
    private readonly List<Block> onSlideBlocks = new(); // 이번에 움직인 블록들

    private float waitTime = 0f;
    private const float maxWaitTime = 0.1f; // 이동이 끝난뒤 체크를 대기하는 시간

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        width = gameSetupSO.width;
        height = gameSetupSO.height;
        board = new Node[width, height];
        generationPoint = new Point(gameSetupSO.generationPoint.x, gameSetupSO.generationPoint.y);

        blockFactory = new Factory(blockPrefab, gameBoard, 0);

        var blockMoveCtrl = gameObject.AddComponent<BlockMoveCtrl>();
        var canvas = gameBoard.GetComponentInParent<Canvas>();
        blockMoveCtrl.InitialSetup(canvas);

        ResetData();
        SpawnSlots();
    }

    public void ResetGame()
    {
        foreach (Node node in board)
        {
            Block block = node.GetBlock();
            if (block != null)
                block.Kill();
        }

        ResetData();
    }

    private void ResetData()
    {
        onMovingBlocks.Clear();
        flippedBlocks.Clear();
        finishedUpdating.Clear();
        onSlideBlocks.Clear();

        InitializeBoard();
        VerifyBoard();
        FirstSpawnBoard();
    }

    void Update()
    {
        bool remainFlipped = FlippedBlockCheck();
        if (remainFlipped) return;

        SpawnFromSpawnPoint(generationPoint);
        ApplyGravityToBoard();

        if (onSlideBlocks.Count > 0)
            SlideBlock();
        else if (HasAnyBlank())
        {
            waitTime = 0f;
            FindBlankAndTopMarkDynamic();
        }
        else
        {
            waitTime += Time.deltaTime;
            if (waitTime < maxWaitTime) return;

            waitTime = 0f;

            CheckIsAnyConnectedBlock();
        }

    }

    /// <summary>
    /// 연결된 블록이 있는지 체크
    /// </summary>
    private void CheckIsAnyConnectedBlock()
    {
        finishedUpdating.Clear();
        for (int i = 0; i < onMovingBlocks.Count; i++)
        {
            Block block = onMovingBlocks[i];
            if (block != null && !block.onMove)
                finishedUpdating.Add(block);
        }
        for (int i = 0; i < onSlideBlocks.Count; i++)
        {
            Block block = onSlideBlocks[i];
            if (block != null && !block.onMove && !finishedUpdating.Contains(block))
                finishedUpdating.Add(block);
        }
        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            Block block = finishedUpdating[i];
            List<Point> connected = IsConnected(block.point, true);
            if (connected.Count > 0)
                RemoveConnectedBlocks(connected);

            if (onMovingBlocks.Contains(block))
                onMovingBlocks.Remove(block);
        }
    }

    /// <summary>
    /// 플립한것 매칭되었는지 미리 체크
    /// </summary>
    private bool FlippedBlockCheck()
    {
        // 이번에 플립한거 매치가 안된경우 되돌아감
        List<Block> checkBlock = new List<Block>();
        for (int i = 0; i < flippedBlocks.Count; i++)
        {
            checkBlock.Add(flippedBlocks[i].one);
            checkBlock.Add(flippedBlocks[i].two);
        }
        for (int i = 0; i < checkBlock.Count; i++)
        {
            Block block = checkBlock[i];
            BlockFlipCtrl flip = GetFlipped(block); // Flip 리스트에 포함되어있다면 null이 아닌 값을 반환함
            if (flip == null)
                continue;

            List<Point> connected = IsConnected(block.point, true);
            Block flippedblock = flip.GetOtherBlock(block);
            AddPoints(ref connected, IsConnected(flippedblock.point, true));

            if (connected.Count == 0) // 매치 안되었을때
            {
                FlipBlocks(block.point, flippedblock.point, false); // 되돌아감
                SoundManager.Inst.PlaySound("missMatch");
            }

            flippedBlocks.Remove(flip); // 업데이트 이후 플립 제거함
        }

        return flippedBlocks.Count > 0;
    }

    /// <summary>
    /// 연결된 블럭들을 전부 삭제함
    /// </summary>
    private void RemoveConnectedBlocks(List<Point> connected)
    {
        SoundManager.Inst.PlaySound("match");
        foreach (Point pnt in connected) // 연결된 노드 조각 제거
        {
            Node.Type type = GetTypeAtPoint(pnt);
            if (Node.IsColorBlock(type))
            {
                Vector2 effPos = pnt.GetAnchorPosition();
                EffectManager.Inst.SpawnKilledBlock(effPos, type);
            }

            Node node = GetNodeAtPoint(pnt);
            Block nodeBlock = node.GetBlock();
            if (nodeBlock != null)
            {
                nodeBlock.Kill();

                // 새로추가.
                if (onMovingBlocks.Contains(nodeBlock))
                    onMovingBlocks.Remove(nodeBlock);

                if (onSlideBlocks.Contains(nodeBlock))
                    onSlideBlocks.Remove(nodeBlock);
            }

            node.SetBlock(null);
        }
    }

    #region 블럭의 움직임 제어
    void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                Node node = GetNodeAtPoint(p);
                Node.Type type = GetTypeAtPoint(p);

                if (type != Node.Type.Blank) continue; // 빈 슬롯인 경우에만 처리함

                // 바로 윗칸을 확인해서, 컬러블럭일경우 현재위치의 블럭과 해당 블럭을 바꾼다.
                Point next = Point.Add(p, Point.Up);

                // 컬러블럭이면
                Node.Type nextType = GetTypeAtPoint(next);
                if (Node.IsColorBlock(nextType))
                {
                    Node got = GetNodeAtPoint(next);
                    Block block = got.GetBlock();

                    if (block.onMove || block.isAlive == false) continue; // 아직 움직이는 중인 블럭은 다시 움직일수없다.

                    node.SetBlock(block);
                    onMovingBlocks.Add(block);

                    got.SetBlock(null); // Blank로 설정
                }
            }
        }
    }

    private void SlideBlock()
    {
        int slideCount = 0;
        for (int i = onSlideBlocks.Count - 1; i >= 0; i--)
        {
            Block block = onSlideBlocks[i];
            if (block != null && block.isAlive)
            {
                if (block.onMove) continue; // 아직 움직이는 중인 블럭은 다시 움직일수없다.

                bool wasSlide = CheckAndSlide(block);
                if (wasSlide)
                    slideCount++;
            }
        }

        if (slideCount == 0)
            onSlideBlocks.Clear();
    }

    private bool CheckAndSlide(Block block)
    {
        Point curPoint = block.point;

        // 위쪽에 제거 가능한 블럭이 있다면 미끄러지지않는다.
        Point upCheck = Point.Add(curPoint, Point.Up);
        Node.Type upType = GetTypeAtPoint(upCheck);
        if (Node.IsColorBlock(upType))
            return false;

        Point[] directions = // 위쪽부터 시계방향으로 돌아간다.
        {
            Point.LeftDown,
            Point.RightDown, // 추후 랜덤으로 바꿔야함.
        };

        Node curNode = GetNodeAtPoint(curPoint);
        foreach (Point dir in directions)
        {
            Point check = Point.Add(curPoint, dir);
            if (GetTypeAtPoint(check) == Node.Type.Blank)
            {
                Node checkNode = GetNodeAtPoint(check);
                checkNode.SetBlock(block);
                onMovingBlocks.Add(block);

                curNode.SetBlock(null);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 구멍이 나올때까지 위쪽으로 탐색하다가 구멍일경우, 양옆 블록이 슬라이드 될 수 있게 마킹한다.
    /// </summary>
    private void FindBlankAndTopMarkDynamic()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y].type == Node.Type.Blank)
                {
                    int upScale = 0;
                    while (true)
                    {
                        upScale++;
                        Point check = new Point(x + upScale, y + upScale);
                        if (GetTypeAtPoint(check) == Node.Type.Hole)
                            break;
                    }

                    Point onTopPoint = new Point(x + upScale - 1, y + upScale - 1);
                    Point[] directions =
                    {
                        Point.LeftUp, Point.RightUp,
                    };

                    foreach (Point dir in directions)
                    {
                        Point check = Point.Add(onTopPoint, dir);
                        if (Node.IsColorBlock(GetTypeAtPoint(check)))
                        {
                            Node node = GetNodeAtPoint(check);
                            onSlideBlocks.Add(node.GetBlock());
                        }
                    }
                }
            }
        }
    }
    #endregion

    void SpawnFromSpawnPoint(Point spawnPoint)
    {
        if (GetTypeAtPoint(spawnPoint) != Node.Type.Blank)
            return;

        Point fallPnt = new Point(spawnPoint.x + 1, spawnPoint.y + 1);

        Vector2 spawnPos = fallPnt.GetAnchorPosition();
        var newObj = blockFactory.Spawn(spawnPos);
        Block block = newObj.GetComponent<Block>();

        block.Revive();

        block.InitSetup(Node.GetRandomColorType(), spawnPoint);
        block.SetQuickPosition(spawnPos);

        Node hole = GetNodeAtPoint(spawnPoint);
        hole.SetBlock(block);
        ResetBlockPosition(block);

        onSlideBlocks.Add(block); // 슬라이딩을 위해 추가됨
    }

    BlockFlipCtrl GetFlipped(Block block)
    {
        for (int i = 0; i < flippedBlocks.Count; i++)
        {
            if (flippedBlocks[i].IsContained(block))
                return flippedBlocks[i];
        }
        return null;
    }

    void InitializeBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isLayoutHole = gameSetupSO.arrayLayout.rows[x].row[y];
                Node.Type type = isLayoutHole ? Node.Type.Hole : Node.GetRandomColorType();
                if (board[x, y] == null)
                    board[x, y] = new Node(type, new Point(x, y));
                else
                    board[x, y].SetType(type);
            }
        }
    }

    bool HasAnyBlank()
    {
        foreach (var item in board)
        {
            if (item.type == Node.Type.Blank)
                return true;
        }
        return false;
    }

    void VerifyBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                Node.Type type = GetTypeAtPoint(p);
                if ((int)type <= 0) continue;

                List<Node.Type> bannedType = new List<Node.Type>();

                while (IsConnected(p, true).Count > 0)
                {
                    type = GetTypeAtPoint(p);
                    if (!bannedType.Contains(type))
                        bannedType.Add(type);
                    SetTypeAtPoint(p, NewValue(ref bannedType));
                }
            }
        }
    }

    public void ResetBlockPosition(Block block)
    {
        if (block == null) return;

        block.ResetPosition();
        onMovingBlocks.Add(block);
    }

    public void FlipBlocks(Point one, Point two, bool main)
    {
        if (Node.IsColorBlock(GetTypeAtPoint(one)) == false) return;

        Node nodeOne = GetNodeAtPoint(one);
        Block blockOne = nodeOne.GetBlock();

        if (Node.IsColorBlock(GetTypeAtPoint(two)))
        {
            Node nodeTwo = GetNodeAtPoint(two);
            Block blockTwo = nodeTwo.GetBlock();

            nodeOne.SetBlock(blockTwo);
            nodeTwo.SetBlock(blockOne);

            if (main)
                flippedBlocks.Add(new BlockFlipCtrl(blockOne, blockTwo));

            onMovingBlocks.Add(blockOne);
            onMovingBlocks.Add(blockTwo);
        }
        else
            ResetBlockPosition(blockOne);
    }

    void FirstSpawnBoard()
    {
        foreach (Node node in board)
        {
            if (Node.IsColorBlock(node.type) == false) continue;

            Vector2 newPos = node.point.GetAnchorPosition();
            var obj = blockFactory.Spawn(newPos);
            Block block = obj.GetComponent<Block>();

            block.InitSetup(node.type, node.point);
            block.SetQuickPosition(newPos);

            node.SetBlock(block);
        }
    }

    void SpawnSlots()
    {
        foreach (Node node in board)
        {
            Node.Type type = node.type;
            if (Node.IsColorBlock(type) == false) continue;

            Vector2 newPos = node.point.GetAnchorPosition();
            Slot slot = Instantiate(slotPrefab, backBoard);
            slot.InitSetup(node, newPos);
        }
    }

    List<Point> IsConnected(Point curPoint, bool main)
    {
        List<Point> connected = new List<Point>();

        Node.Type curType = GetTypeAtPoint(curPoint);
        Point[] directions = // 위쪽부터 시계방향으로 돌아간다.
        {
            Point.Up,   Point.RightUp,  Point.RightDown,
            Point.Down, Point.LeftDown, Point.LeftUp,
        };

        #region 직선 라인 체크
        // 현재위치의 블럭을 기준으로 특정방향으로 같은 색의 블럭이 2연속으로 있는지 체크한다.
        // [현재꺼] [같은색] [같은색]
        foreach (Point dir in directions)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++) // 현재꺼 제외하고 한칸씩 이후꺼를 체크하기위해.
            {
                Point check = Point.Add(curPoint, Point.Mult(dir, i));
                if (GetTypeAtPoint(check) == curType)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1) // 자신과 연속 2개가 같은 색이라면 매치성공!
                AddPoints(ref connected, line);
        }
        #endregion

        #region 양옆 라인 체크
        // 자신을 중심으로 양옆으로 블럭이 배열되어 색 매치가 있는경우를 체크한다.
        // [같은색] [현재꺼] [같은색]
        for (int i = 0; i < 3; i++) // 방향의 절반 값
        {
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] check =
            {
                Point.Add(curPoint, directions[i]),
                Point.Add(curPoint, directions[i + 3]) // 반대방향을 구하기위해 +3
            };
            foreach (Point next in check) // 양 옆의 색을 확인후, 같은 색이라면 리스트에 넣음.
            {
                if (GetTypeAtPoint(next) == curType)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1) // 자신과 양옆이 같은 색이라면 매치성공!
                AddPoints(ref connected, line);
        }
        #endregion

        #region 2x2 체크
        // 자신을 중심으로 특정방향으로 사각형
        // RU-LU / RD-LD / RU-RD / LU-LD 를 각 체크해야함.
        Point[] square_directions = // 위쪽부터 시계방향으로 돌아간다.
        {
            Point.RightUp, Point.RightDown, Point.LeftDown, Point.LeftUp,
        };

        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();
            int first = i;

            for (int k = 0; k < 2; k++)
            {
                int second = (i + 1 + k * 2) % square_directions.Length; // next + 3next

                int same = 0;
                Point[] check =
                {
                    Point.Add(curPoint, square_directions[first]),
                    Point.Add(curPoint, square_directions[second]),
                    Point.Add(curPoint, Point.Add(square_directions[first], square_directions[second]))
                };

                foreach (var pnt in check)
                {
                    if (GetTypeAtPoint(pnt) == curType)
                    {
                        square.Add(pnt);
                        same++;
                    }
                }

                if (same > 2) // 자신과 사각형으로 3개가 같은 색이라면 매치 성공!
                    AddPoints(ref connected, square);
            }
        }
        #endregion

        if (main) // 다른 매치가 현재 매치에 동시발생했는지
        {
            for (int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, IsConnected(connected[i], false));
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd)
                points.Add(p);
        }
    }

    /// <summary>
    /// 해당포인트의 타입을 리턴함. (범위를 넘어가면 구멍으로 판정함!)
    /// </summary>
    Node.Type GetTypeAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return Node.Type.Hole;
        return board[p.x, p.y].type;
    }

    void SetTypeAtPoint(Point p, Node.Type t)
    {
        board[p.x, p.y].type = t;
    }

    Node GetNodeAtPoint(Point p) => board[p.x, p.y];

    // remove와 겹치지않는 색상 한 개 랜덤으로 리턴.
    Node.Type NewValue(ref List<Node.Type> remove)
    {
        List<Node.Type> available = new List<Node.Type>();
        for (int i = 0; i < Node.colorLength; i++)
            available.Add((Node.Type)(i + 1));
        foreach (var i in remove)
            available.Remove(i);

        if (available.Count <= 0)
            return Node.Type.Blank;

        return available[Random.Range(0, available.Count)];
    }
}