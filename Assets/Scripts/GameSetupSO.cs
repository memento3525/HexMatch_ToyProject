using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class GameSetupSO : SerializedScriptableObject
{
    public int width = 6; // ��ǥ�� ĭ��
    public int height = 6; // ��ǥ�� ĭ��
    public Vector2Int generationPoint = new Vector2Int(5, 5); // ���� ��ǥ

    [HideInInspector]
    public ArrayLayout arrayLayout; // ���� ����� �Ѿ�� ������
    public bool[,] boardMatrix = new bool[6, 6]; // ������ ������ ������ // x���ο�(��) y���ο�(��)

    /// <summary>
    /// ����Ƽ���� �������迭�� Serialize�� �ȵǼ� boardMatrix => arrayLayout �� ��ȯ
    /// </summary>
    private void OnValidate()
    {
        SetMatrixSize();
    }

    public void SetMatrixSize()
    {
        bool[,] temp = (bool[,])boardMatrix.Clone();
        boardMatrix = new bool[width, height];

        int width_size = Mathf.Min(width, temp.GetLength(0)); // ���ο�
        int height_size = Mathf.Min(height, temp.GetLength(1)); // ������

        for (int y = 0; y < height_size ; y++)
        {
            for (int x = 0; x < width_size; x++)
                boardMatrix[x, y] = temp[x, y];
        }

        arrayLayout = new ArrayLayout(width, height, boardMatrix);
    }
}
