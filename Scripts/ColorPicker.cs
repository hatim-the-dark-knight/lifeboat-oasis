using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickers : MonoBehaviour
{
    public Image image;

    private float red = 1f;
    private float green = 1f;
    private float blue = 1f;
    private float alpha = 1f;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public Color current_color;

    void Start() 
    {
        Color skinColor;
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("SkinColor"), out skinColor);
        red = skinColor.r;
        green = skinColor.g;
        blue = skinColor.b;
        redSlider.value = red;
        greenSlider.value = green;
        blueSlider.value = blue;
        SetColor();
        GetColor();
    }

    public void SetRed(float passed_red)
    {
        red = passed_red;
        SetColor();
    }

    public void SetGreen(float passed_green)
    {
        green = passed_green;
        SetColor();
    }

    public void SetBlue(float passed_blue)
    {
        blue = passed_blue;
        SetColor();
    }

    private void SetColor()
    {
        current_color = new Color(red, green, blue, alpha);
        image.color = current_color;
    }

    public Color GetColor()
    {
        return current_color;
    }
    public string GetColorString()
    {
        return current_color.ToString();
    }
}
