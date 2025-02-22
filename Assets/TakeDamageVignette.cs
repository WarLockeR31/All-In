using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TakeDamageVignette : MonoBehaviour
{
    public float intensity;
    public float duration;
    public float fadeDuration;

    PostProcessVolume _volume;
    Vignette _vignette;

    #region Singleton
    public static TakeDamageVignette Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        _volume = GetComponent<PostProcessVolume>();

        _volume.profile.TryGetSettings<Vignette>(out _vignette);

        _vignette.enabled.Override(false);
    }

    public void InvokeDamageVignette()
    {
        StartCoroutine(TakeDamageEffect());
    }

    private IEnumerator TakeDamageEffect()
    {
        float curIntensity = intensity;

        _vignette.intensity.Override(curIntensity);
        _vignette.enabled.Override(true);

        yield return new WaitForSeconds(duration);


        float timer = 0;
        while (timer < fadeDuration)
        {
            curIntensity = Mathf.Lerp(intensity, 0, timer / fadeDuration);

            _vignette.intensity.Override(curIntensity);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _vignette.intensity.Override(0);
        _vignette.enabled.Override(false);
    }
}
