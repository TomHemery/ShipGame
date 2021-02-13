using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    private Texture2D texture = null;
    public float maxNoise = 200;
    private Color32[] pixels = null;

    public float corruptionDuration = 5.0f;
    private float corruptionTimer;

    private void Awake()
    {
        corruptionTimer = corruptionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (corruptionTimer > 0.0f)
        {
            corruptionTimer -= Time.deltaTime;
            if (pixels == null) pixels = texture.GetPixels32();

            for (int i = 0; i < corruptionTimer.Map(0, corruptionDuration, 0, maxNoise); i++)
            {
                int x = Random.Range(0, texture.width);
                int y = Random.Range(0, texture.height);

                int x2 = Random.Range(0, texture.width);
                int y2 = Random.Range(0, texture.height);

                SetArea(x, y, Random.Range(1, 3), pixels[x2 + y2 * texture.width]);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }
    }

    private void SetArea(int x, int y, int r, Color32 colour) {
        for (int i = -r; i <= r; i++) {
            for (int j = -r; j <= r; j++) {
                int index = x + i + (y + j) * texture.width;
                if (index > 0 && index < pixels.Length) pixels[index] = colour;
            }
        }
    }

    public void CaptureScreen() {
        int resWidth = Screen.width;
        int resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        Camera.main.targetTexture = rt;
        
        texture = null;
        texture = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        
        RenderTexture.active = rt;
        Camera.main.Render();
        
        texture.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        texture.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        Image backgroundImage = GetComponentInChildren<Image>();
        backgroundImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f));

    }
}
