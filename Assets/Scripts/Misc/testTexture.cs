using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTexture : MonoBehaviour
{
    Texture2D _newTexture;
    float _pixelsPerUnit;
    [SerializeField] Transform parColors;
    Color _colUR, _colUL, _colLR, _colLL;

    SpriteRenderer _spriteRenderer;
    Vector2 _spriteSize;


    private void Awake()
    {
        _colUL = parColors.GetChild(0).GetComponent<SpriteRenderer>().color;
        _colUR = parColors.GetChild(1).GetComponent<SpriteRenderer>().color;
        _colLL = parColors.GetChild(2).GetComponent<SpriteRenderer>().color;
        _colLR = parColors.GetChild(3).GetComponent<SpriteRenderer>().color;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        Texture2D texture2D = _spriteRenderer.sprite.texture;
        _newTexture = new Texture2D(texture2D.width, texture2D.height);
        _newTexture.SetPixels(texture2D.GetPixels());

        for (int i = 0; i < 256; i++)
        {
            for (int j = 0; j < 256; j++)
            {
                Color horUp = Color.Lerp(_colUL, _colUR, (float)i / (float)256f);
                Color horDown = Color.Lerp(_colLL, _colLR, (float)i / (float)256f);

                Color finalCol = Color.Lerp(horDown, horUp, (float)j / (float)256f);
                _newTexture.SetPixel(i, j, finalCol);
            }
        }

        _newTexture.Apply();

        _pixelsPerUnit = _spriteRenderer.sprite.pixelsPerUnit;
        Sprite sprajt = Sprite.Create(_newTexture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), _pixelsPerUnit);
        _spriteRenderer.sprite = sprajt;
        _spriteSize = new Vector2(_spriteRenderer.sprite.rect.width / 2, _spriteRenderer.sprite.rect.height / 2);
    }
}
