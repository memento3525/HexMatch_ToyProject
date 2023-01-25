using System;
using UnityEngine;

/// <summary>
/// ������Ʈ Ǯ���� ���̴� ����Ǵ� �����տ� Ŭ���� (������Ʈ)
/// </summary>
public class PoolObject : MonoBehaviour
{
    public Action<PoolObject> OnDisableAction;

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
        OnDisableAction?.Invoke(this);
    }
}