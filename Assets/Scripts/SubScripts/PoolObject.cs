using System;
using UnityEngine;

namespace Mentum
{
    /// <summary>
    /// ������Ʈ Ǯ���� ���̴� ����Ǵ� �����տ� Ŭ���� (������Ʈ)
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        public Action<PoolObject> onDisableAction;

        public void SpawnLocate(Vector2 spawnPos)
        {
            ((RectTransform)transform).anchoredPosition = spawnPos;
        }

        public void SpawnLocate(Vector3 spawnPos)
        {
            transform.position = spawnPos;
        }

        public void SpawnLocate(Vector3 spawnPos, Quaternion spawnRot)
        {
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }

        private void OnDisable()
        {
            onDisableAction?.Invoke(this);
        }
    }
}