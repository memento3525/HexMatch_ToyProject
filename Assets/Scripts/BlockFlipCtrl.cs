using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockFlipCtrl
{
    public Block one;
    public Block two;

    public BlockFlipCtrl(Block o, Block t)
    {
        one = o;
        two = t;
    }

    public Block GetOtherBlock(Block p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }

    public bool IsContained(Block p)
    {
        return p == one || p == two;
    }
}