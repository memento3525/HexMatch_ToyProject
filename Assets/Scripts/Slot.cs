using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mentum.HexMatch
{
    /// <summary>
    /// Node클래스의 디버깅용으로 작성.
    /// </summary>
    public class Slot : MonoBehaviour
    {
        public Node node;
        RectTransform rect;

        public void InitSetup(Node node, Vector2 pos)
        {
            this.node = node;
            rect = GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            transform.name = $"Slot ({node.point.x}, {node.point.y})";
        }
    }
}