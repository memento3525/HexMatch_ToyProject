using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mentum
{
    /// <summary>
    /// OnEnable에서 트위닝 애니메이션 실행하는 UI 전용 컴포넌트
    /// </summary>
    public class TweenAnim : MonoBehaviour
    {
        public enum AnimType
        {
            Fade,
            Scale,
            MochiScale,
            AnchorPos
        }

        [SerializeField, EnumToggleButtons] private AnimType AnimationType = AnimType.Fade;

        private ITweenAnimSub tweenAnimSub;

        [HideIf("AnimationType", AnimType.MochiScale)]
        [SerializeField] private Ease openEasy = Ease.OutBack;
        [HideIf("AnimationType", AnimType.MochiScale)]
        [SerializeField] private Ease closeEasy = Ease.InBack;
        [SerializeField] float delay = 0f;
        [HideIf("AnimationType", AnimType.MochiScale)]
        [SerializeField] float openTime = 0.3f;
        [HideIf("AnimationType", AnimType.MochiScale)]
        [SerializeField] float closeTime = 0.2f;

        [ShowIf("AnimationType", AnimType.AnchorPos)]
        [SerializeField] private Vector2 from = new Vector2(-1000f, 0f);
        [ShowIf("AnimationType", AnimType.AnchorPos)]
        [SerializeField] private Vector2 to = new Vector2(0f, 0f);
        [SerializeField, ReadOnly] private Vector2 firstPos;

        [SerializeField] private bool CloseDisable = true;

        public Action OnOpenCompleteAction;
        public Action OnCloseCompleteAction;
        private bool destroyOnClose = false;
        private CanvasGroup canvasGroup;
        private bool isSetup = false;

        private readonly List<Tweener> curTweens = new List<Tweener>();
        private Coroutine waitCoroutine;

        private void InitialSetup()
        {
            if (isSetup) return;
            isSetup = true;

            canvasGroup = GetComponent<CanvasGroup>();

            switch (AnimationType)
            {
                case AnimType.Fade:
                    tweenAnimSub = FadeAnim.instance;
                    break;
                case AnimType.Scale:
                    tweenAnimSub = ScaleAnim.instance;
                    break;
                case AnimType.MochiScale:
                    tweenAnimSub = MochiAnim.instance;
                    openTime = 0.2f;
                    break;
                case AnimType.AnchorPos:
                    firstPos = ((RectTransform)transform).anchoredPosition;
                    tweenAnimSub = AnchorPosAnim.instance;
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            ResetToIdle();

            if (delay > 0)
                waitCoroutine = StartCoroutine(WaitCoroutine(delay));
            else
                tweenAnimSub.Open(this);
        }

        private IEnumerator WaitCoroutine(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            tweenAnimSub.Open(this);
        }

        // 애니메이션이 없는 상태로 돌리기
        public void ResetToIdle()
        {
            InitialSetup();
            destroyOnClose = false;
            KillTweens();
            tweenAnimSub.ResetToIdle(this);
        }

        private void KillTweens()
        {
            foreach (var item in curTweens)
                if (item != null && item.IsActive())
                    item.Kill();

            curTweens.Clear();

            if (waitCoroutine != null)
                StopCoroutine(waitCoroutine);
        }

        private void OnDisable() => KillTweens();

        private void OnOpenComplete()
        {
            OnOpenCompleteAction?.Invoke();
        }

        [Button]
        public void Close()
        {
            KillTweens();
            tweenAnimSub.Close(this);
        }

        public void CloseAndDestroy()
        {
            destroyOnClose = true;
            Close();
        }


        private void OnCloseComplete()
        {
            OnCloseCompleteAction?.Invoke();

            if (CloseDisable)
                gameObject.SetActive(false);

            if (destroyOnClose)
                Destroy(gameObject);
        }

        [Button]
        public void AddCanvasGroup() => gameObject.AddComponent<CanvasGroup>();

        private interface ITweenAnimSub
        {
            void Open(TweenAnim tweenAnim);
            void Close(TweenAnim tweenAnim);
            void ResetToIdle(TweenAnim tweenAnim);
        }


        /// <summary>
        /// 트윈 애니메이션을 정의하는 내부 클래스
        /// </summary>
        private class FadeAnim : ITweenAnimSub
        {
            public static FadeAnim instance = new FadeAnim();

            public void Open(TweenAnim tweenAnim)
            {
                tweenAnim.canvasGroup.alpha = 0f;
                Tweener newTween = tweenAnim.canvasGroup.DOFade(1, tweenAnim.openTime)
                                              .OnComplete(tweenAnim.OnOpenComplete)
                                              .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void Close(TweenAnim tweenAnim)
            {
                tweenAnim.canvasGroup.alpha = 1f;
                Tweener newTween = tweenAnim.canvasGroup.DOFade(0, tweenAnim.closeTime)
                                                          .OnComplete(tweenAnim.OnCloseComplete)
                                                          .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void ResetToIdle(TweenAnim tweenAnim)
            {
                tweenAnim.canvasGroup.alpha = 0f;
            }
        }

        private class ScaleAnim : ITweenAnimSub
        {
            public static ScaleAnim instance = new ScaleAnim();

            public void Open(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.zero;
                Tweener newTween = tweenAnim.transform.DOScale(Vector3.one, tweenAnim.openTime)
                                                        .SetEase(tweenAnim.openEasy)
                                                        .OnComplete(tweenAnim.OnOpenComplete)
                                                        .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void Close(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.one;
                Tweener newTween = tweenAnim.transform.DOScale(Vector3.zero, tweenAnim.closeTime)
                                                        .SetEase(tweenAnim.closeEasy)
                                                        .OnComplete(tweenAnim.OnCloseComplete)
                                                        .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void ResetToIdle(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.zero;
            }
        }

        private class MochiAnim : ITweenAnimSub
        {
            public static MochiAnim instance = new MochiAnim();

            public void Open(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.zero;
                Tweener newTweenX = tweenAnim.transform.DOScaleX(1f, tweenAnim.openTime)
                    .SetEase(Ease.OutCirc)
                    .OnComplete(tweenAnim.OnOpenComplete)
                    .SetUpdate(true);
                Tweener newTweenY = tweenAnim.transform.DOScaleY(1f, tweenAnim.openTime)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
                Tweener newTweenZ = tweenAnim.transform.DOScaleZ(1f, tweenAnim.openTime)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);

                tweenAnim.curTweens.Add(newTweenX);
                tweenAnim.curTweens.Add(newTweenY);
                tweenAnim.curTweens.Add(newTweenZ);
            }

            public void Close(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.one;
                tweenAnim.transform.DOScale(Vector3.zero, tweenAnim.closeTime)
                   .SetEase(tweenAnim.closeEasy)
                   .OnComplete(tweenAnim.OnCloseComplete)
                   .SetUpdate(true);
            }

            public void ResetToIdle(TweenAnim tweenAnim)
            {
                tweenAnim.transform.localScale = Vector3.zero;
            }
        }

        private class AnchorPosAnim : ITweenAnimSub
        {
            public static AnchorPosAnim instance = new AnchorPosAnim();

            public void Open(TweenAnim tweenAnim)
            {
                var rect = (RectTransform)tweenAnim.transform;
                var from = tweenAnim.firstPos + tweenAnim.from;
                var to = tweenAnim.firstPos + tweenAnim.to;

                rect.anchoredPosition = from;
                Tweener newTween = rect.DOAnchorPos(to, tweenAnim.openTime)
                                         .SetEase(tweenAnim.openEasy)
                                         .OnComplete(tweenAnim.OnOpenComplete)
                                         .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void Close(TweenAnim tweenAnim)
            {
                var rect = (RectTransform)tweenAnim.transform;
                var from = tweenAnim.firstPos + tweenAnim.from;
                var to = tweenAnim.firstPos + tweenAnim.to;

                rect.anchoredPosition = to;
                Tweener newTween = rect.DOAnchorPos(from, tweenAnim.closeTime)
                                         .SetEase(tweenAnim.closeEasy)
                                         .OnComplete(tweenAnim.OnCloseComplete)
                                         .SetUpdate(true);

                tweenAnim.curTweens.Add(newTween);
            }

            public void ResetToIdle(TweenAnim tweenAnim)
            {
                var rect = (RectTransform)tweenAnim.transform;
                rect.anchoredPosition = tweenAnim.firstPos;
            }
        }
    }
}