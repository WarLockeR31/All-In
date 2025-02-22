using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Настройки тряски")]
    [SerializeField] private float baseDuration = 0.2f;
    [SerializeField] private float maxPower = 0.3f;
    [SerializeField] private float punchIntensity = 1;
    [SerializeField] private float takeDamageIntensity = 1;
    [SerializeField] private Transform camera;

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    #region Singleton
    public static ScreenShake Instance { get; private set; }

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
        originalPosition = camera.localPosition;
    }

    public void TriggerPunchShake()
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(Shake(punchIntensity));
    }

    public void takeDamageShake()
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(Shake(takeDamageIntensity));
    }

    private IEnumerator Shake(float intensityMultiplier)
    {
        float elapsed = 0f;
        float currentPower = Mathf.Clamp(intensityMultiplier, 0.1f, 2f) * maxPower;

        while (elapsed < baseDuration)
        {
            // Линейное затухание силы
            float damp = 1 - (elapsed / baseDuration);

            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            ) * currentPower * damp;

            camera.localPosition = originalPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        camera.localPosition = originalPosition;
    }
}