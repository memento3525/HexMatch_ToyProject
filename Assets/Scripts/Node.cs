using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ǥ���̴�. ������ ����� ��.
/// </summary>
[System.Serializable]
public class Node
{
    public static int colorLength = Enum.GetNames(typeof(Type)).Length - 2;

    public enum Type
    {
        Hole = -1, // ����־, ���� ä�������ʴ� ����
        Blank = 0, // �ӽ÷� ������� ���� ä�������ϴ� ����
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
    /// �÷� ������� üũ. �ַ� ����˻縦 �ؾߵǴ��� üũ�Ҷ� ���.
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
    /// �÷������ Ÿ�Ը��� �����Ѵ�.
    /// </summary>
    public static Type GetRandomColorType()
    {
        int newType = UnityEngine.Random.Range(0, colorLength) + 1;
        return (Type)newType;
    }
}