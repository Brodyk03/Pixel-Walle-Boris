using UnityEngine;
using System.Collections.Generic;
using SFB;
using TMPro;
using System.Linq;
using System;
using System.IO;

public class Funciones_de_botones : MonoBehaviour
{

    public static string[] ArchivoImportado;
    public List<GameObject> Textos;
    public void Input()
    {
        if (!Textos[0].activeSelf) Textos[0].SetActive(true);
        else Textos[0].SetActive(false);
    }
    public void Consola()
    {
        if (!Textos[1].activeSelf) Textos[1].SetActive(true);
        else Textos[1].SetActive(false);
    }
    public void Importar()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Abrir archivo", "", "", false);
        ArchivoImportado = paths;
        string archivo= "";
        foreach (var linea in ArchivoImportado)
        {
            archivo=String.Concat(archivo,linea);
        }
        Textos[0].GetComponent<TMP_InputField>().text = Textos[0].GetComponent<TMP_InputField>().text
        + "/n/n" +archivo;
    }
    public void Exportar()
    {
        string ruta = Application.persistentDataPath + "/Archivo_exportado.txt";
        string contenido = Textos[0].GetComponent<TMP_InputField>().text;

        File.WriteAllText(ruta, contenido);
    }
}
