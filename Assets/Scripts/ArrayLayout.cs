[System.Serializable]
public class ArrayLayout
{
    [System.Serializable]
    public struct RowData
    {
        public bool[] row;
    }

    public RowData[] rows;

    public ArrayLayout(int width, int height, bool[,] values)
    {
        rows = new RowData[width];

        for (int i = 0; i < rows.Length; i++)
            rows[i].row = new bool[height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                rows[x].row[y] = values[x, y];
        }
    }

}
