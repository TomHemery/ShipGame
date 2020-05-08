using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ProceduralWreckage : MonoBehaviour
{

    private SpriteRenderer m_renderer;
    //private Sprite [] m_sprites;

    private static Color32[][] m_texture_colours;
    private static Texture2D[] m_textures = null;
    private static Sprite[][] m_sprites = null;
    private const int NUM_WRECKAGE_TEXTURES = 64;

    private static bool generatingTextures = false;
    private static bool generatedTextures = false;
    private bool assignedTexture = false;

    private DamageAnimation damageAnimation;

    private readonly byte clear = 0;

    public int pixelsPerUnit = 10;
    public int width = 64;
    public int height = 64;
    private int fullWidth;
    public int numFrames = 6;
    public int numBaseShapes = 20;
    public int destructionNeighbourhood = 1;
    public float destructionNoise = 0.5f;

    public Rect baseShapePointBounds;
    public int minShapeSize;
    public int maxShapeSize;

    public int numSplits;
    public int minWires;
    public int maxWires;
    public int maxWireLength;

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

    private System.Random rand;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();

        fullWidth = width * numFrames;

        if (!generatingTextures)
        {
            generatingTextures = true;
            GenerateWreckage();
        }
    }

    private void Start()
    {
        damageAnimation = GetComponent<DamageAnimation>();
        damageAnimation.enabled = false;
    }

    private void Update()
    {
        //textures are ready now and we haven't picked one yet
        if (!assignedTexture && generatedTextures)
        {
            //if the textures haven't been set from the colour arrays, then do it
            if (m_textures == null)
            {
                m_sprites = new Sprite[NUM_WRECKAGE_TEXTURES][];
                m_textures = new Texture2D[NUM_WRECKAGE_TEXTURES];
                for (int tex = 0; tex < NUM_WRECKAGE_TEXTURES; tex++)
                {
                    m_textures[tex] = new Texture2D(width * numFrames, height, TextureFormat.ARGB32, false)
                    {
                        filterMode = FilterMode.Point,
                    };
                    m_textures[tex].SetPixels32(m_texture_colours[tex]);
                    m_textures[tex].Apply();
                    m_sprites[tex] = new Sprite[numFrames];
                    for (int i = 0; i < numFrames; i++) m_sprites[tex][i] =
                        Sprite.Create(m_textures[tex], new Rect(i * width, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                }
            }

            Sprite[] chosenSprites = m_sprites[Random.Range(0, NUM_WRECKAGE_TEXTURES)];
            m_renderer.sprite = chosenSprites[0];
            damageAnimation.sprites = chosenSprites;
            damageAnimation.enabled = true;

            gameObject.AddComponent<PolygonCollider2D>();
            assignedTexture = true;
        }
    }

    private void GenerateWreckage()
    {
        rand = new System.Random(Random.Range(0, int.MaxValue));
        m_texture_colours = new Color32[NUM_WRECKAGE_TEXTURES][];
        //generate the texture
        Thread t = new Thread(new ThreadStart(GenerateTextures));
        t.Start();
    }

    private void GenerateTextures()
    {
        for (int num = 0; num < NUM_WRECKAGE_TEXTURES; num++)
        {
            byte[] pixels = new byte[width * numFrames * height];

            for (int texLeft = 0; texLeft < width * numFrames; texLeft += width)
            {
                if (texLeft == 0)
                {
                    //base case -> generate the first map of the asteroid 
                    //create some points in the area of the texture
                    Vector2[] shapeSeedPoints = new Vector2[numBaseShapes];
                    for (int i = 0; i < numBaseShapes; i++)
                        shapeSeedPoints[i] = new Vector2(
                            ((float)rand.NextDouble()).Map(0, 1, baseShapePointBounds.x, baseShapePointBounds.width),
                            ((float)rand.NextDouble()).Map(0, 1, baseShapePointBounds.y, baseShapePointBounds.height));

                    //spawn shapes at each point
                    foreach (Vector2 seedPoint in shapeSeedPoints) {
                        SpawnRandomShapeAt(seedPoint, pixels);
                    }

                    //carve away splits into the shapes
                    for (int i = 0; i < numSplits; i++) {
                        Vector2 start = new Vector2(rand.Next(0, width), rand.Next(0, height));
                        Vector2 stop = new Vector2(rand.Next((int)start.x, width), rand.Next((int)start.y, height));
                        SpawnSplit(start, stop, pixels);
                    }

                    int numWires = rand.Next(minWires, maxWires + 1);
                    //create wires 
                    for (int i = 0; i < numWires; i++) {
                        Vector2 start;
                        Vector2 step;
                        do
                        {
                            start = new Vector2(rand.Next(0, width), rand.Next(0, height));
                        } while (GetPixel((int)start.x, (int)start.y, pixels) == clear);
                        step.x = start.x >= width / 2 ? 1 : -1;
                        step.y = start.y >= height / 2 ? 1 : -1;
                        SpawnWire(start, step, pixels);
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
                                    x <= (tex_x + destructionNeighbourhood >= fullWidth - 1 ? fullWidth - 1 : tex_x + destructionNeighbourhood);
                                    x++)
                                {
                                    for (int y = tex_y - destructionNeighbourhood <= 0 ? 0 : tex_y - destructionNeighbourhood;
                                        y <= (tex_y + destructionNeighbourhood >= height - 1 ? height - 1 : tex_y + destructionNeighbourhood);
                                        y++)
                                    {
                                        if (GetPixel(x, y, pixels) != clear && rand.NextDouble() < destructionNoise)
                                        {
                                            SetPixel(x, y, clear, pixels);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ShadeInTexture(pixels);

            m_texture_colours[num] = new Color32[width * numFrames * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] != clear)
                {
                    m_texture_colours[num][i] = new Color32(
                        (byte)(baseColour.r * pixels[i]),
                        (byte)(baseColour.g * pixels[i]),
                        (byte)(baseColour.b * pixels[i]),
                        byte.MaxValue
                    );
                }
                else
                {
                    m_texture_colours[num][i] = Color.clear;
                }
            }
        }
        generatedTextures = true;
    }

    private void ShadeInTexture(byte[] pixels)
    {
        ShadePass(clear, shadowValue, midValue1, pixels, mid1ShadingNeighbourhood);
        ShadePass(shadowValue, midValue1, midValue2, pixels, mid2ShadingNeighbourhood);
        ShadePass(midValue1, midValue2, midValue3, pixels, mid3ShadingNeighbourhood);
        ShadePass(midValue2, midValue3, highlightValue, pixels, highlightShadingNeighbourhood);
    }

    private void ShadePass(byte outsideColour, byte targetColour, byte newColour, byte[] pixels, int neighbourhood)
    {
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
                        SetPixel(x, y, (byte)(newColour + rand.Next(0, shadingNoise)), pixels);
                    }
                }
            }
        }
    }

    private void SetPixel(int x, int y, byte c, byte[] pixels)
    {
        pixels[x + y * width * numFrames] = c;
    }

    private byte GetPixel(int x, int y, byte[] pixels)
    {
        return pixels[x + y * width * numFrames];
    }

    private void CopyPrevFrame(int newFrameX, byte[] pixels)
    {
        for (int x = newFrameX; x < newFrameX + width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetPixel(x, y, GetPixel(x - width, y, pixels), pixels);
            }
        }
    }

    //spawns split in texture between start and stop, start must be less than stop in x and y
    private void SpawnSplit(Vector2 start, Vector2 stop, byte[] pixels) {
        Vector2 pos = start;
        while (pos.x < stop.x || pos.y < stop.y) {
            if (pos.x > stop.x) pos.x = stop.x;
            if (pos.y > stop.y) pos.y = stop.y;
            SetPixel((int)pos.x, (int)pos.y, clear, pixels);
            if (rand.Next(0, 2) == 0)
            {
                pos.x += 1;
            }
            else {
                pos.y += 1;
            }
        }
    }

    private void SpawnWire(Vector2 start, Vector2 step, byte[] pixels)
    {
        Vector2 pos = start;
        int count = 0;
        while (pos.x >= 0 && pos.y < width && pos.y >= 0 && pos.y < height && count < maxWireLength)
        {
            SetPixel((int)pos.x, (int)pos.y, shadowValue, pixels);

            if (rand.Next(0, 2) == 0)
            {
                pos.x += step.y;
            }
            else
            {
                pos.y += step.x;
            }
            count++;
        }
    }

    private void SpawnRandomShapeAt(Vector2 pos, byte[] pixels) {
        int shapeSize = rand.Next(minShapeSize, maxShapeSize + 1);

        int shapeType = rand.Next(0, 2);
        switch (shapeType) {
            case 0:
                SpawnSquareAt(pos, 3 * shapeSize / 4, pixels);
                break;
            case 1:
                SpawnTriangleAt(pos, shapeSize, pixels);
                break;
        }        
    }

    private void SpawnSquareAt(Vector2 pos, int size, byte[] pixels) {
        
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                SetPixel((int)pos.x + x, (int)pos.y + y, shadowValue, pixels);
            }
        }
    }

    private void SpawnTriangleAt(Vector2 pos, int size, byte[] pixels)
    {
        int dir = rand.Next(0, 5);
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                switch (dir) {
                    case 0:
                        if(x < y) SetPixel((int)pos.x + x, (int)pos.y + y, shadowValue, pixels);
                        break;
                    case 1:
                        if(x > y) SetPixel((int)pos.x + x, (int)pos.y + y, shadowValue, pixels);
                        break;
                    case 2:
                        if(x + y < size / 2) SetPixel((int)pos.x + x, (int)pos.y + y, shadowValue, pixels);
                        break;
                    case 3:
                        if(x + y > size / 2) SetPixel((int)pos.x + x, (int)pos.y + y, shadowValue, pixels);
                        break;
                }
            }
        }
    }
}
