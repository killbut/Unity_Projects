using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationObjects
    {
        private Sequence mySequence;
        private  Vector3 _toChange;

        public AnimationObjects()
        {
            mySequence = DOTween.Sequence();
        }

        public void Bounce(GameObject value,float duration)
        {
            _toChange = value.transform.localScale;
            value.transform.localScale=Vector3.zero;
            mySequence.Append(value.transform.DOScale(_toChange + new Vector3(0.3f, 0.3f, 0), duration))
                      .Append(value.transform.DOScale(_toChange, duration))
                      .OnComplete(() => WhenComplete());
        }
        /// <summary>
        /// анимация смены текста
        /// </summary>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        public  void FadeIn(TextMeshProUGUI text,float duration)
        {
            mySequence.Append(text.DOFade(0, 0))
                      .Append(text.DOFade(1, duration))
                      .OnComplete(() => WhenComplete());
        }
        /// <summary>
        ///  анимация при неправильном выборе
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        public  void Snake(GameObject value, float duration)
        {
            mySequence.Append(value.transform.DOShakePosition(duration, new Vector3(1f, 0f, 0), 10, 0f, false, true))
                      .OnComplete(() => WhenComplete());
        }
        /// <summary>
        /// Затемнение сцены
        /// </summary>
        /// <param name="image"></param>
        /// <param name="duration"></param>
        public void Fading(Image image,float duration)
        {
            mySequence.Append(image.DOFade(0.7f, 1f))
                      .OnComplete(() => WhenComplete());
        }
        /// <summary>
        /// Имитация загрузки
        /// </summary>
        /// <param name="image"></param>
        /// <param name="duration"></param>
        public void Restarting(Image image,float duration)
        {
            mySequence.Append(image.DOFade(1f,duration))
                      .Append(image.DOFade(0f,0f))
                      .OnComplete(() => WhenComplete());
        }
        
        /// <summary>
        /// уничтожение sequence при окончаний анимаций
        /// </summary>
        private void WhenComplete()
        {
            Debug.Log("Complete");
            mySequence?.Kill();
        }
    }
