using System;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static List<Material> materials;
    public static Pincel pincel;
    public static Lienzo lienzo;

    public List<Material> Materials;
    public Canvas canvas;
    public GameObject pincelPrefab;
    public GameObject pixel;
    
    public enum color { Red, Blue, Green, Yellow, Orange, Purple, Black, White, Transparent }
    static List<(int, int)> direcciones = new List<(int,int)>{ (0, 1),(1,1),(1,0),(1,-1),(0,-1),(-1,-1),(-1,0),(-1,1) };

    public static void Spawn(int x, int y)
    {
        pincel.Pos_tablero = (x, y);
        for (int i = 0; i < pincel.pincels.Count; i++)
        {
            for (int j = 0; j < pincel.pincels[i].Count; j++)
            {
                if (i != 0 && j != 0)
                {
                    pincel.pincels[i][j].Pos_tablero = (x + i, y + j);
                }
            }
        }
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
        int aprox = 0;
        double ratio;
        (int, int) centro = (pincel.Pos_tablero.Item1 + (radius * dirY),
                            pincel.Pos_tablero.Item2 + (radius * dirX));
        if (centro.Item1 - radius < 0 || centro.Item2 - radius < 0
        || centro.Item1 + radius > lienzo.dimX || centro.Item2 + radius > lienzo.dimY)
        {
            Debug.LogError("No se pude pintar el circulo porque se sale de los limites de la pantalla");
            return;
        }
        if (!(dirX == 0 || dirY == 0))
        {
            ratio = Math.Sqrt(2) * (double)radius;
            aprox = 1;
        }
        else ratio = radius;
        Pixel[,] cuadrado = new Pixel[((int)Math.Abs(ratio) + aprox) * 2, ((int)Math.Abs(ratio) + aprox) * 2];
        (int, int) Posicion = (centro.Item1 - (cuadrado.GetLength(0) / 2), centro.Item2 - (cuadrado.GetLength(0) / 2));
        (int, int) Otro_Centro = (cuadrado.GetLength(0) / 2, cuadrado.GetLength(0) / 2);
        (int, int) Transformacion = (centro.Item1 - Otro_Centro.Item1, centro.Item2 - Otro_Centro.Item2);
        for (int i = 0; i < cuadrado.GetLength(0); i++)
        {
            for (int j = 0; j < cuadrado.GetLength(1); j++)
            {
                cuadrado[i, j] = lienzo[Posicion.Item1 + i, Posicion.Item2 + j];
            }
        }
        (int, int) Verificar = (Otro_Centro.Item1, 0);
        Posicion = Verificar;
        (int, int) Trance = Posicion;
        List<(int, int)> pintar = new List<(int, int)>();
        do
        {
            double menor = int.MaxValue;
            foreach (var item in direcciones)
            {
                double distancia = Math.Pow(Otro_Centro.Item1 - Posicion.Item1 + item.Item1, 2) +
                                    Math.Pow(Otro_Centro.Item2 - Posicion.Item2 + item.Item2, 2);
                distancia = Math.Abs(distancia - ratio);
                if (distancia < menor)
                {
                    menor = distancia;
                    Trance = (Posicion.Item1 + item.Item1, Posicion.Item2 + item.Item2);
                }
            }
            Posicion = Trance;
            (int, int) agregar = (Trance.Item1 + Transformacion.Item1,
                                Trance.Item2 + Transformacion.Item2);
            pintar.Add(agregar);
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
        if (posicion.Item1 < 0 || posicion.Item2 < 0 || centro.Item1 + (height - 1) > lienzo.dimX ||
         centro.Item2 - (width - 1) > lienzo.dimY)
        {
            Debug.LogError("No se pude pintar el rectangulo porque se sale de los limites de la pantalla");
            return;
        }
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
        lienzo = ScriptableObject.CreateInstance<Lienzo>();
        lienzo.Constructor(pixel, canvas);
        pincel = Instantiate(pincelPrefab).GetComponent<Pincel>();
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
    Pixel[,] imagen;
    public int dimX { get => imagen.GetLength(0); }
    public int dimY {get=> imagen.GetLength(1);}
    public Pixel this[int i, int j]
    {
        get => imagen[i, j];
    }
    public void Constructor(GameObject pixel, Canvas canvas)
    {
        this.pixel = pixel;this.canvas = canvas;
    }
    public void Crear_lienzo(int i, int j)
    {
        foreach (var pixel in imagen) Destroy(pixel.gameObject);
        imagen = new Pixel[i, j];
        Vector2 tamano_canvas = canvas.GetComponent<RectTransform>().sizeDelta;
        float tamano_pixel;

        if (tamano_canvas.x / tamano_canvas.y > i / j) tamano_pixel = tamano_canvas.x / i;
        else tamano_pixel = tamano_canvas.y / j;

        Vector3 pos_esquina = new Vector3(-tamano_canvas.x / 2 + tamano_pixel / 2, -tamano_canvas.y / 2 + tamano_pixel / 2);

        for (int x = 0; x < i; x++)
        {
            for (int y = 0; y < j; y++)
            {
                var pixelGameObject = Instantiate(pixel, pos_esquina + new Vector3(x * tamano_pixel, y * tamano_pixel, 0), Quaternion.identity, parent: canvas.transform);
                // Cambiar el tamaño del GameObject pixel (UI)
                RectTransform rt = pixelGameObject.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.sizeDelta = new Vector2(tamano_pixel, tamano_pixel);
                }

                imagen[x, y] = pixelGameObject.GetComponent<Pixel>();
                imagen[x, y].pos_tablero = (x, y);
            }
        }
    }

     
}
