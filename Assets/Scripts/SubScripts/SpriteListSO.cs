using System.Collections.Generic;
using UnityEngine;

namespace Mentum.Utility
{
    [CreateAssetMenu()]
    public class SpriteListSO : ScriptableObject
    {
        public List<Sprite> sprites = new();

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
}