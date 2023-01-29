using Mentum.HexMatch;
using Mentum.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Mentum
{
    /// <summary>
    /// 터진 조각이 바닥으로 떨어지는 이펙트
    /// </summary>
    public class KilledBlock : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] SpriteListSO blockSprites;

        private bool falling;

        private const float speed = 40f;
        private const float gravity = 120f;

        private Vector2 moveDir;
        private RectTransform rect;
        private Image img;

        public void Setup(Node.Type type, Vector2 start)
        {
            gameObject.SetActive(true);
            falling = true;

            moveDir = Vector2.up;
            moveDir.x = Random.Range(-1f, 1f);
            moveDir *= speed / 2;

            img = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
            img.sprite = blockSprites[(int)type - 1];
            rect.anchoredPosition = start;
        }

        private void Update()
        {
            if (!falling)
            {
                gameObject.SetActive(false);
                return;
            }

            moveDir.y -= Time.deltaTime * gravity;
            moveDir.x = Mathf.Lerp(moveDir.x, 0, Time.deltaTime);
            rect.anchoredPosition += speed * Time.deltaTime * moveDir;

            if (rect.position.y > Screen.height + St.DOUBLE_CELL)
                falling = false;
        }
    }
}