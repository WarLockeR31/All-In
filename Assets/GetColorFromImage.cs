using UnityEngine;
using UnityEngine.UI;

public class GetColorFromImage : MonoBehaviour
{
    public Image image;           // Image �� ��������
    public Transform childObject; // �������� ������ ������ Image
    public Color color;
    private Texture2D spriteTexture;

    void Start()
    {
        // ����������� ������ � Texture2D
        spriteTexture = SpriteToTexture(image.sprite);
    }

    void Update()
    {
        color = GetColorAtChild();
        //Debug.Log($"���� ��� ��������: {color}");
    }

    Color GetColorAtChild()
    {
        if (spriteTexture == null) return Color.clear;

        // �������� ���������� ��������� ������� ������������ Image
        Vector2 localPoint = childObject.localPosition;

        // ��������� � ���������� ��������
        RectTransform rt = image.rectTransform;
        float px = Mathf.InverseLerp(-rt.rect.width / 2, rt.rect.width / 2, localPoint.x) * spriteTexture.width;
        float py = Mathf.InverseLerp(-rt.rect.height / 2, rt.rect.height / 2, localPoint.y) * spriteTexture.height;

        // ��������� ���� �������
        return spriteTexture.GetPixel((int)px, (int)py);
    }

    Texture2D SpriteToTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x, (int)sprite.rect.y,
            (int)sprite.rect.width, (int)sprite.rect.height
        );
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
