using System;
using UnityEngine;
using TMPro;

public class Pixel : MonoBehaviour
{
    public Manager.color color;
    public static float tamano_pixel;
    public (int, int) pos_tablero;
    private int numerolinea;
    public int NumeroLinea
    {
        get => numerolinea;
        set
        {
            if (transform.childCount==2)
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();

            numerolinea = value;
        }
    }
    // public void Change_color(Color colors)
    // {
    //     // this.color = colors; 
    //     throw new NotImplementedException("Metodo no implementado");
    // }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.childCount == 2)
        {
            int pixelIndex = transform.GetSiblingIndex();
            transform.GetChild(0).transform.SetSiblingIndex(pixelIndex + 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
