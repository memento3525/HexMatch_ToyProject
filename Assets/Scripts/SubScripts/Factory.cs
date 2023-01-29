using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mentum
{
    /// <summary>
    /// ������Ʈ Ǯ���� ���̴� ������ Ŭ����
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
                Debug.LogAssertion("Prefab�� null ����");
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

        // Ǯ �ǵ�����
        public void Insert(PoolObject obj)
        {
            // Assert�� ù��° ���� ������ �� ����Կ� ����
            Debug.Assert(obj != null, "Null object�� ��ȯ��");

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