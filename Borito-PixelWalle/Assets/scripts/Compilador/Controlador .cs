using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.UIElements;


public class Controlador : MonoBehaviour
{
    public InputField Programa;
    public InputField ProgramCopia;
    public Text ListaTokens;
    public Text ListaErros;

    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    /// 


    void Awake()
    {
        TALexico.initALexico();
    }
    // Start is called before the first frame update
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }
    public void Start()
    {

    }


    public void OnCompilar()
    {
        TALexico ALexico = new TALexico(Programa.text);
   
        if (ListaErros != null)
        {
             ALexico.MostrarErros(ListaErros);
        }
         if (ListaTokens != null)
        {
            ALexico.MostrarTodo(ListaTokens);
        }
        if (ProgramCopia != null)
        {
            ProgramCopia.readOnly = true;
            ALexico.MostrarPrograma(ProgramCopia);
        }
    
    }

}
public class TError
{
    public String texto;
    public int poslinea;
    public int posCaracter;
    public TError(String atxt, int alinea, int apos = -1)
    {
        texto = atxt;
        poslinea = alinea;
        posCaracter = apos;
    }


}
public class TToken
{

    public enum TtipoToken
    {
        token_op, token_opSoma, token_opProducto
            , token_opPotencia, token_opAsignacion, token_opAnd
            , token_opOr, token_opComparador
            , token_Goto, token_Comandos, token_FuncionNombre, token_StringColor
            , token_literal, token_id, token_none
    }
    public String simbolo;
    public TtipoToken tipoToken;
    public TToken(String atxt, TtipoToken atk)
    {
        simbolo = atxt;
        tipoToken = atk;
    }
    public TToken(char atxt, TtipoToken atk)
    {
        simbolo = atxt.ToString();
        tipoToken = atk;
    }

}

public class TLineaVisual
{
    public List<String> Palabras;
    public List<TToken> Tokens;
    public bool Error;
    public TLineaVisual(List<String> aPalabras, List<TToken> aTokens, bool aError)
    {
        Palabras = aPalabras;
        Tokens = aTokens;
        Error = aError;
    }
}

public class TALexico
{

    public static Dictionary<char, TToken.TtipoToken> OperadoresSimples;
    public static Dictionary<String, TToken.TtipoToken> OperadoresCompostos;
    public static Dictionary<String, TToken.TtipoToken> PalabrasClaves;
    public static Dictionary<String, Color> Colores;

    public static List<TError> LErrores; // lista 
    public static List<List<TToken>> LLineasDeToKens;
    public static List<TLineaVisual> LineasVisualesPrograma;
    static HashSet<char> SimboloMultiple;

    public static bool IsInicioDeOperadorComposto(char op)
    {
        return OperadoresCompostos.Any(par => par.Key[0] == op);
    }

    public static bool IsInicioDeOperador(char op)
    {
        return (OperadoresSimples.ContainsKey(op) || IsInicioDeOperadorComposto(op));
    }

    public static bool Esletra(char aCaracter)
    {
        return (char.IsLetter(aCaracter));
    }

    public static bool EsNumero(char aCaracter)
    {
        return (char.IsDigit(aCaracter));
    }

