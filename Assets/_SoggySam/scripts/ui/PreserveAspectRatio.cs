using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PreserveAspectRatio : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // Calculate the target aspect ratio
        float targetAspectRatio = image.sprite.rect.width / image.sprite.rect.height;

        // Calculate the current aspect ratio
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        // Calculate the aspect ratio difference
        float aspectRatioDifference = Mathf.Abs(currentAspectRatio - targetAspectRatio);

        // Set the image's scale based on the aspect ratio difference
        if (aspectRatioDifference < 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            float scale = currentAspectRatio / targetAspectRatio;
            transform.localScale = new Vector3(scale, 1, 1);
        }
    }
}
