using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mentum.HexMatch
{
    /// <summary>
    /// 좌표계이다. 슬롯의 기능을 함.
    /// </summary>
    [System.Serializable]
    public class Node
    {
        public static int colorLength = Enum.GetNames(typeof(Type)).Length - 2;

        public enum Type
        {
            Hole = -1, // 비어있어서, 절대 채워지지않는 공간
            Blank = 0, // 임시로 비워져서 블럭이 채워져야하는 공간
            Red = 1,
            Green = 2,
            Blue = 3,
            Purple = 4,
            Yellow = 5,
            White = 6,
        }

        public Type type;
        public Point point;
        private Block block;

        public Node(Type t, Point p)
        {
            type = t;
            point = p;
        }

        public void SetBlock(Block b)
        {
            block = b;
            type = (block == null) ? Type.Blank : block.type;
            if (block == null) return;

            block.SetPoint(point);
        }

        public void SetType(Type t)
        {
            type = t;
        }

        public Block GetBlock() => block;

        /// <summary>
        /// 컬러 블록인지 체크. 주로 연결검사를 해야되는지 체크할때 사용.
        /// </summary>
        public static bool IsColorBlock(Type type)
        {
            return type != Type.Hole && type != Type.Blank;
        }

        public bool IsColorBlock()
        {
            return type != Type.Hole && type != Type.Blank;
        }

        /// <summary>
        /// 컬러블록의 타입만을 리턴한다.
        /// </summary>
        public static Type GetRandomColorType()
        {
            int newType = UnityEngine.Random.Range(0, colorLength) + 1;
            return (Type)newType;
        }
    }
}