    public static void initALexico()
    {
        SimboloMultiple = new HashSet<char>()
            { '*', '<', '>' };

        OperadoresSimples = new Dictionary<char, TToken.TtipoToken>()
            {
                { '[', TToken.TtipoToken.token_op },
                { ']', TToken.TtipoToken.token_op },
                { '(', TToken.TtipoToken.token_op },
                { ')', TToken.TtipoToken.token_op },
                { ',', TToken.TtipoToken.token_op },
                { '+', TToken.TtipoToken.token_opSoma },
                { '-', TToken.TtipoToken.token_opSoma },
                { '*', TToken.TtipoToken.token_opProducto },
                { '/', TToken.TtipoToken.token_opProducto },
                { '%', TToken.TtipoToken.token_opProducto },
                { '<', TToken.TtipoToken.token_opComparador },
                { '>', TToken.TtipoToken.token_opComparador }
            };

        OperadoresCompostos = new Dictionary<String, TToken.TtipoToken>()
            {
                { "**", TToken.TtipoToken.token_opPotencia },
                { "<-", TToken.TtipoToken.token_opAsignacion },
                { "&&", TToken.TtipoToken.token_opAnd },
                { "||", TToken.TtipoToken.token_opOr },
                { "==", TToken.TtipoToken.token_opComparador },
                { ">=", TToken.TtipoToken.token_opComparador },
                { "<=", TToken.TtipoToken.token_opComparador }
            };
        PalabrasClaves = new Dictionary<String, TToken.TtipoToken>()
            {
                { "GoTo", TToken.TtipoToken.token_Goto },
                { "Goto", TToken.TtipoToken.token_Goto },
                { "GoTO", TToken.TtipoToken.token_Goto },
                { "Spawn", TToken.TtipoToken.token_Comandos },
                { "Color", TToken.TtipoToken.token_Comandos },
                { "Size", TToken.TtipoToken.token_Comandos },
                { "DrawLine", TToken.TtipoToken.token_Comandos },
                { "DrawCircle", TToken.TtipoToken.token_Comandos },
                { "DrawRectangle", TToken.TtipoToken.token_Comandos },
                { "Fill", TToken.TtipoToken.token_Comandos },
                { "GetActualX", TToken.TtipoToken.token_FuncionNombre },
                { "GetActualY", TToken.TtipoToken.token_FuncionNombre },
                { "GetCanvasSize", TToken.TtipoToken.token_FuncionNombre },
                { "GetColorCount", TToken.TtipoToken.token_FuncionNombre },
                { "IsBrushColor", TToken.TtipoToken.token_FuncionNombre },
                { "IsBrushSize", TToken.TtipoToken.token_FuncionNombre },
                { "IsCanvasColor", TToken.TtipoToken.token_FuncionNombre },
                { "IsColor", TToken.TtipoToken.token_FuncionNombre }

            };

        Colores = new Dictionary<String, Color>()
            {
                { "\"Blue\"", Color.blue },
                { "\"Red\"", Color. red },
                { "\"Green\"", Color.green  },
                { "\"Yellow\"", Color.yellow  },
                { "\"Orange\"", new Color(255,165,0) },
                { "\"Purple\"", new Color(160,32,240) },
                { "\"White\"", Color. white },
                { "\"Transparent\"", Color. clear }
            };

    }


