using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EsceneLoadingTransition : MonoBehaviour
{
    static EsceneLoadingTransition Singleton;

    [SerializeField]
    GameObject mainPanel;

    [SerializeField]
    Transform loadingIcon;

    [SerializeField]
    Image backgroundImg;

    Coroutine animationCoroutine;

    [SerializeField]
    float smoothness = 0.1f;

    private void Awake()
    {
        if (Singleton != null)
        {
            //DestroyImmediate(gameObject);
            Debug.LogWarning($"Another instance of {nameof(EsceneLoadingTransition)} already exists!! Make sure only one exists");
            return;
        }
        Singleton = this;
    }

    private void FixedUpdate()
    {
        if (!mainPanel.activeSelf)
            return;

        loadingIcon.Rotate(Vector3.forward, -5, Space.Self);
    }

    public static void Show(bool show)
    {
        if (Singleton == null)
            return;

        Singleton.ShowPanel(show);
    }

    void ShowPanel(bool show)
    {
        Debug.Log($"EsceneLoadingTransition {show}");

        // already in that state
        if (show == mainPanel.activeSelf)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        mainPanel.SetActive(true);

        if (show)
            animationCoroutine = StartCoroutine(ShowAnimation());
        else
            animationCoroutine = StartCoroutine(HideAnimation());
    }

    IEnumerator ShowAnimation()
    {
        float currentAlpha = 0;
        SetAlpha(currentAlpha);

        while (currentAlpha < 1f)
        {
            // move alhpa
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, smoothness * 0.1f * Time.deltaTime);
            SetAlpha(currentAlpha);

            // wait
            yield return new WaitForEndOfFrame();
        }

        SetAlpha(1);
        animationCoroutine = null;
    }

    IEnumerator HideAnimation()
    {
        float currentAlpha = 1;
        SetAlpha(currentAlpha);

        while (currentAlpha > 0)
        {
            // move alhpa
            currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, smoothness * 0.1f * Time.deltaTime);
            SetAlpha(currentAlpha);

            // wait
            yield return new WaitForEndOfFrame();
        }

        SetAlpha(0);
        mainPanel.SetActive(false);
        animationCoroutine = null;
    }

    void SetAlpha(float alpha)
    {
        backgroundImg.color = new Color(backgroundImg.color.r, backgroundImg.color.g, backgroundImg.color.b, alpha);
    }
}
