using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralAsteroid : MonoBehaviour
{

    private SpriteRenderer m_renderer;
    private Texture2D m_texture;
    private Sprite m_sprite;

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
    public Color32 midColour1;
    public Color32 midColour2;
    public Color32 highlightColour;
    public int shadingNeigbourhood = 1;

    public bool refresh = false;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        GenerateTexture();
    }

    private void Update()
    {
        if (refresh) {
            GenerateTexture();
            refresh = false;
        }
    }

    private void GenerateTexture() {
        //create the texture we will populate
        m_texture = new Texture2D(width * numFrames, height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        Color32[] pixels = m_texture.GetPixels32();

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
                                x <= (tex_x + destructionNeighbourhood >= m_texture.width-1 ? m_texture.width-1 : tex_x + destructionNeighbourhood); 
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

        m_texture.SetPixels32(pixels);
        m_texture.Apply();

        ShadeTexture();

        m_sprite = Sprite.Create(m_texture, new Rect(0, 0, width * numFrames, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        m_renderer.sprite = m_sprite;
    }

    private void ShadeTexture() {
        Color32[] pixels = m_texture.GetPixels32();
        //pass 1 
        for (int x = shadingNeigbourhood; x < m_texture.width - shadingNeigbourhood; x++) {
            for (int y = shadingNeigbourhood; y < m_texture.height - shadingNeigbourhood; y++) {
                bool onEdgeOfColourBand = false;
                if (!CompareColours(GetPixel(x, y, pixels), Color.clear))
                { //for each filled in pixel
                    for (int x2 = x - shadingNeigbourhood; x2 <= x + shadingNeigbourhood; x2++)
                    {
                        for (int y2 = y - shadingNeigbourhood; y2 <= y + shadingNeigbourhood; y2++)
                        {
                            if (CompareColours(GetPixel(x2, y2, pixels), Color.clear))
                            {
                                onEdgeOfColourBand = true;
                                break;
                            }
                        }
                    }
                    if (!onEdgeOfColourBand)
                    {
                        SetPixel(x, y, midColour1, pixels);
                    }
                }
            }
        }
        //pass 2
        for (int x = shadingNeigbourhood; x < m_texture.width - shadingNeigbourhood; x++)
        {
            for (int y = shadingNeigbourhood; y < m_texture.height - shadingNeigbourhood; y++)
            {
                if (CompareColours(GetPixel(x, y, pixels), midColour1))
                { //for each filled in pixel
                    bool onEdge = false;
                    for (int x2 = x - shadingNeigbourhood <= 0 ? 0 : x - shadingNeigbourhood; x2 <= (x + shadingNeigbourhood > m_texture.width - 1 ? m_texture.width - 1 : x + shadingNeigbourhood); x2++)
                    {
                        for (int y2 = y - shadingNeigbourhood <= 0 ? 0 : y - shadingNeigbourhood; y2 <= (y + shadingNeigbourhood > m_texture.height - 1 ? m_texture.height - 1 : y + shadingNeigbourhood); y2++)
                        {
                            if (CompareColours(GetPixel(x2, y2, pixels), baseColour))
                            {
                                onEdge = true;
                                break;
                            }
                        }
                    }
                    if (!onEdge)
                    {
                        SetPixel(x, y, midColour2, pixels);
                    }
                }
            }
        }
        //pass 3
        for (int x = shadingNeigbourhood; x < m_texture.width - shadingNeigbourhood; x++)
        {
            for (int y = shadingNeigbourhood; y < m_texture.height - shadingNeigbourhood; y++)
            {
                if (CompareColours(GetPixel(x, y, pixels), midColour2))
                { //for each filled in pixel
                    bool onEdge = false;
                    for (int x2 = x - shadingNeigbourhood <= 0 ? 0 : x - shadingNeigbourhood; x2 <= (x + shadingNeigbourhood > m_texture.width - 1 ? m_texture.width - 1 : x + shadingNeigbourhood); x2++)
                    {
                        for (int y2 = y - shadingNeigbourhood <= 0 ? 0 : y - shadingNeigbourhood; y2 <= (y + shadingNeigbourhood > m_texture.height - 1 ? m_texture.height - 1 : y + shadingNeigbourhood); y2++)
                        {
                            if (CompareColours(GetPixel(x2, y2, pixels), midColour1))
                            {
                                onEdge = true;
                                break;
                            }
                        }
                    }
                    if (!onEdge)
                    {
                        SetPixel(x, y, highlightColour, pixels);
                    }
                }
            }
        }
        m_texture.SetPixels32(pixels);
        m_texture.Apply();
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
        pixels[x + y * m_texture.width] = c;
    }

    private Color32 GetPixel(int x, int y,  Color32[] pixels) {
        return pixels[x + y * m_texture.width];
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