    public TALexico(String texto)
    {

        LErrores = new List<TError>();
        LLineasDeToKens = new List<List<TToken>>();
        LineasVisualesPrograma = new List<TLineaVisual>();


        string[] codigo = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries);  // Divide el texto en linhas y lo deja en un array
        List<String> lineasDeCodigo = codigo.ToList();
        int linea = 0;
        while (linea < lineasDeCodigo.Count)
        {
            String s = lineasDeCodigo.ElementAt(linea);
            s = s.Trim();// de cada linea elimina los espacios en blanco al inicio y el final del string

            if (String.IsNullOrEmpty(s))
            {
                lineasDeCodigo.RemoveAt(linea); //si la linea esta vacia se elimina
            }
            else
            {
                List<TToken> lineaTokens = new List<TToken>();
                //**** eliminar espacios  s antes y despues de las comillas
                String cadeiaConComillas = s;
                String lineaSinEspaciosEntreComillas = "";
                int posComillas = -1;
                while ((posComillas = cadeiaConComillas.IndexOf('\"')) > -1)
                {
                    lineaSinEspaciosEntreComillas = lineaSinEspaciosEntreComillas + cadeiaConComillas.Substring(0, posComillas).Trim() + "\"";
                    cadeiaConComillas = cadeiaConComillas.Substring(posComillas + 1).Trim();
                }
                s = lineaSinEspaciosEntreComillas + cadeiaConComillas;
                //**************************************************************

                //**** separar las palabras por espacios 
                string[] Palabras = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                List<String> listaDePalabrasEspacios = Palabras.ToList();
                //**************************************************************

                //**** analizar dentro de cada palabra si existe operador y separar en diferentes subcadenas 
                List<String> listaDePalabras = new List<String>();

                String operador = "";
                for (int posPalabra = 0; posPalabra < listaDePalabrasEspacios.Count; posPalabra++)
                {
                    String Palabra = listaDePalabrasEspacios.ElementAt(posPalabra);
                    int posLetra = 0;
                    int estado = 0;
                    String textoAnterior, textoSinUltimaLetra;
                    String textoSinOperador = "";
                    while (posLetra < Palabra.Length) // cada letra de la palabra
                    {
                        char caracter = Palabra[posLetra];
                        textoAnterior = Palabra.Substring(0, posLetra + 1);
                        textoSinUltimaLetra = Palabra.Substring(0, posLetra);
                        switch (estado)
                        {
                            case 0:
                                if (SimboloMultiple.Contains(caracter))
                                {
                                    operador = caracter.ToString();
                                    estado = 1;
                                }
                                else if (OperadoresSimples.ContainsKey(caracter))
                                {  //identificado operador simple;
                                   //guardar la palabra hasta el momentoen la nueva lista e despues el operador
                                   //dejar en palabra el resto sin analizar

                                    if (!(String.IsNullOrEmpty(textoSinUltimaLetra) || (String.IsNullOrWhiteSpace(textoSinUltimaLetra))))
                                    {
                                        listaDePalabras.Add(textoSinUltimaLetra);
                                    }
                                    listaDePalabras.Add(caracter.ToString());
                                    Palabra = Palabra.Substring(posLetra + 1);
                                    posLetra = -1;


                                }
                                else if (IsInicioDeOperadorComposto(caracter))
                                {
                                    operador = caracter.ToString();
                                    estado = 3;
                                }
                                break;
                            case 1:
                                operador = operador + caracter.ToString();
                                textoSinOperador = Palabra.Substring(0, posLetra - operador.Length + 1);
                                if (OperadoresCompostos.ContainsKey(operador))
                                {   //identificado operador double;
                                    //palabra sin las 2 ultimasletras
                                    if (!(String.IsNullOrEmpty(textoSinOperador) || (String.IsNullOrWhiteSpace(textoSinOperador))))
                                    {
                                        listaDePalabras.Add(textoSinOperador);
                                    }
                                    listaDePalabras.Add(operador);
                                    Palabra = Palabra.Substring(posLetra + 1);
                                    posLetra = -1;
                                }
                                else
                                { //identificado operador simple de inicio comun;
                                  //palabra sin las 2 ultimasletras
                                    if (!(String.IsNullOrEmpty(textoSinOperador) || (String.IsNullOrWhiteSpace(textoSinOperador))))
                                    {
                                        listaDePalabras.Add(textoSinOperador);
                                    }
                                    listaDePalabras.Add(Palabra[posLetra - 1].ToString()); //letra anterior
                                    Palabra = Palabra.Substring(posLetra);
                                    posLetra = 0;
                                }

                                estado = 0;
                                break;

                            case 3: //operador composto unico
                                operador = operador + caracter.ToString();
                                textoSinOperador = Palabra.Substring(0, posLetra - operador.Length + 1);
                                if (OperadoresCompostos.ContainsKey(operador))
                                {   //identificado operador double unico;
                                    //palabra sin las 2 ultimasletras
                                    if (!(String.IsNullOrEmpty(textoSinOperador) || (String.IsNullOrWhiteSpace(textoSinOperador))))
                                    {
                                        listaDePalabras.Add(textoSinOperador);
                                    }
                                    listaDePalabras.Add(operador);
                                    Palabra = Palabra.Substring(posLetra + 1);
                                    posLetra = -1;
                                }
                                estado = 0;
                                break;
                        }
                        posLetra++;
                    }
                    if (!(String.IsNullOrEmpty(Palabra) || (String.IsNullOrWhiteSpace(Palabra))))
                    {
                        listaDePalabras.Add(Palabra);
                    }
                }
                //**************************************************************


                //analizar con las palabras finales para crear tokens
                bool Error = false;
                for (int posPalabra = 0; (posPalabra < listaDePalabras.Count && !Error); posPalabra++)
                {
                    String Palabra = listaDePalabras.ElementAt(posPalabra).Trim();
                    TToken.TtipoToken atoken = TToken.TtipoToken.token_none;
                    TToken tokenIdentificado = null;

                    if (OperadoresCompostos.TryGetValue(Palabra, out atoken) //identificado operador double;
                        || ((Palabra.Length == 1) && (OperadoresSimples.TryGetValue(Palabra[0], out atoken))) //identificado operador simple 
                        || PalabrasClaves.TryGetValue(Palabra, out atoken) // palabra clave
                        )
                    {
                        tokenIdentificado = new TToken(Palabra, atoken);
                    }
                    else
                    { //identificar StringColor, literal , otexto
                        int posLetra = 0;
                        int estado = 0;


                        for (posLetra = 0; (posLetra < Palabra.Length) && !Error; posLetra++)
                        {

                            char caracter = Palabra[posLetra];
                            String textoAnterior = Palabra.Substring(0, posLetra + 1);
                            switch (estado)
                            {
                                case 0://inicio da palabra
                                    if (Esletra(caracter))
                                    {
                                        estado = 1; //operador texto  (comando , funcion, variable)

                                    }
                                    else if (caracter == '\"')
                                    {
                                        estado = 2; //color

                                    }
                                    else if (EsNumero(caracter))
                                    {
                                        estado = 3;//literal
                                    }
                                    else
                                    {
                                        Error = true;
                                        LErrores.Add(new TError("El Caracter (" + caracter + ") es ilegal", linea, posLetra));
                                    }

                                    break;



                                case 1: //operador texto  (variable, label)

                                    if (!(EsNumero(caracter) || caracter == '_' || Esletra(caracter)))
                                    {
                                        Error = true;
                                        LErrores.Add(new TError(" Se esperaba numero, letra o \"_\". La Cadena (" + textoAnterior + ") es inválida. ", linea, posLetra));
                                    }
                                    break;
                                case 2: //color

                                    if (caracter == '\"')  //identificado color
                                    {
                                        //verificar si color de los posibles
                                        if (Colores.ContainsKey(textoAnterior))
                                        {
                                            tokenIdentificado = new TToken(textoAnterior, TToken.TtipoToken.token_StringColor);
                                            estado = 4; //fin del color
                                        }
                                        else
                                        {
                                            Error = true;
                                            LErrores.Add(new TError("String(Color) no disponible. El color " + textoAnterior + " es invalido.", linea, posLetra));
                                        }
                                    }
                                    else if (!Esletra(caracter))
                                    {
                                        Error = true;
                                        LErrores.Add(new TError("String(Color) Incompleto. La Cadena (" + textoAnterior + ") es invalida.", linea, posLetra));
                                    }
                                    break;
                                case 3: //literal

                                    if (!EsNumero(caracter))
                                    { //identificado literal numerico
                                        Error = true;
                                        LErrores.Add(new TError(" Se esperaba numero o operador. La Cadena (" + textoAnterior + ") es inválida. ", linea, posLetra));
                                    }
                                    break;
                                case 4:
                                    Error = true;
                                    LErrores.Add(new TError("Caracteres invalidos depues de Color. La Cadena (" + textoAnterior + ") es inválida. ", linea, posLetra));
                                    break;
                            }
                        }

                        if (!Error)
                        {
                            switch (estado)
                            {

                                case 1: //operador texto  ( variable, label)
                                    tokenIdentificado = new TToken(Palabra, TToken.TtipoToken.token_id);
                                    break;
                                case 2: //color
                                    Error = true;
                                    LErrores.Add(new TError("String(Color) Incompleto Se esperaba o caracter \" . La Cadena (" + Palabra + ") es invalida.", linea, posLetra));

                                    break;
                                case 3: //literal
                                    tokenIdentificado = new TToken(Palabra, TToken.TtipoToken.token_literal);
                                    break;
                                case 4:// esta correcto
                                    break;

                                default: //no es logico no tendria letras
                                    LErrores.Add(new TError(" Error De sistema. Estado do AnaliseLexico no logico (" + estado + ") ", linea, posLetra));
                                    break;
                            }
                        }
                    }

                    if (!(tokenIdentificado == null)) { lineaTokens.Add(tokenIdentificado); }

                }



                // clasificar palabrao colocr error

                LLineasDeToKens.Add(lineaTokens);
                LineasVisualesPrograma.Add(new TLineaVisual(listaDePalabras, lineaTokens, Error));
                linea++; //proxima linea
            }

        }

