using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAsteroid : MonoBehaviour
{

    private SpriteRenderer m_renderer;
    private Texture2D m_texture;
    private Sprite [] m_sprites;

    private DamageAnimation damageAnimation;

    private readonly byte clear = 0;
    private float maxRealDistance;

    public int pixelsPerUnit = 10;
    public int width = 64;
    public int height = 64;
    public int numFrames = 6;
    public int numWhorleyNoisePoints = 20;
    public int minCraters = 0;
    public int maxCraters = 5;
    public int minCraterRadius = 1;
    public int maxCraterRadius = 3;
    public int craterNoise = 1;
    public int minCratersDestruction = 0;
    public int maxCratersDestruction = 2;
    public int minCraterRadiusDestruction = 1;
    public int maxCraterRadiusDestruction = 3;
    public int craterNoiseDestruction = 1;
    public int destructionNeighbourhood = 1;
    public float destructionNoise = 0.5f;

    public Rect whorleyNoisePointBounds;

    public float threshold = 0.55f;
    public byte shadowValue;
    public int mid1ShadingNeighbourhood = 2;
    public byte midValue1;
    public int mid2ShadingNeighbourhood = 2;
    public byte midValue2;
    public int mid3ShadingNeighbourhood = 2;
    public byte midValue3;
    public int highlightShadingNeighbourhood = 2;
    public byte highlightValue;
    public byte shadingNoise = 10;

    public Color baseColour;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_sprites = new Sprite[numFrames];
        maxRealDistance = Mathf.Sqrt(width * width + height * height);
        GenerateAsteroid();
    }

    private void GenerateAsteroid() {
        //generate the texture
        m_texture = GenerateTexture();

        for (int i = 0; i < numFrames; i++) m_sprites[i] = Sprite.Create(m_texture, new Rect(i * width, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);

        m_renderer.sprite = m_sprites[0];

        damageAnimation = GetComponent<DamageAnimation>();
        damageAnimation.sprites = m_sprites;

        gameObject.AddComponent<PolygonCollider2D>();
    }

    private Texture2D GenerateTexture() {
        //create the texture we will populate
        Texture2D result = new Texture2D(width * numFrames, height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        byte[] pixels = new byte[width * numFrames * height];

        for (int texLeft = 0; texLeft < width * numFrames; texLeft += width)
        {
            if (texLeft == 0)
            { //base case -> generate the first map of the asteroid 
                //create some points in the area of the texture
                Vector2[] whorleyNoisePoints = new Vector2[numWhorleyNoisePoints];

                //assign every pixel a grayscale value based on distance from the nearest point 
                byte brightest = 0;
                byte darkest = byte.MaxValue;
                for (int i = 0; i < numWhorleyNoisePoints; i++)
                    whorleyNoisePoints[i] = new Vector2(Random.Range(whorleyNoisePointBounds.x, whorleyNoisePointBounds.width),
                                                        Random.Range(whorleyNoisePointBounds.y, whorleyNoisePointBounds.height));
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        byte value = DistanceToGrayscale(x, y, whorleyNoisePoints);
                        if (value > brightest) brightest = value;
                        if (value < darkest) darkest = value;
                        SetPixel(x, y, value, pixels);
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        byte normalizedValue = NormalizeByte(GetPixel(x, y, pixels), darkest, brightest);
                        SetPixel(x, y, normalizedValue, pixels);
                        if (normalizedValue > threshold)
                        {
                            SetPixel(x, y, shadowValue, pixels);
                        }
                        else
                        {
                            SetPixel(x, y, clear, pixels);
                        }
                    }
                }

                //punch out holes
                int numCraters = Random.Range(minCraters, maxCraters + 1);
                for (int i = 0; i < numCraters; i++)
                {
                    int radius = Random.Range(minCraterRadius, maxCraterRadius + 1);
                    Vector2 pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
                    for (int x = (pos.x - radius < 0) ? 0 : (int)pos.x - radius; x <= (pos.x + radius > width - 1 ? width - 1 : pos.x + radius); x++)
                    {
                        for (int y = (pos.y - radius < 0) ? 0 : (int)pos.y - radius; y <= (pos.y + radius > height - 1 ? height - 1 : pos.y + radius); y++)
                        {

                            if ((pos.x - x) * (pos.x - x) + (pos.y - y) * (pos.y - y) <= radius * radius + Random.Range(-craterNoise, craterNoise))
                            {
                                SetPixel(x, y, clear, pixels);
                            }
                        }
                    }
                }
            }

            else
            {
                //copy previous "frame" with some noise that eats away at populated pixels with empty neighbours
                CopyPrevFrame(texLeft, pixels);
                for (int tex_x = texLeft; tex_x < texLeft + width; tex_x++)
                {
                    for (int tex_y = 0; tex_y < height; tex_y++)
                    {
                        if (GetPixel(tex_x, tex_y, pixels) == clear)
                        {
                            for (int x = tex_x - destructionNeighbourhood;
                                x <= (tex_x + destructionNeighbourhood >= result.width - 1 ? result.width - 1 : tex_x + destructionNeighbourhood);
                                x++)
                            {
                                for (int y = tex_y - destructionNeighbourhood <= 0 ? 0 : tex_y - destructionNeighbourhood;
                                    y <= (tex_y + destructionNeighbourhood >= height - 1 ? height - 1 : tex_y + destructionNeighbourhood);
                                    y++)
                                {
                                    if (GetPixel(x, y, pixels) != clear && Random.value < destructionNoise)
                                    {
                                        SetPixel(x, y, clear, pixels);
                                    }
                                }
                            }
                        }
                    }
                }
                //punch out more holes
                int numCraters = Random.Range(minCratersDestruction, maxCratersDestruction + 1);
                for (int i = 0; i < numCraters; i++)
                {
                    int radius = Random.Range(minCraterRadiusDestruction, maxCraterRadiusDestruction + 1);
                    Vector2 pos = new Vector2(Random.Range(texLeft, texLeft + width), Random.Range(0, height));
                    for (int x = (pos.x - radius < texLeft) ? texLeft : (int)pos.x - radius; x <= (pos.x + radius > texLeft + width - 1 ? texLeft + width - 1 : pos.x + radius); x++)
                    {
                        for (int y = (pos.y - radius < 0) ? 0 : (int)pos.y - radius; y <= (pos.y + radius > height - 1 ? height - 1 : pos.y + radius); y++)
                        {

                            if ((pos.x - x) * (pos.x - x) + (pos.y - y) * (pos.y - y) <= radius * radius + Random.Range(-craterNoiseDestruction, craterNoiseDestruction))
                            {
                                SetPixel(x, y, clear, pixels);
                            }
                        }
                    }
                }
            }
        }

        ShadeInTexture(pixels);

        Color32[] colours = new Color32[width * numFrames * height];

        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i] != clear)
            {
                colours[i] = new Color32(
                    (byte)(baseColour.r * pixels[i]),
                    (byte)(baseColour.g * pixels[i]),
                    (byte)(baseColour.b * pixels[i]),
                    byte.MaxValue
                );
            }
            else
            {
                colours[i] = Color.clear;
            }
        }

        result.SetPixels32(colours);
        result.Apply();
        return result;
    }

    private void ShadeInTexture(byte [] pixels) {
        ShadePass(clear, shadowValue, midValue1, pixels, mid1ShadingNeighbourhood);
        ShadePass(shadowValue, midValue1, midValue2, pixels, mid2ShadingNeighbourhood);
        ShadePass(midValue1, midValue2, midValue3, pixels, mid3ShadingNeighbourhood);
        ShadePass(midValue2, midValue3, highlightValue, pixels, highlightShadingNeighbourhood);
    }

    private void ShadePass(byte outsideColour, byte targetColour, byte newColour, byte[] pixels, int neighbourhood) {
        float texWidth = width * numFrames;
        for (int x = neighbourhood; x < texWidth - neighbourhood; x++)
        {
            for (int y = neighbourhood; y < height - neighbourhood; y++)
            {
                if (GetPixel(x, y, pixels) >= targetColour)
                { //for each filled in pixel
                    bool onEdge = false;
                    for (int x2 = x - neighbourhood <= 0 ? 0 : x - neighbourhood; x2 <= (x + neighbourhood > texWidth - 1 ? texWidth - 1 : x + neighbourhood); x2++)
                    {
                        for (int y2 = y - neighbourhood <= 0 ? 0 : y - neighbourhood; y2 <= (y + neighbourhood > height - 1 ? height - 1 : y + neighbourhood); y2++)
                        {
                            if (GetPixel(x2, y2, pixels) < targetColour)
                            {
                                onEdge = true;
                                break;
                            }
                        }
                    }
                    if (!onEdge)
                    {
                        SetPixel(x, y, (byte)(newColour + (byte)Random.Range(0, shadingNoise)), pixels);
                    }
                }
            }
        }
    }

    private void SetPixel(int x, int y, byte c,  byte[] pixels) {
        pixels[x + y * width * numFrames] = c;
    }

    private byte GetPixel(int x, int y,  byte[] pixels) {
        return pixels[x + y * width * numFrames];
    }

    private byte NormalizeByte(byte val, byte min, byte max) {
        float top = val - min;
        float bottom = max - min;
        byte result = (byte)(byte.MaxValue * top / bottom);
        return result;
    }

    private void CopyPrevFrame(int newFrameX,  byte[] pixels) {
        for (int x = newFrameX; x < newFrameX + width; x++) {
            for (int y = 0; y < height; y++) {
                SetPixel(x, y, GetPixel(x - width, y,  pixels), pixels);
            }
        }
    }

    private byte DistanceToGrayscale(int x, int y, Vector2[] points) {
        byte minScaledDistance = byte.MaxValue;

        foreach (Vector2 point in points) {
            float normalisedDistance = Mathf.Sqrt((x - point.x) * (x - point.x) + (y - point.y) * (y - point.y)) / maxRealDistance;
            byte byteDist = (byte)(normalisedDistance * byte.MaxValue);
            if (byteDist < minScaledDistance) minScaledDistance = byteDist;
        }

        return (byte)(byte.MaxValue - minScaledDistance);
    }
}
