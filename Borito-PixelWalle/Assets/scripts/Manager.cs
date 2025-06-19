using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static List<Material> materials;
    public static Pincel pincel;
    public static Lienzo lienzo;

    public List<Material> Materials;
    Canvas canvas;
    public GameObject pincelPrefab;
    public GameObject pixel;
    public GameObject Linea;
    
    public enum color { Red, Blue, Green, Yellow, Orange, Purple, Black, White, Transparent }
    static List<(int, int)> direcciones = new List<(int,int)>{ (0, 1),(1,1),(1,0),(1,-1),(0,-1),(-1,-1),(-1,0),(-1,1) };

    public static void Spawn(int x, int y)
    {
        pincel.Pos_tablero = (x, y);
        // for (int i = 0; i < pincel.pincels.Count; i++)
        // {
        //     for (int j = 0; j < pincel.pincels[i].Count; j++)
        //     {
        //         if (i != 0 && j != 0)
        //         {
        //             pincel.pincels[i][j].Pos_tablero = (x + i, y + j);
        //         }
        //     }
        // }
    }
    public static void Color(string color){pincel.Cambiar_color(color);}
    public static void Size(int k) {pincel.Cambiar_resolucion(k);}
    public static void DrawLine(int dirX, int dirY, int distance)
    {
        for (int i = 0; i < distance; i++)
        {
            int y = pincel.Pos_tablero.Item1 + dirY * i;
            int x = pincel.Pos_tablero.Item2 + dirX * i;
            if (x >= 0 && x < lienzo.dimX && y >= 0 && y < lienzo.dimY)
            {
                pincel.Pos_tablero = (y, x);
                pincel.Pintar();
            }
        }
    }
    public static void DrawCircle(int dirX, int dirY, int radius)
    {
        double ratio;
        (int, int) centro = (pincel.Pos_tablero.Item1 + (radius * dirY),
                            pincel.Pos_tablero.Item2 + (radius * dirX));
        // if (centro.Item1 - radius < 0 || centro.Item2 - radius < 0
        // || centro.Item1 + radius > lienzo.dimX || centro.Item2 + radius > lienzo.dimY)
        // {
        //     Debug.LogError("No se pude pintar el circulo porque se sale de los limites de la pantalla");
        //     return;
        // }
        if (!(dirX == 0 || dirY == 0))ratio = Math.Sqrt(2) * (double)radius;
        else ratio = radius;

        (int, int) Posicion = pincel.Pos_tablero;//(centro.Item1, centro.Item2-Convert.ToInt32(ratio));
        (int, int) Verificar = Posicion;
        (int, int) Trance = Posicion;
        List<(int, int)> pintar = new List<(int, int)>();
        List<(int, int)> direccion_decision;

        int i;
        for ( i = 0; i < direcciones.Count && !(direcciones[i].Item1 == dirY)
                && !(direcciones[i].Item2 == dirX); i++) ;
        (int, int) direccion = direcciones[i<2?i+6:i-2]; // Direccion inicial
        int numeroD;

        do
        {
            numeroD = direcciones.IndexOf(direccion);
            direccion_decision = new List<(int, int)>() { direcciones[numeroD == 0 ? 7 : numeroD - 1], direcciones[numeroD], direcciones[numeroD == 7 ? 0 : numeroD + 1] };
            double menor = int.MaxValue;
            foreach (var item in direccion_decision)
            {
                double distancia = Math.Pow(centro.Item1 - Posicion.Item1 + item.Item1, 2) +
                        Math.Pow(centro.Item2 - Posicion.Item2 + item.Item2, 2);
                distancia = Math.Abs(distancia - Math.Pow(ratio, 2));

                if (distancia < menor)
                {
                    direccion = item;
                    menor = distancia;
                    Trance = (Posicion.Item1 + item.Item1, Posicion.Item2 + item.Item2);
                }
            }

            Posicion = Trance;
            pintar.Add(Trance);

        } while (Verificar == Posicion);
        foreach (var item in pintar)
        {
            pincel.Pos_tablero = item;
            pincel.Pintar();
        }
        pincel.Pos_tablero = centro;
    }
    public static void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {
        (int, int) centro = (pincel.Pos_tablero.Item1 + dirX * distance, pincel.Pos_tablero.Item1 + dirX * distance);
        (int, int) posicion = (centro.Item1 - (height - 1), centro.Item2 - (width - 1));
        (int, int) verificar = posicion;
        byte direccion = 0;
        int x = 0;
        int y = 0;
        // if (posicion.Item1 < 0 || posicion.Item2 < 0 || centro.Item1 + (height - 1) > lienzo.dimX ||
        //  centro.Item2 - (width - 1) > lienzo.dimY)
        // {
        //     Debug.LogError("No se pude pintar el rectangulo porque se sale de los limites de la pantalla");
        //     return;
        // }
        do
        {
            pincel.Pos_tablero = (posicion.Item1 + direcciones[direccion * 2].Item1,
                                posicion.Item2 + direcciones[direccion * 2].Item2);
            posicion = pincel.Pos_tablero;
            pincel.Pintar();
            if (direccion % 2 == 0)
            {
                x++;
                if (x % (width * 2 - 1) == 0) direccion += 1;
            }
            else
            {
                y++;
                if (y % (height * 2 - 1) == 0) direccion += 1;
            }

        } while (verificar == posicion);
        pincel.Pos_tablero = centro;
    }
    public static void Fill()
    {
        int tamano_pincel = pincel.pincels.Count;
        pincel.Cambiar_resolucion(1);
        (int, int) pos = pincel.Pos_tablero;

        bool[,] mask = new bool[lienzo.dimX, lienzo.dimY];
        for (int i = 0; i < mask.GetLength(0); i++)
        {
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                mask[i, j] = false;
            }
        }
        Fill(mask, lienzo[pos.Item1, pos.Item2].color, pos);
        pincel.Cambiar_resolucion(tamano_pincel);

    }
    private static void Fill(bool[,] mask, color Color,(int, int) pos)
    {
        pincel.Pos_tablero = pos;
        pincel.Pintar();
        mask[pos.Item1, pos.Item2] = true;
        for (int i = 0; i < direcciones.Count; i++)
        {
            pos = (pos.Item1 + direcciones[i].Item1, pos.Item2 + direcciones[i].Item2);
            if (!(pos.Item1 < 0 || pos.Item1 > mask.GetLength(0) || pos.Item2 < 0
            || pos.Item2 > mask.GetLength(1) || mask[pos.Item1, pos.Item2]
            || Color == lienzo[pos.Item1, pos.Item2].color))Fill(mask,Color, pos);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        materials = Materials;
        canvas = GetComponent<Canvas>();
        if (canvas is null)
        {
            Debug.LogError("No se ha encontrado el componente Canvas en el GameObject Manager.");
            return;
        }
        lienzo = ScriptableObject.CreateInstance<Lienzo>();
        lienzo.Constructor(pixel, canvas, Linea);
        lienzo.Crear_lienzo(16, 9);
        pincel = Instantiate(pincelPrefab,canvas.transform).GetComponent<Pincel>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
public class Lienzo : ScriptableObject
{
    Canvas canvas;
    GameObject pixel;
    GameObject linea;
    public Pixel[,] imagen_ampliada;
    Pixel[,] imagen;
    public int dimX { get => imagen.GetLength(1); }
    public int dimY {get=> imagen.GetLength(0);}
    public Vector2 pixelScale;
    public Pixel this[int i, int j]
    {
        get => imagen[i, j];
    }
    public void Constructor(GameObject pixel, Canvas canvas, GameObject linea)
    {
        this.pixel = pixel;this.canvas = canvas;this.linea = linea;
    }
    public void Crear_lienzo(int x, int y)
    {
        int i = y+1;int j = x+1;
        if (!(imagen is null))
        {
            foreach (var pixel in imagen) Destroy(pixel.gameObject);
        }
        imagen = new Pixel[y, x];
        imagen_ampliada = new Pixel[i, j];

        Vector2 tamano_canvas;
        CanvasScaler canvascaler = canvas.GetComponent<CanvasScaler>();
        if (canvascaler != null && canvascaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            tamano_canvas = canvascaler.referenceResolution;
        else
            return;
        float tamano_pixel;

        if (tamano_canvas.x / tamano_canvas.y > j / i) tamano_pixel = tamano_canvas.x / j;
        else tamano_pixel = tamano_canvas.y / i;
        Pixel.tamano_pixel = tamano_pixel;

        pixelScale = new Vector2(tamano_pixel,tamano_pixel);

        Vector3 pos_esquina = new Vector3(  (-j+1)*tamano_pixel / 2,   (i-1)*tamano_pixel / 2);

        for (int a = 0; a < i; a++)
        {
            for (int b = 0; b < j; b++)
            {
                if (a == 0 || b == 0)
                {
                    if (!(a == 0 && b == 0))
                    {
                        GameObject pixelGameObject = Instantiate(linea, canvas.transform);
                        pixelGameObject.transform.localPosition = pos_esquina + new Vector3(b * tamano_pixel, -a * tamano_pixel, 0);
                        pixelGameObject.transform.localScale = pixelScale;

                        imagen_ampliada[a, b] = pixelGameObject.GetComponent<Pixel>();
                        imagen_ampliada[a, b].NumeroLinea = a == 0 ? b - 1 : a - 1;
                    }
                }
                    else
                    {
                        GameObject pixelGameObject = Instantiate(pixel, canvas.transform);
                        pixelGameObject.transform.localPosition = pos_esquina + new Vector3(b * tamano_pixel, -a * tamano_pixel, 0);
                        pixelGameObject.transform.localScale = pixelScale;

                        imagen_ampliada[a, b] = pixelGameObject.GetComponent<Pixel>();
                        imagen[a - 1, b - 1] = imagen_ampliada[a, b];
                        imagen[a - 1, b - 1].pos_tablero = (a - 1, b - 1);
                    }
            }
        }
    }

     
}
