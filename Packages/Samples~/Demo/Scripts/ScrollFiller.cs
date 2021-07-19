using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Data
{
    public Color color;
    public string text;
}

public class ScrollFiller : MonoBehaviour
{
    public int totalCount = -1;
    public GameObject go;
    public List<LoopScrollRect> ls;

    private List<Data> list = new List<Data>();

    void Start()
    {
        for (int i = 0; i < totalCount; i++)
        {
            list.Add(new Data()
            {
                color = Rainbow(i / 50f),
                text = i.ToString(),
            });
        }

        foreach (var l in ls)
        {
            l.Init(totalCount, go, list.ToArray());
        }
    }


    private static Color Rainbow(float progress)
    {
        progress = Mathf.Clamp01(progress);
        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;
        int i = (int) (progress * 6);
        float f = progress * 6.0f - i;
        float q = 1 - f;

        switch (i % 6)
        {
            case 0:
                r = 1;
                g = f;
                b = 0;
                break;
            case 1:
                r = q;
                g = 1;
                b = 0;
                break;
            case 2:
                r = 0;
                g = 1;
                b = f;
                break;
            case 3:
                r = 0;
                g = q;
                b = 1;
                break;
            case 4:
                r = f;
                g = 0;
                b = 1;
                break;
            case 5:
                r = 1;
                g = 0;
                b = q;
                break;
        }

        return new Color(r, g, b);
    }
}