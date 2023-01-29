using Sirenix.OdinInspector;
using UnityEngine;

namespace Mentum.HexMatch
{
    [CreateAssetMenu(), System.Serializable]
    public class GameSetupSO : SerializedScriptableObject
    {
        public int width = 6; // 좌표계 칸수
        public int height = 6; // 좌표계 칸수
        public Vector2Int generationPoint = new(5, 5); // 생산 좌표

        [HideInInspector]
        public ArrayLayout arrayLayout; // 실제 빌드시 넘어가는 데이터
        public bool[,] boardMatrix = new bool[6, 6]; // 에디터 설정용 데이터 // x가로열(열) y세로열(행)

        /// <summary>
        /// 유니티에서 다차원배열이 Serialize가 안되서 boardMatrix => arrayLayout 로 변환
        /// </summary>
        private void OnValidate()
        {
            SetMatrixSize();
        }

        public void SetMatrixSize()
        {
            bool[,] temp = (bool[,])boardMatrix.Clone();
            boardMatrix = new bool[width, height];

            int width_size = Mathf.Min(width, temp.GetLength(0)); // 가로열
            int height_size = Mathf.Min(height, temp.GetLength(1)); // 세로행

            for (int y = 0; y < height_size; y++)
            {
                for (int x = 0; x < width_size; x++)
                    boardMatrix[x, y] = temp[x, y];
            }

            arrayLayout = new ArrayLayout(width, height, boardMatrix);
        }
    }
}