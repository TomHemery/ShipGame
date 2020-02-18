using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAsteroid : MonoBehaviour
{

    private SpriteRenderer m_renderer;
    private Texture2D m_texture;
    private Sprite [] m_sprites;
    private PolygonCollider2D m_collider;

    private DamageAnimation damageAnimation;

    private readonly Color32 clear = new Color32(0, 0, 0, 0);

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
    public Color32 baseColour;
    public int mid1ShadingNeighbourhood = 2;
    public Color32 midColour1;
    public int mid2ShadingNeighbourhood = 2;
    public Color32 midColour2;
    public int mid3ShadingNeighbourhood = 2;
    public Color32 midColour3;
    public int highlightShadingNeighbourhood = 2;
    public Color32 highlightColour;
    public float shadingNoise = 0.5f;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_sprites = new Sprite[numFrames];
        GenerateAsteroid();
    }

    private void GenerateAsteroid() {
        //generate the texture
        m_texture = GenerateTexture();

        for (int i = 0; i < numFrames; i++) m_sprites[i] = Sprite.Create(m_texture, new Rect(i * width, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);

        m_renderer.sprite = m_sprites[0];

        damageAnimation = GetComponent<DamageAnimation>();
        damageAnimation.sprites = m_sprites;

        m_collider = gameObject.AddComponent<PolygonCollider2D>();
    }

    private Texture2D GenerateTexture() {
        //create the texture we will populate
        Texture2D result = new Texture2D(width * numFrames, height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        Color32 [] pixels = result.GetPixels32();

        for (int texLeft = 0; texLeft < width * numFrames; texLeft += width)
        {
            if (texLeft == 0)
            { //base case -> generate the first map of the asteroid 
                //create some points in the area of the texture
                Vector2[] whorleyNoisePoints = new Vector2[numWhorleyNoisePoints];

                //assign every pixel a grayscale colour based on distance from the nearest point 
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
                        SetPixel(x, y, new Color32(value, value, value, byte.MaxValue), pixels);
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        Color32 c = GetPixel(x, y, pixels);
                        byte normalizedValue = NormalizeByte(c.r, darkest, brightest);
                        SetPixel(x, y, new Color32(normalizedValue, normalizedValue, normalizedValue, 255), pixels);
                        if (normalizedValue > threshold)
                        {
                            SetPixel(x, y, baseColour, pixels);
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
                        if (CompareColours(GetPixel(tex_x, tex_y, pixels), clear))
                        {
                            for (int x = tex_x - destructionNeighbourhood; 
                                x <= (tex_x + destructionNeighbourhood >= result.width-1 ? result.width-1 : tex_x + destructionNeighbourhood); 
                                x++)
                            {
                                for (int y = tex_y - destructionNeighbourhood <= 0 ? 0 : tex_y - destructionNeighbourhood; 
                                    y <= (tex_y + destructionNeighbourhood >= height - 1 ? height - 1 : tex_y + destructionNeighbourhood); 
                                    y++)
                                {
                                    if (!CompareColours(GetPixel(x, y, pixels), clear) && Random.value < destructionNoise)
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
        result.SetPixels32(pixels);
        result.Apply();
        return result;
    }

    private void ShadeInTexture(Color32 [] pixels) {
        ShadePass(clear, baseColour, midColour1, pixels, mid1ShadingNeighbourhood);
        ShadePass(baseColour, midColour1, midColour2, pixels, mid2ShadingNeighbourhood);
        ShadePass(midColour1, midColour2, midColour3, pixels, mid3ShadingNeighbourhood);
        ShadePass(midColour2, midColour3, highlightColour, pixels, highlightShadingNeighbourhood);
    }

    private void ShadePass(Color32 outsideColour, Color32 targetColour, Color32 newColour, Color32[] pixels, int neighbourhood) {
        float texWidth = width * numFrames;
        for (int x = neighbourhood; x < texWidth - neighbourhood; x++)
        {
            for (int y = neighbourhood; y < height - neighbourhood; y++)
            {
                if (CompareColours(GetPixel(x, y, pixels), targetColour))
                { //for each filled in pixel
                    bool onEdge = false;
                    for (int x2 = x - neighbourhood <= 0 ? 0 : x - neighbourhood; x2 <= (x + neighbourhood > texWidth - 1 ? texWidth - 1 : x + neighbourhood); x2++)
                    {
                        for (int y2 = y - neighbourhood <= 0 ? 0 : y - neighbourhood; y2 <= (y + neighbourhood > height - 1 ? height - 1 : y + neighbourhood); y2++)
                        {
                            if (CompareColours(GetPixel(x2, y2, pixels), outsideColour))
                            {
                                onEdge = true;
                                break;
                            }
                        }
                    }
                    if (!onEdge)
                    {
                        SetPixel(x, y, newColour, pixels);
                    }
                }
            }
        }
    }

    private bool CompareColours(Color32 colourA, Color32 colourB){
        return
            colourA.r == colourB.r &&
            colourA.g == colourB.g &&
            colourA.b == colourB.b &&
            colourA.a == colourB.a
        ;
    }

    private void SetPixel(int x, int y, Color32 c,  Color32[] pixels) {
        pixels[x + y * width * numFrames] = c;
    }

    private Color32 GetPixel(int x, int y,  Color32[] pixels) {
        return pixels[x + y * width * numFrames];
    }

    private byte NormalizeByte(byte val, byte min, byte max) {
        float top = val - min;
        float bottom = max - min;
        byte result = (byte)(byte.MaxValue * top / bottom);
        return result;
    }

    private void CopyPrevFrame(int newFrameX,  Color32[] pixels) {
        for (int x = newFrameX; x < newFrameX + width; x++) {
            for (int y = 0; y < height; y++) {
                SetPixel(x, y, GetPixel(x - width, y,  pixels), pixels);
            }
        }
    }

    private byte DistanceToGrayscale(int x, int y, Vector2[] points) {
        float maxRealDistance = Mathf.Sqrt(width * width + height * height);
        byte minScaledDistance = byte.MaxValue;

        foreach (Vector2 point in points) {
            float normalisedDistance = Mathf.Sqrt((x - point.x) * (x - point.x) + (y - point.y) * (y - point.y)) / maxRealDistance;
            byte byteDist = (byte)(normalisedDistance * byte.MaxValue);
            if (byteDist < minScaledDistance) minScaledDistance = byteDist;
        }

        return (byte)(byte.MaxValue - minScaledDistance);
    }
}
