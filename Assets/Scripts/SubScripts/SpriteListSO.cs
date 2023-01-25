using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SpriteListSO : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();

    public Sprite this[int n]
    {
        get
        {
            try
            {
                return sprites[n];
            }
            catch
            {
                Debug.Log(n);
                return null;
            }
        }
    }
}
