using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Cần thiết cho Decal

public class ShadowEffectController : MonoBehaviour
{
    private const string FADE_AMOUNT = "_FadeAmount";
    private const string RANDOMAMOUNT = "_RandomAmount";
    [SerializeField] private float fadeDuration = 2.0f;


    private float minRandomAmount = 15f;
    private float maxRandomAmount = 25f;
    private float randomAmount;

    private Material _decalMaterial;
    private DecalProjector _decalProjector;

    void Awake()
    {
        _decalProjector = GetComponent<DecalProjector>();
        if (_decalProjector != null)
        {
            _decalMaterial = new Material(_decalProjector.material);
            _decalProjector.material = _decalMaterial;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(StartLifeCycleCoroutine());
    }

    private IEnumerator StartLifeCycleCoroutine()
    {
        if (_decalMaterial == null)
        {
            yield break;
        }


        // Bắt đầu hiệu ứng mờ dần
        ShadowFadeIn();
        //yield return StartCoroutine(FadeInCoroutine());

        // Chờ một khoảng thời gian trước khi mờ đi
        yield return new WaitForSeconds(fadeDuration);

        // Bắt đầu hiệu ứng mờ đi
        yield return StartCoroutine(FadeOutCoroutine());
    }


    // Coroutine để TẠO bóng

    private void ShadowFadeIn()
    {
        randomAmount = Random.Range(minRandomAmount, maxRandomAmount);
        _decalMaterial.SetFloat(RANDOMAMOUNT, randomAmount);
        _decalMaterial.SetFloat(FADE_AMOUNT, 0);
    }
    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        randomAmount = Random.Range(minRandomAmount, maxRandomAmount);
        _decalMaterial.SetFloat(RANDOMAMOUNT, randomAmount);
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float amount = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            _decalMaterial.SetFloat(FADE_AMOUNT, amount);
            yield return null;
        }
        _decalMaterial.SetFloat(FADE_AMOUNT, 0);
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float amount = Mathf.Clamp01(elapsedTime / fadeDuration);
            _decalMaterial.SetFloat(FADE_AMOUNT, amount);
            yield return null;
        }
        _decalMaterial.SetFloat(FADE_AMOUNT, 1);

        gameObject.SetActive(false);
    }

}