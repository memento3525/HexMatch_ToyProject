using Mentum.HexMatch;
using UnityEngine;

namespace Mentum
{
    public class EffectManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] PoolObject killedBlockPrefab;
        [SerializeField] PoolObject matchEffPrefab;

        [Header("Transform")]
        [SerializeField] Transform killedBlockParent;
        [SerializeField] Transform matchEffParent;

        private Factory killedFactory;
        private Factory matchFactory;

        public static EffectManager Inst { get; private set; }

        private void Awake()
        {
            Inst = this;
            killedFactory = new Factory(killedBlockPrefab, killedBlockParent, 3);
            matchFactory = new Factory(matchEffPrefab, matchEffParent, 3);
        }

        public void SpawnKilledBlock(Vector2 pos, Node.Type type)
        {
            PoolObject obj = killedFactory.Spawn(pos);
            KilledBlock killedBlock = obj.GetComponent<KilledBlock>();
            killedBlock.Setup(type, pos);

            SpawnMatchEff(pos);
        }

        public void SpawnMatchEff(Vector2 pos)
        {
            matchFactory.Spawn(pos);
        }
    }
}