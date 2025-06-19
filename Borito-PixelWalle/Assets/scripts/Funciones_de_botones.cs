using UnityEngine;
using System.Collections.Generic;
using SFB;
using TMPro;
using System;
using System.IO;

public class Funciones_de_botones : MonoBehaviour
{
    public string[] archivoImportado;
    public List<GameObject> textos;
    List<Pixel> numeros;

    // Alterna la visibilidad del input
    public void Input()
    {
        if (textos.Count > 0)
        {
            textos[0].SetActive(!textos[0].activeSelf);
            Quita_Pon_numeros();
        }
    }

    // Alterna la visibilidad de la consola
    public void Consola()
    {
        if (textos.Count > 1)
            textos[1].SetActive(!textos[1].activeSelf);
    }

    // Importa un archivo y lo muestra en el input
    public void Importar()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Abrir archivo", "", "", false);
        if (paths.Length == 0) return;

        archivoImportado = paths;
        string archivo = string.Join("", archivoImportado);

        if (textos.Count > 0)
        {
            var inputField = textos[0].GetComponent<TMP_InputField>();
            inputField.text += "\n\n" + archivo;
        }
    }

    // Exporta el contenido del input a un archivo
    public void Exportar()
    {
        if (textos.Count == 0) return;

        string ruta = Application.persistentDataPath + "/Archivo_exportado.txt";
        string contenido = textos[0].GetComponent<TMP_InputField>().text;
        File.WriteAllText(ruta, contenido);
    }
    public void Write()
    {
        textos[0].transform.GetChild(1).gameObject.SetActive(false);
        textos[0].transform.GetChild(0).gameObject.SetActive(true);
    }
    public void Read()
    {
        textos[0].transform.GetChild(0).gameObject.SetActive(false);
        textos[0].transform.GetChild(1).gameObject.SetActive(true);
    }
    void Quita_Pon_numeros()
    {
        foreach (var pixel in numeros)
        {
            GameObject numero = pixel.gameObject.transform.GetChild(1).gameObject;
            numero.SetActive(!numero.gameObject.activeSelf);
        }
    }
    void Start()
    {
        numeros = new List<Pixel>();
        int mayor = Manager.lienzo.dimX > Manager.lienzo.dimY ? Manager.lienzo.dimX +1: Manager.lienzo.dimY+1;
        for (int i = 1; i < mayor+1; i++)
        {
            if(i<=Manager.lienzo.dimX)numeros.Add(Manager.lienzo.imagen_ampliada[0, i]);
            if(i<=Manager.lienzo.dimY)numeros.Add(Manager.lienzo.imagen_ampliada[i, 0]);
        }
    }
}
