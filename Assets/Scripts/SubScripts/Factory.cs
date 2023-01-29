using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mentum
{
    /// <summary>
    /// 오브젝트 풀링에 쓰이는 생산자 클래스
    /// </summary>
    public class Factory
    {
        private readonly PoolObject prefab;
        private readonly Transform spawnParent;
        private readonly int defaultPoolSize;

        private Queue<PoolObject> pool;
        public List<PoolObject> SpawnedPool { get; set; }

        public Factory(PoolObject prefab, Transform spawnParent, int defaultPoolSize)
        {
            this.prefab = prefab;
            this.spawnParent = spawnParent;
            this.defaultPoolSize = defaultPoolSize;

            if (this.prefab != null)
                CreatePool();
            else
                Debug.LogAssertion("Prefab이 null 값임");
        }

        private void CreatePool()
        {
            pool = new Queue<PoolObject>(defaultPoolSize);
            SpawnedPool = new List<PoolObject>();

            for (int i = 0; i < defaultPoolSize; i++)
                AddOne();
        }

        private void AddOne()
        {
            PoolObject newOne = Object.Instantiate(prefab, spawnParent);
            newOne.gameObject.SetActive(false);
            pool.Enqueue(newOne);
        }

        private PoolObject Get()
        {
            if (pool.Count == 0)
                AddOne();

            PoolObject obj = pool.Dequeue();
            SpawnedPool.Add(obj);

            return obj;
        }

        // 풀 되돌리기
        public void Insert(PoolObject obj)
        {
            // Assert는 첫번째 항이 거짓일 때 출력함에 주의
            Debug.Assert(obj != null, "Null object가 반환됨");

            obj.onDisableAction -= Insert;
            obj.gameObject.SetActive(false);

            SpawnedPool.Remove(obj);
            pool.Enqueue(obj);
        }

        public PoolObject Spawn(Vector2 position)
        {
            PoolObject curObj = Get();
            curObj.gameObject.SetActive(false);
            curObj.SpawnLocate(position);
            curObj.onDisableAction += Insert;
            curObj.gameObject.SetActive(true);

            return curObj;
        }
    }
}