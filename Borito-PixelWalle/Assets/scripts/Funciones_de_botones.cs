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

    // Alterna la visibilidad del input
    public void Input()
    {
        if (textos.Count > 0)
            textos[0].SetActive(!textos[0].activeSelf);
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
}
