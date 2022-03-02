using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderColor : MonoBehaviour
{
    public Color[] colors = new Color[] { Color.red, Color.yellow, Color.green, Color.red };
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.fillRect.transform.GetComponent<Image>().color = Color.green;
        Debug.Log(slider.name);
    }
    void Update()
    {
        float val = slider.value;
        //val *= (colors.Length - 1);
        int startIndex = Mathf.FloorToInt(val);

        Color color = colors[0];
        Debug.Log(startIndex);
        if (startIndex >= 0)
        {
            if (startIndex + 1 < colors.Length)
            {
                float factor = (val - startIndex);
                color = Color.Lerp(colors[startIndex], colors[startIndex + 1], factor);
            }
            else if (startIndex < colors.Length)
            {
                color = colors[startIndex];
            }
            else color = colors[colors.Length - 1];
        }
        color.a = slider.fillRect.transform.GetComponent<Image>().color.a;
        slider.fillRect.transform.GetComponent<Image>().color = color;
    }

}
