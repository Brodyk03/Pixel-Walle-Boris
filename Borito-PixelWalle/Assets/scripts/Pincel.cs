using System;
using System.Collections.Generic;
using Unity.VisualScripting;

// using System.Numerics;
using UnityEngine;

public class Pincel : MonoBehaviour
{
    Pixel pixel
    {
        get
        {
            if (Pos_tablero.Item1 < 0 || Pos_tablero.Item1 >= Manager.lienzo.dimY ||
                Pos_tablero.Item2 < 0 || Pos_tablero.Item2 >= Manager.lienzo.dimX)
                return null;
            else return Manager.lienzo[Pos_tablero.Item1, Pos_tablero.Item2];
        }
        
    }
    private (int, int) pos_tablero;
    private float tamano_pincel;
    private Material material;
    int resolution;
    private Manager.color color;
    public List<List<Pincel>> pincels;

    public float Tamano_pincel { get => Pixel.tamano_pixel; set => tamano_pincel = value; }
    public Manager.color Color
    {
        get => color;
        set
        {
            int i = 10;
            switch (value)
            {
                case Manager.color.Red:
                    i = 0;
                    break;
                case Manager.color.Blue:
                    i = 1;
                    break;
                case Manager.color.Green:
                    i = 2;
                    break;
                case Manager.color.Yellow:
                    i = 3;
                    break;
                case Manager.color.Orange:
                    i = 4;
                    break;
                case Manager.color.Purple:
                    i = 5;
                    break;
                case Manager.color.Black:
                    i = 6;
                    break;
                case Manager.color.White:
                    i = 7;
                    break;
                case Manager.color.Transparent:
                    i = 8;
                    break;


                default:
                    break;
            }

            var renderer = this.transform.GetChild(0).gameObject.GetComponent<Renderer>();
            if (renderer != null && Manager.materials != null && Manager.materials.Count > 0 && i < Manager.materials.Count)
            {
                renderer.material = Manager.materials[i];
                material = Manager.materials[i];
            }
            color = value;
            foreach (var lista in pincels)foreach (var pincel in lista)
                    if(pincel!=this)pincel.color = value;
        }
    }
    public (int, int) Pos_tablero
    {
        get => pos_tablero;
        set
        {
            pos_tablero = value;
            transform.SetParent(pixel.gameObject.transform);
            transform.localPosition = new Vector2(0f, 0f);
            transform.localScale = new Vector2(1f, 1f);
            int pixelIndex = pixel.transform.GetSiblingIndex();
            transform.SetSiblingIndex(pixelIndex + 1);
            this.transform.GetChild(0).gameObject.transform.SetSiblingIndex(pixelIndex + 2);
            for (int i = 0; i < pincels.Count; i++)
            {
                for (int j = 0; j < pincels[i].Count; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        pincels[i][j].Pos_tablero = (i + value.Item1, j + value.Item2);
                    }
                }
            }
            // transform.localPosition = Manager.lienzo[value.Item1, value.Item2].gameObject.transform.localPosition+new Vector3(0f,0f,0.1f);
        }
    }

    private void AD_resolucion(bool x)
    {
        if (x)
        {
            for (int i = 0; i < pincels.Count; i++)
            {
                pincels[i].Add(Instantiate(this.transform.parent).GetComponent<Pincel>());/*Manager.lienzo[Pos_tablero.Item1,Pos_tablero.Item2+pincels.Count]*/
                pincels[i][pincels.Count - 1].pos_tablero = (Pos_tablero.Item1, Pos_tablero.Item2 + pincels.Count);
                pincels[i][pincels.Count - 1].color = color;
            }
            pincels.Add(new List<Pincel>());
            for (int i = 0; i < pincels[0].Count; i++)
            {
                pincels[pincels.Count - 1].Add(Instantiate(this.transform.parent).GetComponent<Pincel>());
                pincels[pincels.Count - 1][i].pos_tablero = (Pos_tablero.Item1 + pincels.Count, Pos_tablero.Item2);
                pincels[pincels.Count - 1][i].color = color;
            }
        }
        else
        {
            for (int i = 0; i < pincels.Count; i++)
            {
                Destroy(pincels[i][pincels.Count - 1].gameObject);
                pincels[i].Remove(pincels[i][pincels.Count - 1]);
            }
             for (int i = 0; i < pincels[pincels.Count-1].Count; i++)
            {
                Destroy(pincels[pincels.Count-1][i].gameObject);
                pincels[i].Remove(pincels[pincels.Count-1][i]);
            }

        }
    }
    public void Cambiar_resolucion(int tamano)
    {
        int repeticiones;
        if (resolution < tamano)
        {
            repeticiones = tamano - resolution;
            for (int i = 0; i < repeticiones; i++) AD_resolucion(true);
        }
        else if (resolution > tamano)
        {
            repeticiones = resolution - tamano;
            for (int i = 0; i < repeticiones; i++) AD_resolucion(false);
        }
            resolution = tamano;
    }
    public void Cambiar_color(string color)
    {
        switch (color)
        {
            case "Red":
                this.Color = Manager.color.Red;
                break;
            case "Blue":
                this.Color = Manager.color.Blue;
                break;
            case "Green":
                this.Color = Manager.color.Green;
                break;
            case "Yellow":
                this.Color = Manager.color.Yellow;
                break;
            case "Orange":
                this.Color = Manager.color.Orange;
                break;
            case "Purple":
                this.Color = Manager.color.Purple;
                break;
            case "Black":
                this.Color = Manager.color.Black;
                break;
            case "White":
                this.Color = Manager.color.White;
                break;
            case "Transparent":
                this.Color = Manager.color.Transparent;
                break;
            default:
                throw new ArgumentException("Color no valido");
        }
        // this.color;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void pintar()
    {
        if (pixel != null)
        {
            if (this.color!=Manager.color.Transparent)
            {
                pixel.color = this.Color;
                pixel.gameObject.GetComponent<Renderer>().material = material;
            }
        }
    }
    public void Pintar()
    {
        foreach (var item in pincels)
        {
            foreach (var pincel in item)
            {
                pincel.pintar();
            }
        }
    }
    void Start()
    {
        pincels = new List<List<Pincel>>
        {
            new List<Pincel>() { this }
        };
        material = this.gameObject.GetComponent<Renderer>().material;
        resolution = 1;
        Pos_tablero = (0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
