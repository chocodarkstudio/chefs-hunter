using DG.Tweening;
using ScreenTransition;
using UnityEngine;

namespace ScreenTransition
{
    public enum TransitionState { Open, Close }
}

namespace UIAnimShortcuts
{
    public static class UIAnim
    {

        public static Tween Scale(Transform t, Vector3 targetValue, float duration = 0.2f, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            var tween = DOTween.To(
                  () => t.localScale,     // getter
                  x => t.localScale = x,  // setter
                  targetValue,            // target value
                  duration   // duration
              );

            if (callback != null)
                tween.onComplete += callback;

            return tween;
        }

        public static Tween Scale(Transform t, TransitionState targetState, float duration = 0.2f, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            Vector3 initialValue = targetState == TransitionState.Open ? Vector3.zero : Vector3.one;
            Vector3 targetValue = targetState == TransitionState.Open ? Vector3.one : Vector3.zero;

            t.localScale = initialValue;

            return Scale(t, targetValue, duration, callback);
        }

        public static Sequence ClickMe(Transform t, float speed = 1)
        {
            // invalid object
            if (t == null)
                return null;

            Vector3 initialValue = Vector3.one;

            // Grab a free Sequence to use
            Sequence sequence = DOTween.Sequence();
            sequence.timeScale = speed;

            // two times sequence
            for (int i = 0; i < 2; i++)
            {
                // Add a up scale tween
                sequence.Append(
                    DOTween.To(
                    () => t.localScale,     // getter
                    x => t.localScale = x,  // setter
                    Vector3.one * 1.15f,            // target value
                    0.15f   // duration
                ));

                // Add a scale tween
                sequence.Append(
                    DOTween.To(
                    () => t.localScale,     // getter
                    x => t.localScale = x,  // setter
                    initialValue,            // target value
                    0.2f   // duration
                ));
            }

            return sequence;
        }

        public static Sequence ClickMeInfinity(Transform t, float speed = 1)
            => MakeItLoop(ClickMe(t, speed), interval: 1f);



        /// <summary>
        /// Button click scale animation </summary>
        public static Sequence BtnClick(Transform t, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            Vector3 initialValue = Vector3.one;
            Vector3 targetValue = Vector3.one * 0.7f;

            // set initial value
            t.localScale = initialValue;

            // Grab a free Sequence to use
            Sequence sequence = DOTween.Sequence();

            // Add a scale tween
            sequence.Append(
                DOTween.To(
                () => t.localScale,     // getter
                x => t.localScale = x,  // setter
                targetValue,            // target value
                0.1f   // duration
            ));

            // Add a scale tween
            sequence.Append(
                DOTween.To(
                () => t.localScale,     // getter
                x => t.localScale = x,  // setter
                initialValue,            // target value
                0.1f   // duration
            ));

            if (callback != null)
                sequence.onComplete += callback;

            return sequence;
        }

        public static Tween Move(Transform t, Vector3 targetPos, Vector3? initialPos = null, float duration = 0.2f, bool localSpace = true, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            Tween tween;
            // Local Space
            if (localSpace)
            {
                if (initialPos.HasValue)
                    t.localPosition = initialPos.Value;

                tween = DOTween.To(
                      () => t.localPosition,     // getter
                      x => t.localPosition = x,  // setter
                      targetPos,            // target value
                      duration   // duration
                  );
            }
            // Global Space
            else
            {
                if (initialPos.HasValue)
                    t.position = initialPos.Value;

                tween = DOTween.To(
                      () => t.position,     // getter
                      x => t.position = x,  // setter
                      targetPos,            // target value
                      duration   // duration
                  );
            }


            if (callback != null)
                tween.onComplete += callback;

            return tween;
        }

        public static Sequence Jump(Transform t, Vector3 directionPos, Vector3? initialPos = null, float duration = 0.2f, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            // set initial pos
            if (initialPos.HasValue)
                t.localPosition = initialPos.Value;
            else
                initialPos = t.localPosition;

            // Grab a free Sequence to use
            Sequence sequence = DOTween.Sequence();

            // Add a move tween to target
            sequence.Append(
                DOTween.To(
                () => t.localPosition,     // getter
                x => t.localPosition = x,  // setter
               initialPos.Value + directionPos,            // target value
                duration   // duration
            ));

            // Add a move tween to initial
            sequence.Append(
                DOTween.To(
                () => t.localPosition,     // getter
                x => t.localPosition = x,  // setter
                initialPos.Value,            // target value
                duration   // duration
            ));

            if (callback != null)
                sequence.onComplete += callback;

            return sequence;
        }



        public static Tween Spin(Transform t, float duration = 0.5f, TweenCallback callback = null)
        {
            // invalid object
            if (t == null)
                return null;

            Vector3 targetRot = t.localEulerAngles - Vector3.forward * 360;

            var tween = DOTween.To(
                  () => t.localEulerAngles,     // getter
                  x => t.localEulerAngles = x,  // setter
                  targetRot,            // target value
                  duration   // duration
              );

            if (callback != null)
                tween.onComplete += callback;

            return tween;
        }


        /// <param name="RPS"> Revolution per seconds </param>
        public static Sequence SpinInfinity(Transform t, float RPS = 1)
            => MakeItLoop(Spin(t, RPS));


        public static Sequence MakeItRepeat(Tween t, int times, float interval = 0, LoopType loopType = LoopType.Restart)
        {
            // invalid object
            if (t == null)
                return null;

            // Grab a free Sequence to use
            Sequence sequence = DOTween.Sequence();

            sequence.Append(t);

            // wait a bit to repeat the previous sequence
            sequence.AppendInterval(interval);

            // set as inifnity loop
            return sequence.SetLoops(times, loopType);
        }



        public static Sequence MakeItLoop(Tween t, float interval = 0, LoopType loopType = LoopType.Restart)
        {
            // invalid object
            if (t == null)
                return null;

            return MakeItRepeat(t, -1, interval, loopType);
        }


        /// <summary>
        /// Teaches the player to close this panel with a swipe track </summary>
        /// <remarks> Infinity loop </remarks>
        /// <param name="direction"> direction is added to Global position </param>
        public static Sequence SrollHint(Transform t, Vector3 direction, float interval = 8f)
        {
            // move the panel down
            var tween = Move(
                t: t,
                targetPos: t.position + direction,
                localSpace: false,
                duration: 0.15f
            );

            // do a repeat
            var repeatedTween = MakeItRepeat(tween, times: 3, interval: 0.5f);

            // do a inifinity loop
            return MakeItLoop(repeatedTween, interval);
        }
    }

}
