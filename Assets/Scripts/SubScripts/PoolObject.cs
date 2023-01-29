using System;
using UnityEngine;

namespace Mentum
{
    /// <summary>
    /// 오브젝트 풀링에 쓰이는 생산되는 프리팹용 클래스 (컴포넌트)
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