        if (LLineasDeToKens.Count == 0)
        {
            LErrores.Insert(0, new TError("Programa Vazio", -1));
        }

        if (LErrores.Count == 0) //el programa no tiene errores, analise sintactico
        {


        }

    }
    public void MostrarErros(Text atxt)
    {
        String destino = "";
        foreach (TError item in LErrores)
        {
            String aError = "Linea " + item.poslinea + ":\t" + item.texto + "\n";
            destino = destino + aError;
        }
        atxt.text = destino;

    }
    public void MostrarTokens(Text atxt)
    {
        String destino = "";

        for (int i = 0; i < LLineasDeToKens.Count; i++)
        {
            List<TToken> item = LLineasDeToKens[i];
            String aTokens = "Linea: " + i + "\t";
            foreach (TToken t in item)
            {
                aTokens = aTokens + t.simbolo + "{" + t.tipoToken + "}" + " ";
            }

            destino = destino + aTokens + "\n";
        }
        atxt.text = destino;
    }
    public void MostrarTodo(Text atxt)
    {
        String destino = "";

        for (int i = 0; i < LineasVisualesPrograma.Count; i++)
        {
            TLineaVisual item = LineasVisualesPrograma[i];
            string SError = (item.Error ? " E " : "   ");

            String aLineaVisual = " " + i + SError;
            String aTokens = "";
            foreach (TToken t in item.Tokens)
            {
                aTokens = aTokens + "{" + t.simbolo + " , " + t.tipoToken + "}" + " ";
            }
            destino = destino + aLineaVisual + "\t" + aTokens + "\n";
        }
        atxt.text = destino;
    }

    public void MostrarPrograma(InputField atxt)
    {
        String destino = "";

        for (int i = 0; i < LineasVisualesPrograma.Count; i++)
        {
            TLineaVisual item = LineasVisualesPrograma[i];
            string SError = (item.Error ? " E " : "   ");

            String aLineaVisual = " " + i + SError;
            String linea = "";
            foreach (String t in item.Palabras)
            {
                linea = linea + t + " ";
            }
            destino = destino + aLineaVisual + "\t" + linea + "\n";
        }
        atxt.text = destino;
    }

}
public class TInstrucion
{
    int linea;

