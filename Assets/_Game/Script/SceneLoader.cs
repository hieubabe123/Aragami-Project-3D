using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : SingletonDontDestroy<SceneLoader>
{
    [SerializeField] private Animator screenAnimator;
    [SerializeField] private Image cover;

    private bool isFading = false;
    private Coroutine sceneLoadCoroutine;
    private void Start()
    {
        EventDispatcher.AddListener<EventDefine.OnChangeScene>(HandleSceneChange);
        FadeOut();
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveListener<EventDefine.OnChangeScene>(HandleSceneChange);
    }

    /// <summary>
    /// Handles scene change request.
    /// </summary>
    private void HandleSceneChange(EventDefine.OnChangeScene scene)
    {
        Debug.Log($"Loading scene: {scene.SceneName}");
        SFX.Instance.StopCurrentBgMusic();

        if (sceneLoadCoroutine != null)
        {
            StopCoroutine(sceneLoadCoroutine);
        }
        sceneLoadCoroutine = StartCoroutine(LoadSceneWithFade(scene.SceneName));
    }

    /// <summary>
    /// Loads a scene with fade-in and fade-out effect.
    /// </summary>
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        FadeIn();
        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

        yield return new WaitForSeconds(1f);

        FadeOut();
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);

        sceneLoadCoroutine = null;
    }

    /// <summary>
    /// Triggers fade-out animation.
    /// </summary>
    public void FadeOut()
    {
        if (screenAnimator != null)
            screenAnimator.Play("FadeOut");
        isFading = false;
        cover.raycastTarget = isFading;
    }

    /// <summary>
    /// Triggers fade-in animation.
    /// </summary>
    public void FadeIn()
    {
        if (screenAnimator != null)
            screenAnimator.Play("FadeIn");
        isFading = true;
        cover.raycastTarget = isFading;
    }
}
