using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// [!] AnimatorClipTrack(clipDuration) doesnt support clip speed multiplier
// [!] make your clips with speed 1 and only change their duration (Loop clips are exceptions)

public class GenericStateMachine : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    [field: SerializeField] public string DefaultAnimatorClip { get; protected set; } = "idle";
    public string CurrentAnimatorClip { get; protected set; }
    public bool InTransition { get; private set; }

    // events
    public readonly UnityEvent<string> onAnimationPlay = new();
    public readonly UnityEvent<string> onAnimationEnd = new();

    /// <summary>
    /// [!] Be sure to call base.OnTransitionEnd() if you override this method </summary>
    protected virtual void Awake()
    {
        // configure state events
        AnimatorTransitionEvents.onTransitionStart.AddListener(
            (stateAnimator) =>
            {
                // only if its my animator
                if (animator == stateAnimator)
                    OnTransitionStart();
            }
        );

        AnimatorTransitionEvents.onTransitionEnds.AddListener(
            (stateAnimator) =>
            {
                // only if its my animator
                if (animator == stateAnimator)
                    OnTransitionEnd();
            }
        );
    }

    /// <summary>
    /// [!] Be sure to call base.OnTransitionStart() if you override this method </summary>
    protected virtual void OnTransitionStart()
    {
        InTransition = true;
    }

    /// <summary>
    /// [!] Be sure to call base.OnTransitionEnd() if you override this method </summary>
    protected virtual void OnTransitionEnd()
    {
        InTransition = false;
    }


    /// <summary>
    /// Play an animation clip on pet animator </summary>
    public virtual void Play(string clipName, bool overwrite = true, bool fade = true)
    {
        if (!overwrite && clipName == CurrentAnimatorClip)
            return;

        AnimationClip clip = FindAnimation(animator, clipName);
        if (clip == null)
        {
            Debug.LogWarning($"No clip found in stateAnimator ({clipName})", this);
            return;
        }
        Debug.Log($"Playing stateAnimator clip ({clipName}) - {clip.length} seconds", this);

        if (fade)
            animator.CrossFade(clipName, 0.05f, 0, 0);
        else
            animator.Play(clipName);

        CurrentAnimatorClip = clipName;

        StopAllCoroutines();
        StartCoroutine(AnimatorClipTrack(clip.length));

        onAnimationPlay.Invoke(clipName);
    }

    public AnimationClip FindAnimation(Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip;
        }

        return null;
    }


    IEnumerator AnimatorClipTrack(float clipDuration = -1)
    {
        string GetCurrentClipName()
        {
            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips == null || clips.Length == 0)
                return string.Empty;

            return clips[0].clip.name;
        }

        bool IsCurrentClipEnding()
        {
            // get current state
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animTime = stateInfo.normalizedTime;

            return (animTime % 1) >= 0.999f;
        }

        // wait for previous clip ends (and not in transition)
        //yield return new WaitUntil(() => !InTransition);
        yield return new WaitUntil(() => GetCurrentClipName() == CurrentAnimatorClip);
        //yield return new WaitForEndOfFrame();

        // get current state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // looping clips never ends
        if (stateInfo.loop)
            yield break;

        // wait for given clip duration (more accurate by evading transitions)
        if (clipDuration > 0)
            yield return new WaitForSeconds(clipDuration * 0.98f);
        // no give clip duration, wait for clip normalizedTime
        else
        {
            // wait until state finishes
            while (!IsCurrentClipEnding())
                yield return null;
        }

        Debug.Log($"Animation clip ends ({CurrentAnimatorClip})");
        onAnimationEnd.Invoke(CurrentAnimatorClip);
        yield break;
    }

    public virtual void SetAnimatorFloat(string property, float value)
       => animator.SetFloat(property, value);

    public virtual void Stop(string clipName)
    {
        // isnt playing that clip
        if (clipName != CurrentAnimatorClip)
            return;

        Play(DefaultAnimatorClip, overwrite: false, fade: false);
    }

    public virtual void SetController(RuntimeAnimatorController animatorController)
    {
        animator.runtimeAnimatorController = animatorController;
        Play(CurrentAnimatorClip);
    }
}