    public static TInstrucion AnalizarLinea()
    {
        TInstrucion Inst = null;
        List<TToken> lt = TALexico.LLineasDeToKens[TASintactico.CursorLinea];
        if (!(TComando.Sintaxis(lt, out Inst) || TGoto.Sintaxis(lt, out Inst)
            || TAsignacion.Sintaxis(lt, out Inst) || TEtiqueta.Sintaxis(lt, out Inst)))
        {
            TALexico.LErrores.AddRange(TASintactico.LErroresSintacticos);
        }
        TASintactico.LErroresSintacticos.Clear();
        return Inst;
    }

    public static bool MatchCon(List<TToken> lt, ref int CursorToken, TToken.TtipoToken t)
    {
        if ((CursorToken < lt.Count) && (lt[CursorToken].tipoToken == t))
        {
            CursorToken++;
            return true;
        }
        return false;
    }

    public static bool MatchCon(List<TToken> lt, ref int CursorToken, TToken.TtipoToken t, String valor)
    {
        if (CursorToken < lt.Count)
        {
            TToken tok = lt[CursorToken];
            if ((tok.tipoToken == t) && (tok.simbolo == valor))
            {
                CursorToken++;
                return true;
            }
        }

        return false;
    }

    virtual public bool Semantica()
    {
        return (true);
    }
    
    virtual public bool Executar()
    {
        return (true);
    }


}
 public enum TtipoParametro
    {
        tipo_numero, tipo_Boolean, tipo_Color
     }

public class TParametro
{
    TtipoParametro tipo;
    int Valor;
    String Color;
    bool ValorBooleano;
    bool IsVariable=false;
    String Variable="";
    public TParametro(int aValor)
    {
        tipo = TtipoParametro.tipo_numero;
        Valor = aValor;
    }
    public TParametro(String aStr, bool variable)
    {
        if (variable)
        {
            IsVariable = true;
            Variable=aStr;
            
       }
        else
        {
            tipo = TtipoParametro.tipo_Color;
            Color = aStr;
        }
        
    }
    public TParametro(bool aBoolean)
    {
        tipo = TtipoParametro.tipo_Boolean;
        ValorBooleano = aBoolean;
    }
   
} 

