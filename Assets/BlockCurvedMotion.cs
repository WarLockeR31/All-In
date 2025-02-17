using UnityEngine;
using UnityEngine.UI;

public class ImageCurvePingPong : MonoBehaviour
{

    private Animator animator;
    public RectTransform imageRect;  // Ссылка на Image
    public AnimationCurve curve;     // Кривая движения
    public float duration = 1f;      // Время движения туда и обратно
    public Vector2 startPos;         // Начальная позиция
    public Vector2 endPos;           // Конечная позиция

    private float time = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = imageRect.anchoredPosition; // Запоминаем стартовую позицию
    }

    void Update()
    {
        if (animator.GetBool("isBlocking"))
        {
            time += Time.deltaTime;
            float t = Mathf.PingPong(time / duration, 1f); // Двигаемся туда и обратно (0 → 1 → 0)

            float curveValue = curve.Evaluate(t); // Получаем значение кривой
            Vector2 newPos = Vector2.Lerp(startPos, endPos, t); // Движение по X
            newPos.y += curveValue * 100f; // Добавляем отклонение по кривой

            imageRect.anchoredPosition = newPos; // Устанавливаем новую позицию
        }
    }
}
