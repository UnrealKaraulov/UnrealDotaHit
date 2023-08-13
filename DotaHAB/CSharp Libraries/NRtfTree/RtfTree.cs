/********************************************************************************
 *   This file is part of NRtfTree Library.
 *
 *   NRtfTree Library is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   NRtfTree Library is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Lesser General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program. If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************************/

/********************************************************************************
 * Library:		NRtfTree
 * Version:     v0.3.0
 * Date:		02/09/2007
 * Copyright:   2007 Salvador Gomez
 * E-mail:      sgoliver.net@gmail.com
 * Home Page:	http://www.sgoliver.net
 * SF Project:	http://nrtftree.sourceforge.net
 *				http://sourceforge.net/projects/nrtftree
 * Class:		RtfTree
 * Description:	Representa un documento RTF en forma de �rbol.
 * ******************************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing;
using Net.Sgoliver.NRtfTree.Util;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Reresenta la estructura en forma de �rbol de un documento RTF.
        /// </summary>
        public class RtfTree
        {
			#region Atributos privados

            /// <summary>
            /// Nodo ra�z del documento RTF.
            /// </summary>
            private RtfTreeNode rootNode;
            /// <summary>
            /// Fichero/Cadena de entrada RTF
            /// </summary>
            private TextReader rtf;
            /// <summary>
            /// Analizador l�xico para RTF
            /// </summary>
            private RtfLex lex;
            /// <summary>
            /// Token actual
            /// </summary>
            private RtfToken tok;
            /// <summary>
            /// Profundidad del nodo actual
            /// </summary>
            private int level;
            /// <summary>
            /// Indica si se decodifican los caracteres especiales (\') uni�ndolos a nodos de texto contiguos.
            /// </summary>
            private bool mergeSpecialCharacters;

            #endregion

            #region Contructores

            /// <summary>
            /// Constructor de la clase RtfTree.
            /// </summary>
            public RtfTree()
            {
                //Se crea el nodo ra�z del documento
                rootNode = new RtfTreeNode(RtfNodeType.Root,"ROOT",false,0);

                rootNode.Tree = this;

				/* Inicializados por defecto */

                //Se inicializa la propiedad mergeSpecialCharacters
                mergeSpecialCharacters = false;

                //Se inicializa la profundidad actual
                //level = 0;
            }

            #endregion

            #region M�todos P�blicos

            /// <summary>
            /// Carga un fichero en formato RTF.
            /// </summary>
            /// <param name="path">Ruta del fichero con el documento.</param>
            /// <returns>Se devuelve el valor 0 en caso de no producirse ning�n error en la carga del documento.
            /// En caso contrario se devuelve el valor -1.</returns>
            public int LoadRtfFile(string path)
            {
                //Resultado de la carga
                int res = 0;

                //Se abre el fichero de entrada
                rtf = new StreamReader(path);

                //Se crea el analizador l�xico para RTF
                lex = new RtfLex(rtf);

                //Se carga el �rbol con el contenido del documento RTF
                res = parseRtfTree();

                //Se cierra el stream
                rtf.Close();

                //Se devuelve el resultado de la carga
                return res;
            }

            /// <summary>
            /// Carga una cadena de Texto con formato RTF.
            /// </summary>
            /// <param name="text">Cadena de Texto que contiene el documento.</param>
            /// <returns>Se devuelve el valor 0 en caso de no producirse ning�n error en la carga del documento.
            /// En caso contrario se devuelve el valor -1.</returns>
            public int LoadRtfText(string text)
            {
                //Resultado de la carga
                int res = 0;

                //Se abre el fichero de entrada
                rtf = new StringReader(text);

                //Se crea el analizador l�xico para RTF
                lex = new RtfLex(rtf);

                //Se carga el �rbol con el contenido del documento RTF
                res = parseRtfTree();

                //Se cierra el stream
                rtf.Close();

                //Se devuelve el resultado de la carga
                return res;
            }

            /// <summary>
            /// Escribe el c�digo RTF del documento a un fichero.
            /// </summary>
            /// <param name="filePath">Ruta del fichero a generar con el documento RTF.</param>
            public void SaveRtf(string filePath)
            { 
                //Stream de salida
                StreamWriter sw = new StreamWriter(filePath);

                //Se trasforma el �rbol RTF a Texto y se escribe al fichero
                sw.Write(this.RootNode.Rtf);

                //Se cierra el fichero
                sw.Flush();
                sw.Close();
            }

            /// <summary>
            /// Devuelve una representaci�n Textual del documento cargado.
            /// </summary>
            /// <returns>Cadena de caracteres con la representaci�n del documento.</returns>
            public override string ToString()
            {
                string res = "";

                res = toStringInm(rootNode, 0, false);

                return res;
            }

            /// <summary>
            /// Devuelve una representaci�n Textual del documento cargado. A�ade el tipo de nodo a la izquierda del contenido del nodo.
            /// </summary>
            /// <returns>Cadena de caracteres con la representaci�n del documento.</returns>
            public string ToStringEx()
            {
                string res = "";

                res = toStringInm(rootNode, 0, true);

                return res;
            }

            /// <summary>
            /// Devuelve la tabla de fuentes del documento RTF.
            /// </summary>
            /// <returns>Tabla de fuentes del documento RTF</returns>
            public String[] GetFontTable()
            {
                ArrayList tabla = new ArrayList();
                String[] tablaFuentes;

				//Nodo raiz del documento
				RtfTreeNode root = this.rootNode;

				//Grupo principal del documento
				RtfTreeNode nprin = root.FirstChild;

                //Buscamos la tabla de fuentes en el �rbol
                bool enc = false;
                int i = 0;
                RtfTreeNode ntf = new RtfTreeNode();  //Nodo con la tabla de fuentes

                while (!enc && i < nprin.ChildNodes.Count)
                {
                    if (nprin.ChildNodes[i].NodeType == RtfNodeType.Group &&
                        nprin.ChildNodes[i].FirstChild.NodeKey == "fonttbl")
                    {
                        enc = true;
                        ntf = nprin.ChildNodes[i];
                    }

                    i++;
                }

                //Rellenamos el array de fuentes
                for (int j = 1; j < ntf.ChildNodes.Count; j++)
                {
                    RtfTreeNode fuente = ntf.ChildNodes[j];

                    string nombreFuente = null;

                    foreach (RtfTreeNode nodo in fuente.ChildNodes)
                    {
                        if (nodo.NodeType == RtfNodeType.Text)
                            nombreFuente = nodo.NodeKey.Substring(0, nodo.NodeKey.Length - 1);
                    }

                    tabla.Add(nombreFuente);
                }

                //Convertimos el ArrayList en un array tradicional
                tablaFuentes = new String[tabla.Count];

                for (int c = 0; c < tabla.Count; c++)
                {
                    tablaFuentes[c] = (String)tabla[c];
                }

                return tablaFuentes;
            }

            /// <summary>
            /// Devuelve la tabla de colores del documento RTF.
            /// </summary>
            /// <returns>Tabla de colores del documento RTF</returns>
            public Color[] GetColorTable()
            {
                ArrayList tabla = new ArrayList();
                Color[] tablaColores;

                //Nodo raiz del documento
                RtfTreeNode root = this.rootNode;

                //Grupo principal del documento
                RtfTreeNode nprin = root.FirstChild;

                //Buscamos la tabla de colores en el �rbol
                bool enc = false;
                int i = 0;
                RtfTreeNode ntc = new RtfTreeNode();  //Nodo con la tabla de fuentes

                while (!enc && i < nprin.ChildNodes.Count)
                {
                    if (nprin.ChildNodes[i].NodeType == RtfNodeType.Group &&
                        nprin.ChildNodes[i].FirstChild.NodeKey == "colortbl")
                    {
                        enc = true;
                        ntc = nprin.ChildNodes[i];
                    }

                    i++;
                }

                //Rellenamos el array de colores
                int rojo = 0;
                int verde = 0;
                int azul = 0;

                //A�adimos el color por defecto, en este caso el negro.
                //tabla.Add(Color.FromArgb(rojo,verde,azul));

                for (int j = 1; j < ntc.ChildNodes.Count; j++)
                {
                    RtfTreeNode nodo = ntc.ChildNodes[j];

                    if (nodo.NodeType == RtfNodeType.Text && nodo.NodeKey.Trim() == ";")
                    {
                        tabla.Add(Color.FromArgb(rojo, verde, azul));

                        rojo = 0;
                        verde = 0;
                        azul = 0;
                    }
                    else if (nodo.NodeType == RtfNodeType.Keyword)
                    {
                        switch (nodo.NodeKey)
                        {
                            case "red":
                                rojo = nodo.Parameter;
                                break;
                            case "green":
                                verde = nodo.Parameter;
                                break;
                            case "blue":
                                azul = nodo.Parameter;
                                break;
                        }
                    }
                }

                //Convertimos el ArrayList en un array tradicional
                tablaColores = new Color[tabla.Count];

                for (int c = 0; c < tabla.Count; c++)
                {
                    tablaColores[c] = (Color)tabla[c];
                }

                return tablaColores;
            }

            /// <summary>
            /// Devuelve la informaci�n contenida en el grupo "\info" del documento RTF.
            /// </summary>
            /// <returns>Objeto InfoGroup con la informaci�n del grupo "\info" del documento RTF.</returns>
            public InfoGroup GetInfoGroup()
            {
                InfoGroup info = null;

                RtfTreeNode infoNode = this.RootNode.SelectSingleNode("info");

                //Si existe el nodo "\info" exraemos toda la informaci�n.
                if (infoNode != null)
                {
                    RtfTreeNode auxnode = null;

                    info = new InfoGroup();

                    //Title
                    if ((auxnode = this.rootNode.SelectSingleNode("title")) != null)
                        info.Title = auxnode.NextSibling.NodeKey;

                    //Subject
                    if ((auxnode = this.rootNode.SelectSingleNode("subject")) != null)
                        info.Subject = auxnode.NextSibling.NodeKey;

                    //Author
                    if ((auxnode = this.rootNode.SelectSingleNode("author")) != null)
                        info.Author = auxnode.NextSibling.NodeKey;

                    //Manager
                    if ((auxnode = this.rootNode.SelectSingleNode("manager")) != null)
                        info.Manager = auxnode.NextSibling.NodeKey;

                    //Company
                    if ((auxnode = this.rootNode.SelectSingleNode("company")) != null)
                        info.Company = auxnode.NextSibling.NodeKey;

                    //Operator
                    if ((auxnode = this.rootNode.SelectSingleNode("operator")) != null)
                        info.Operator = auxnode.NextSibling.NodeKey;

                    //Category
                    if ((auxnode = this.rootNode.SelectSingleNode("category")) != null)
                        info.Category = auxnode.NextSibling.NodeKey;

                    //Keywords
                    if ((auxnode = this.rootNode.SelectSingleNode("keywords")) != null)
                        info.Keywords = auxnode.NextSibling.NodeKey;

                    //Comments
                    if ((auxnode = this.rootNode.SelectSingleNode("comment")) != null)
                        info.Comment = auxnode.NextSibling.NodeKey;

                    //Document comments
                    if ((auxnode = this.rootNode.SelectSingleNode("doccomm")) != null)
                        info.DocComment = auxnode.NextSibling.NodeKey;

                    //Hlinkbase (The base address that is used for the path of all relative hyperlinks inserted in the document)
                    if ((auxnode = this.rootNode.SelectSingleNode("hlinkbase")) != null)
                        info.HlinkBase = auxnode.NextSibling.NodeKey;

                    //Version
                    if ((auxnode = this.rootNode.SelectSingleNode("version")) != null)
                        info.Version = auxnode.Parameter;

                    //Internal Version
                    if ((auxnode = this.rootNode.SelectSingleNode("vern")) != null)
                        info.InternalVersion = auxnode.Parameter;

                    //Editing Time
                    if ((auxnode = this.rootNode.SelectSingleNode("edmins")) != null)
                        info.EditingTime = auxnode.Parameter;

                    //Number of Pages
                    if ((auxnode = this.rootNode.SelectSingleNode("nofpages")) != null)
                        info.NumberOfPages = auxnode.Parameter;

                    //Number of Chars
                    if ((auxnode = this.rootNode.SelectSingleNode("nofchars")) != null)
                        info.NumberOfChars = auxnode.Parameter;

                    //Number of Words
                    if ((auxnode = this.rootNode.SelectSingleNode("nofwords")) != null)
                        info.NumberOfWords = auxnode.Parameter;

                    //Id
                    if ((auxnode = this.rootNode.SelectSingleNode("id")) != null)
                        info.Id = auxnode.Parameter;

                    //Creation DateTime
                    if ((auxnode = this.rootNode.SelectSingleNode("creatim")) != null)
                        info.CreationTime = parseDateTime(auxnode.ParentNode);

                    //Revision DateTime
                    if ((auxnode = this.rootNode.SelectSingleNode("revtim")) != null)
                        info.RevisionTime = parseDateTime(auxnode.ParentNode);
                    
                    //Last Print Time
                    if ((auxnode = this.rootNode.SelectSingleNode("printim")) != null)
                        info.LastPrintTime = parseDateTime(auxnode.ParentNode);

                    //Backup Time
                    if ((auxnode = this.rootNode.SelectSingleNode("buptim")) != null)
                        info.BackupTime = parseDateTime(auxnode.ParentNode);
                }

                return info;
            }

            /// <summary>
            /// Devuelve la tabla de c�digos con la que est� codificado el documento RTF.
            /// </summary>
            /// <returns>Tabla de c�digos del documento RTF. Si no est� especificada en el documento se devuelve la tabla de c�digos actual del sistema.</returns>
            public Encoding GetEncoding()
            {
                //Contributed by Jan Stuchl�k

                Encoding encoding = Encoding.Default;

                RtfTreeNode cpNode = RootNode.SelectSingleNode("ansicpg");

                if (cpNode != null)
                {
                    encoding = Encoding.GetEncoding(cpNode.Parameter);
                }

                return encoding;
            }

            #endregion

            #region M�todos Privados

            /// <summary>
            /// Analiza el documento y lo carga con estructura de �rbol.
            /// </summary>
            /// <returns>Se devuelve el valor 0 en caso de no producirse ning�n error en la carga del documento.
            /// En caso contrario se devuelve el valor -1.</returns>
            private int parseRtfTree()
            {
                //Resultado de la carga del documento
                int res = 0;

                //Codificaci�n por defecto del documento
                Encoding encoding = Encoding.Default;

                //Nodo actual
                RtfTreeNode curNode = rootNode;

                //Nuevos nodos para construir el �rbol RTF
                RtfTreeNode newNode = null;

                //Se obtiene el primer token
                tok = lex.NextToken();

                while (tok.Type != RtfTokenType.Eof)
                {
                    switch (tok.Type)
                    {
                        case RtfTokenType.GroupStart:
                            newNode = new RtfTreeNode(RtfNodeType.Group,"GROUP",false,0);
                            curNode.AppendChild(newNode);
                            curNode = newNode;
                            level++;
                            break;
                        case RtfTokenType.GroupEnd:
                            curNode = curNode.ParentNode;
                            level--;
                            break;
                        case RtfTokenType.Keyword:
                        case RtfTokenType.Control:
                        case RtfTokenType.Text:
                            if (mergeSpecialCharacters)
                            {
                                //Contributed by Jan Stuchl�k
                                bool isText = tok.Type == RtfTokenType.Text || (tok.Type == RtfTokenType.Control && tok.Key == "'");
                                if (curNode.LastChild != null && (curNode.LastChild.NodeType == RtfNodeType.Text && isText))
                                {
                                    if (tok.Type == RtfTokenType.Text)
                                    {
                                        curNode.LastChild.NodeKey += tok.Key;
                                        break;
                                    }
                                    if (tok.Type == RtfTokenType.Control && tok.Key == "'")
                                    {
                                        curNode.LastChild.NodeKey += DecodeControlChar(tok.Parameter, encoding);
                                        break;
                                    }
                                }
                                else
                                {
                                    //Primer caracter especial \'
                                    if (tok.Type == RtfTokenType.Control && tok.Key == "'")
                                    {
                                        newNode = new RtfTreeNode(RtfNodeType.Text, DecodeControlChar(tok.Parameter, encoding), false, 0);
                                        curNode.AppendChild(newNode);
                                        break;
                                    }
                                }
                            }

                            newNode = new RtfTreeNode(tok);
                            curNode.AppendChild(newNode);

                            if (mergeSpecialCharacters)
                            {
                                //Contributed by Jan Stuchl�k
                                if (level == 1 && newNode.NodeType == RtfNodeType.Keyword && newNode.NodeKey == "ansicpg")
                                {
                                    encoding = Encoding.GetEncoding(newNode.Parameter);
                                }
                            }

                            break;
                        default:
                            res = -1;
                            break;
                    }

                    //Se obtiene el siguiente token
                    tok = lex.NextToken();
                }

                //Si el nivel actual no es 0 ( == Algun grupo no est� bien formado )
                if (level != 0)
                {
                    res = -1;
                }

                //Se devuelve el resultado de la carga
                return res;
            }

            /// <summary>
            /// Decodifica un caracter especial indicado por su c�digo decimal
            /// </summary>
            /// <param name="code">C�digo del caracter especial (\')</param>
            /// <param name="enc">Codificaci�n utilizada para decodificar el caracter especial.</param>
            /// <returns>Caracter especial decodificado.</returns>
            private static string DecodeControlChar(int code, Encoding enc)
            {
                //Contributed by Jan Stuchl�k
                return enc.GetString(new byte[] {(byte)code});                
            }

            /// <summary>
            /// M�todo auxiliar para generar la representaci�n Textual del documento RTF.
            /// </summary>
            /// <param name="curNode">Nodo actual del �rbol.</param>
            /// <param name="level">Nivel actual en �rbol.</param>
            /// <param name="showNodeTypes">Indica si se mostrar� el tipo de cada nodo del �rbol.</param>
            /// <returns>Representaci�n Textual del nodo 'curNode' con nivel 'level'</returns>
            private string toStringInm(RtfTreeNode curNode, int level, bool showNodeTypes)
            {
                StringBuilder res = new StringBuilder();

                RtfNodeCollection children = curNode.ChildNodes;

                for (int i = 0; i < level; i++)
                    res.Append("  ");

                if (curNode.NodeType == RtfNodeType.Root)
                    res.Append("ROOT\r\n");
                else if (curNode.NodeType == RtfNodeType.Group)
                    res.Append("GROUP\r\n");
                else
                {
                    if (showNodeTypes)
                    {
                        res.Append(curNode.NodeType);
                        res.Append(": ");
                    }

                    res.Append(curNode.NodeKey);

                    if (curNode.HasParameter)
                    {
                        res.Append(" ");
                        res.Append(Convert.ToString(curNode.Parameter));
                    }

                    res.Append("\r\n");
                }

                foreach (RtfTreeNode node in children)
                {
                    res.Append(toStringInm(node, level + 1, showNodeTypes));
                }

                return res.ToString();
            }

			/// <summary>
			/// Parsea una fecha con formato "\yr2005\mo12\dy2\hr22\min56\sec15"
			/// </summary>
			/// <param name="group">Grupo RTF con la fecha.</param>
			/// <returns>Objeto DateTime con la fecha leida.</returns>
            private static DateTime parseDateTime(RtfTreeNode group)
            {
                DateTime dt;

                int year = 0, month = 0, day = 0, hour = 0, min = 0, sec = 0;

                foreach (RtfTreeNode node in group.ChildNodes)
                {
                    switch (node.NodeKey)
                    {
                        case "yr":
                            year = node.Parameter;
                            break;
                        case "mo":
                            month = node.Parameter;
                            break;
                        case "dy":
                            day = node.Parameter;
                            break;
                        case "hr":
                            hour = node.Parameter;
                            break;
                        case "min":
                            min = node.Parameter;
                            break;
                        case "sec":
                            sec = node.Parameter;
                            break;
                    }
                }

                dt = new DateTime(year, month, day, hour, min, sec);

                return dt;
            }

            /// <summary>
            /// Extrae el texto de un �rbol RTF.
            /// </summary>
            /// <returns>Texto plano del documento.</returns>
            private string ConvertToText()
            {
                RtfTreeNode pardNode =
                    this.RootNode.FirstChild.SelectSingleChildNode("pard");

                int pPard = this.RootNode.FirstChild.ChildNodes.IndexOf(pardNode);

                Encoding enc = this.GetEncoding();

                return ConvertToTextAux(this.RootNode.FirstChild, pPard, enc);
            }

            /// <summary>
            /// Extrae el texto de un nodo RTF (Auxiliar de ConvertToText())
            /// </summary>
            /// <param name="curNode">Nodo actual.</param>
            /// <param name="prim">Nodo a partir del que convertir.</param>
            /// <param name="enc">Codificaci�n del documento.</param>
            /// <returns>Texto plano del documento.</returns>
            private string ConvertToTextAux(RtfTreeNode curNode, int prim, Encoding enc)
            {
                StringBuilder res = new StringBuilder("");

                RtfTreeNode nprin = curNode;

                RtfTreeNode nodo = new RtfTreeNode();

                for (int i = prim; i < nprin.ChildNodes.Count; i++)
                {
                    nodo = nprin.ChildNodes[i];

                    if (nodo.NodeType == RtfNodeType.Group)
                    {
                        res.Append(ConvertToTextAux(nodo, 0, enc));
                    }
                    else if (nodo.NodeType == RtfNodeType.Control)
                    {
                        if (nodo.NodeKey == "'")
                            res.Append(DecodeControlChar(nodo.Parameter, enc));
                    }
                    else if (nodo.NodeType == RtfNodeType.Text)
                    {
                        res.Append(nodo.NodeKey);
                    }
                    else if (nodo.NodeType == RtfNodeType.Keyword)
                    {
                        if (nodo.NodeKey.Equals("par"))
                            res.AppendLine("");
                    }
                }

                return res.ToString();
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Devuelve el nodo ra�z del �rbol del documento.
            /// </summary>
            public RtfTreeNode RootNode
            {
                get
                {
                    //Se devuelve el nodo ra�z del documento
                    return rootNode;
                }
            }

            /// <summary>
            /// Devuelve el Texto RTF del documento.
            /// </summary>
            public string Rtf
            {
                get
                {
                    //Se devuelve el Texto RTF del documento completo
                    return rootNode.Rtf;
                }
            }

            /// <summary>
            /// Indica si se decodifican los caracteres especiales (\') uni�ndolos a nodos de texto contiguos.
            /// </summary>
            public bool MergeSpecialCharacters
            {
                get
                {
                    return mergeSpecialCharacters;
                }
                set
                {
                    mergeSpecialCharacters = value;
                }
            }

            /// <summary>
            /// Devuelve el texto plano del documento.
            /// </summary>
            public string Text
            {
                get
                {
                    return ConvertToText();
                }
            }

            #endregion
        }
    }
}