public class TComando : TInstrucion
{
    String strfuncion;
    List<TParametro> LParametros;

    public TComando(String aFuncion, List<TParametro> aLParametros)
    {
        strfuncion = aFuncion;
        LParametros = aLParametros;
    }
    static bool funcionNombre(List<TToken> lt, ref int ACursor,  TToken.TtipoToken atipoComOFun )
    {
        return MatchCon(lt, ref ACursor,atipoComOFun) && MatchCon(lt, ref ACursor, TToken.TtipoToken.token_op, "(");
       
    }
    static bool Parametro(List<TToken> lt, ref int ACursor, ref TParametro par)
    {
        TToken tokActual = lt[ACursor];
        if (MatchCon(lt, ref ACursor, TToken.TtipoToken.token_id))
        {
            par = new TParametro(tokActual.simbolo, true);
            return true;
        }
        else if (MatchCon(lt, ref ACursor, TToken.TtipoToken.token_StringColor))
        {
            par = new TParametro(tokActual.simbolo, false);
            return true;
        }
        else if (MatchCon(lt, ref ACursor, TToken.TtipoToken.token_literal))
        {
            par = new TParametro(int.Parse(tokActual.simbolo));
            return true;
        }
        return false;
    }
       
    static bool listaParametro(List<TToken> lt, ref int ACursor, ref List<TParametro> lp)
    {
        TParametro par = null;
        if (Parametro(lt, ref ACursor, ref par))
        {
            lp.Add(par);
            return (ACursor >= lt.Count)
                    || (MatchCon(lt, ref ACursor, TToken.TtipoToken.token_op, ",") && listaParametro(lt, ref ACursor,ref lp));
        }
        return false;

    }
 
    static public bool Sintaxis(List<TToken> lt, out TInstrucion Inst)
    {
        int ACursor = 0;
        Inst = null;
        List<TParametro> lp = null;
        if (funcionNombre(lt, ref ACursor, TToken.TtipoToken.token_Comandos))
        {
            if (MatchCon(lt, ref ACursor, TToken.TtipoToken.token_op, ")"))
            {
                Inst = new TComando(lt[0].simbolo, new List<TParametro>());
                return true;
            }
            else if (listaParametro(lt, ref ACursor, ref lp) && MatchCon(lt, ref ACursor, TToken.TtipoToken.token_op, ")"))
            {
                Inst = new TComando(lt[0].simbolo, lp);
                return true;
            } else
            {
                //**Error inválida lista de parametros
            }
        }


        return false;
    }

    override public bool Semantica()
    {
        return true;
    }

}

public class TGoto : TInstrucion
{
    static public bool Sintaxis(List<TToken> lt, out TInstrucion Inst)
    {
        Inst = null;
        int ACursor = 0;
        return true;
    }
    override public bool Semantica()
    {
        return true;
    }
    
    override public bool Executar()
    {
        return true;
    }

}
public class TAsignacion : TInstrucion
{
    static public bool Sintaxis(List<TToken> lt, out TInstrucion Inst)
    {
        Inst = null;
        int ACursor = 0;


        return true;
    }
    override public bool Semantica()
    {
        return true;
    }
     override public bool Executar()
    {
        return true;
    }

}

public class TEtiqueta : TInstrucion
{
    static public bool Sintaxis(List<TToken> lt, out TInstrucion Inst)
    {
        Inst = null;
        int ACursor = 0;
        return true;
    }
    override public bool Semantica()
    {
        return true;
    }
     override public bool Executar()
    {
        return true;
    }

}

public class TASintactico
{
    public static List<TInstrucion> Instrucciones;
    public static int CursorLinea;
    public static List<TError> LErroresSintacticos; // lista 

    public TASintactico()
    {
        Instrucciones = new List<TInstrucion>();
           //  if Sinatxis

        for (CursorLinea = 0; CursorLinea < TALexico.LLineasDeToKens.Count; CursorLinea++)
        {
             Instrucciones.Add(TInstrucion.AnalizarLinea());
           
        }
    }
}