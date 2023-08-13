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
 * Class:		RtfTreeNode
 * Description:	Nodo RTF de la representaci�n en �rbol de un documento.
 * ******************************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Net.Sgoliver.NRtfTree
{
    namespace Core
    {
        /// <summary>
        /// Nodo RTF de la representaci�n en �rbol de un documento.
        /// </summary>
        public class RtfTreeNode
        {
            #region Atributos Privados

            /// <summary>
            /// Tipo de nodo.
            /// </summary>
            private RtfNodeType type;
            /// <summary>
            /// Palabra clave / S�mbolo de Control / Texto.
            /// </summary>
            private string key;
            /// <summary>
            /// Indica si la palabra clave o s�mbolo de Control tiene par�metro.
            /// </summary>
            private bool hasParam;
            /// <summary>
            /// Par�metro de la palabra clave o s�mbolo de Control.
            /// </summary>
            private int param;
            /// <summary>
            /// Nodos hijos del nodo actual.
            /// </summary>
            private RtfNodeCollection children;
            /// <summary>
            /// Nodo padre del nodo actual.
            /// </summary>
            private RtfTreeNode parent;
            /// <summary>
            /// Nodo ra�z del documento.
            /// </summary>
            private RtfTreeNode root;
            /// <summary>
            /// �rbol Rtf al que pertenece el nodo
            /// </summary>
            private RtfTree tree;

            #endregion

            #region Constructores P�blicos

            /// <summary>
            /// Constructor de la clase RtfTreeNode. Crea un nodo sin inicializar.
            /// </summary>
            public RtfTreeNode()
            {
                children = new RtfNodeCollection();

                this.type = RtfNodeType.None;
                this.key = "";
                
				/* Inicializados por defecto */
				//this.param = 0;
				//this.hasParam = false;
                //this.parent = null;
                //this.root = null;
            }

            /// <summary>
            /// Constructor de la clase RtfTreeNode. Crea un nodo de un tipo concreto.
            /// </summary>
            /// <param name="nodeType">Tipo del nodo que se va a crear.</param>
            public RtfTreeNode(RtfNodeType nodeType)
            {
                children = new RtfNodeCollection();

                this.type = nodeType;
                this.key = "";
                
				/* Inicializados por defecto */
				//this.param = 0;
				//this.hasParam = false;
                //this.parent = null;
                //this.root = null;
            }

            /// <summary>
            /// Constructor de la clase RtfTreeNode. Crea un nodo especificando su tipo, palabra clave y par�metro.
            /// </summary>
            /// <param name="type">Tipo del nodo.</param>
            /// <param name="key">Palabra clave o s�mbolo de Control.</param>
            /// <param name="hasParameter">Indica si la palabra clave o el s�mbolo de Control va acompa�ado de un par�metro.</param>
            /// <param name="parameter">Par�metro del la palabra clave o s�mbolo de Control.</param>
            public RtfTreeNode(RtfNodeType type, string key, bool hasParameter, int parameter)
            {
                children = new RtfNodeCollection();

                this.type = type;
                this.key = key;
                this.hasParam = hasParameter;
                this.param = parameter;

				/* Inicializados por defecto */
                //this.parent = null;
                //this.root = null;
            }

            #endregion

            #region Constructor Privado

            /// <summary>
            /// Constructor privado de la clase RtfTreeNode. Crea un nodo a partir de un token del analizador l�xico.
            /// </summary>
            /// <param name="token">Token RTF devuelto por el analizador l�xico.</param>
            internal RtfTreeNode(RtfToken token)
            {
                children = new RtfNodeCollection();

                this.type = (RtfNodeType)token.Type;
                this.key = token.Key;
                this.hasParam = token.HasParameter;
                this.param = token.Parameter;

				/* Inicializados por defecto */
                //this.parent = null;
                //this.root = null;
            }

            #endregion

            #region M�todos P�blicos

            /// <summary>
            /// A�ade un nodo al final de la lista de hijos.
            /// </summary>
            /// <param name="newNode">Nuevo nodo a a�adir.</param>
            public void AppendChild(RtfTreeNode newNode)
            {
				if(newNode != null)
				{
					//Se asigna como nodo padre el nodo actual
					newNode.parent = this;

                    //Se actualizan las propiedades Root y Tree del nuevo nodo y sus posibles hijos
                    updateNodeRoot(newNode);

					//Se a�ade el nuevo nodo al final de la lista de nodos hijo
					children.Add(newNode);
				}
            }

            /// <summary>
            /// Inserta un nuevo nodo en una posici�n determinada de la lista de hijos.
            /// </summary>
            /// <param name="index">Posici�n en la que se insertar� el nodo.</param>
            /// <param name="newNode">Nuevo nodo a insertar.</param>
            public void InsertChild(int index, RtfTreeNode newNode)
            {
                if (newNode != null && index >= 0 && index <= children.Count)
                {
                    //Se asigna como nodo padre el nodo actual
                    newNode.parent = this;

                    //Se actualizan las propiedades Root y Tree del nuevo nodo y sus posibles hijos
                    updateNodeRoot(newNode);

                    //Se a�ade el nuevo nodo al final de la lista de nodos hijo
                    children.Insert(index, newNode);
                }
            }

            /// <summary>
            /// Elimina un nodo de la lista de hijos.
            /// </summary>
            /// <param name="index">Indice del nodo a eliminar.</param>
            public void RemoveChild(int index)
            {
                //Se elimina el i-�simo hijo
                children.RemoveAt(index);
            }

            /// <summary>
            /// Elimina un nodo de la lista de hijos.
            /// </summary>
            /// <param name="node">Nodo a eliminar.</param>
            public void RemoveChild(RtfTreeNode node)
            {
                //Se busca el nodo a eliminar
                int index = children.IndexOf(node);

                //Se elimina el i-�simo hijo
                children.RemoveAt(index);
            }

            /// <summary>
            /// Realiza una copia exacta del nodo actual.
            /// </summary>
            /// <param name="cloneChildren">Si este par�metro recibe el valor true se clonar�n tambi�n todos los nodos hijo del nodo actual.</param>
            /// <returns>Devuelve una copia exacta del nodo actual.</returns>
            public RtfTreeNode CloneNode(bool cloneChildren)
            {
                RtfTreeNode clon = new RtfTreeNode();

                clon.key = this.key;
                clon.hasParam = this.hasParam;
                clon.param = this.param;
                clon.parent = this.parent;
                clon.root = this.root;
                clon.type = this.type;

                //Si cloneChildren=false se copia directamente la lista de hijos
                if (!cloneChildren)
                {
                    clon.children = this.children;
                }
                else  //En caso contrario se clonan tambi�n cada uno de los hijos, propagando el par�metro cloneChildren=true
                {
                    clon.children = new RtfNodeCollection();

                    foreach (RtfTreeNode child in this.children)
                    {
                        clon.children.Add(child.CloneNode(true));
                    }
                }

                return clon;
            }

            /// <summary>
            /// Indica si el nodo actual tiene nodos hijos.
            /// </summary>
            /// <returns>Devuelve true si el nodo actual tiene alg�n nodo hijo.</returns>
            public bool HasChildNodes()
            {
                return (children.Count != 0);
            }

            /// <summary>
            /// Devuelve el primer nodo de la lista de nodos hijos del nodo actual cuya palabra clave es la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Primer nodo de la lista de nodos hijos del nodo actual cuya palabra clave es la indicada como par�metro.</returns>
            public RtfTreeNode SelectSingleChildNode(string keyword)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].key == keyword)
                    {
                        node = children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo de la lista de nodos hijos del nodo actual cuyo tipo es el indicado como par�metro.
            /// </summary>
            /// <param name="nodeType">Tipo de nodo buscado.</param>
            /// <returns>Primer nodo de la lista de nodos hijos del nodo actual cuyo tipo es el indicado como par�metro.</returns>
            public RtfTreeNode SelectSingleChildNode(RtfNodeType nodeType)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].type == nodeType)
                    {
                        node = children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo de la lista de nodos hijos del nodo actual cuya palabra clave y par�metro son los indicados como par�metros.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <param name="param">Par�metro buscado.</param>
            /// <returns>Primer nodo de la lista de nodos hijos del nodo actual cuya palabra clave y par�metro son los indicados como par�metros.</returns>
            public RtfTreeNode SelectSingleChildNode(string keyword, int param)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].key == keyword && children[i].param == param)
                    {
                        node = children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo del �rbol, a partir del nodo actual, cuyo tipo es el indicado como par�metro.
            /// </summary>
            /// <param name="nodeType">Tipo del nodo buscado.</param>
            /// <returns>Primer nodo del �rbol, a partir del nodo actual, cuyo tipo es el indicado como par�metro.</returns>
            public RtfTreeNode SelectSingleNode(RtfNodeType nodeType)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].type == nodeType)
                    {
                        node = children[i];
                        found = true;
                    }
                    else
                    {
                        node = children[i].SelectSingleNode(nodeType);

                        if (node != null)
                        {
                            found = true;
                        }
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo del �rbol, a partir del nodo actual, cuya palabra clave es la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Primer nodo del �rbol, a partir del nodo actual, cuya palabra clave es la indicada como par�metro.</returns>
            public RtfTreeNode SelectSingleNode(string keyword)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].key == keyword)
                    {
                        node = children[i];
                        found = true;
                    }
                    else
                    {
                        node = children[i].SelectSingleNode(keyword);

                        if (node != null)
                        {
                            found = true;
                        }
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo del �rbol, a partir del nodo actual, cuya palabra clave y par�metro son los indicados como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <param name="param">Par�metro buscado.</param>
            /// <returns>Primer nodo del �rbol, a partir del nodo actual, cuya palabra clave y par�metro son ls indicados como par�metro.</returns>
            public RtfTreeNode SelectSingleNode(string keyword, int param)
            {
                int i = 0;
                bool found = false;
                RtfTreeNode node = null;

                while (i < children.Count && !found)
                {
                    if (children[i].key == keyword && children[i].param == param)
                    {
                        node = children[i];
                        found = true;
                    }
                    else
                    {
                        node = children[i].SelectSingleNode(keyword, param);

                        if (node != null)
                        {
                            found = true;
                        }
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve todos los nodos, a partir del nodo actual, cuya palabra clave es la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Colecci�n de nodos, a partir del nodo actual, cuya palabra clave es la indicada como par�metro.</returns>
            public RtfNodeCollection SelectNodes(string keyword)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.key == keyword)
                    {
                        nodes.Add(node);
                    }

                    nodes.AddRange(node.SelectNodes(keyword));
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve todos los nodos, a partir del nodo actual, cuyo tipo es el indicado como par�metro.
            /// </summary>
            /// <param name="nodeType">Tipo del nodo buscado.</param>
            /// <returns>Colecci�n de nodos, a partir del nodo actual, cuyo tipo es la indicado como par�metro.</returns>
            public RtfNodeCollection SelectNodes(RtfNodeType nodeType)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.type == nodeType)
                    {
                        nodes.Add(node);
                    }

                    nodes.AddRange(node.SelectNodes(nodeType));
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve todos los nodos, a partir del nodo actual, cuya palabra clave y par�metro son los indicados como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <param name="param">Par�metro buscado.</param>
            /// <returns>Colecci�n de nodos, a partir del nodo actual, cuya palabra clave y par�metro son los indicados como par�metro.</returns>
            public RtfNodeCollection SelectNodes(string keyword, int param)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.key == keyword && node.param == param)
                    {
                        nodes.Add(node);
                    }

                    nodes.AddRange(node.SelectNodes(keyword, param));
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve todos los nodos de la lista de nodos hijos del nodo actual cuya palabra clave es la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Colecci�n de nodos de la lista de nodos hijos del nodo actual cuya palabra clave es la indicada como par�metro.</returns>
            public RtfNodeCollection SelectChildNodes(string keyword)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.key == keyword)
                    {
                        nodes.Add(node);
                    }
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve todos los nodos de la lista de nodos hijos del nodo actual cuyo tipo es el indicado como par�metro.
            /// </summary>
            /// <param name="nodeType">Tipo del nodo buscado.</param>
            /// <returns>Colecci�n de nodos de la lista de nodos hijos del nodo actual cuyo tipo es el indicado como par�metro.</returns>
            public RtfNodeCollection SelectChildNodes(RtfNodeType nodeType)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.type == nodeType)
                    {
                        nodes.Add(node);
                    }
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve todos los nodos de la lista de nodos hijos del nodo actual cuya palabra clave y par�metro son los indicados como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <param name="param">Par�metro buscado.</param>
            /// <returns>Colecci�n de nodos de la lista de nodos hijos del nodo actual cuya palabra clave y par�metro son los indicados como par�metro.</returns>
            public RtfNodeCollection SelectChildNodes(string keyword, int param)
            {
                RtfNodeCollection nodes = new RtfNodeCollection();

                foreach (RtfTreeNode node in children)
                {
                    if (node.key == keyword && node.param == param)
                    {
                        nodes.Add(node);
                    }
                }

                return nodes;
            }

            /// <summary>
            /// Devuelve el primer nodo hermano del actual cuya palabra clave es la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Primer nodo hermano del actual cuya palabra clave es la indicada como par�metro.</returns>
            public RtfTreeNode SelectSibling(string keyword)
            {
                RtfTreeNode par = this.parent;
                int curInd = par.ChildNodes.IndexOf(this);

                int i = curInd + 1;
                bool found = false;
                RtfTreeNode node = null;

                while (i < par.children.Count && !found)
                {
                    if (par.children[i].key == keyword)
                    {
                        node = par.children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo hermano del actual cuyo tipo es el indicado como par�metro.
            /// </summary>
            /// <param name="nodeType">Tpo de nodo buscado.</param>
            /// <returns>Primer nodo hermano del actual cuyo tipo es el indicado como par�metro.</returns>
            public RtfTreeNode SelectSibling(RtfNodeType nodeType)
            {
                RtfTreeNode par = this.parent;
                int curInd = par.ChildNodes.IndexOf(this);

                int i = curInd + 1;
                bool found = false;
                RtfTreeNode node = null;

                while (i < par.children.Count && !found)
                {
                    if (par.children[i].type == nodeType)
                    {
                        node = par.children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

            /// <summary>
            /// Devuelve el primer nodo hermano del actual cuya palabra clave y par�metro son los indicados como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <param name="param">Par�metro buscado.</param>
            /// <returns>Primer nodo hermano del actual cuya palabra clave y par�metro son los indicados como par�metro.</returns>
            public RtfTreeNode SelectSibling(string keyword, int param)
            {
                RtfTreeNode par = this.parent;
                int curInd = par.ChildNodes.IndexOf(this);

                int i = curInd + 1;
                bool found = false;
                RtfTreeNode node = null;

                while (i < par.children.Count && !found)
                {
                    if (par.children[i].key == keyword && par.children[i].param == param)
                    {
                        node = par.children[i];
                        found = true;
                    }

                    i++;
                }

                return node;
            }

			/// <summary>
			/// Devuelve una representaci�n del nodo donde se indica su tipo, clave, indicador de par�metro y valor de par�metro
			/// </summary>
			/// <returns>Cadena de caracteres del tipo [TIPO, CLAVE, IND_PARAMETRO, VAL_PARAMETRO]</returns>
			public override string ToString()
			{
				return "[" + this.type + ", " + this.key + ", " + this.hasParam + ", " + this.param + "]";
			}

            #endregion

            #region Metodos Privados

            /// <summary>
            /// Obtiene el Texto RTF a partir de la representaci�n en �rbol del nodo actual.
            /// </summary>
            /// <returns>Texto RTF del nodo.</returns>
            private string getRtf()
            {
                string res = "";

                Encoding enc = this.tree.GetEncoding();

                res = getRtfInm(this, null, enc);

                return res;
            }

            /// <summary>
            /// M�todo auxiliar para obtener el Texto RTF del nodo actual a partir de su representaci�n en �rbol.
            /// </summary>
            /// <param name="curNode">Nodo actual del �rbol.</param>
            /// <param name="prevNode">Nodo anterior tratado.</param>
            /// <param name="enc">Codificaci�n del documento.</param>
            /// <returns>Texto en formato RTF del nodo.</returns>
            private string getRtfInm(RtfTreeNode curNode, RtfTreeNode prevNode, Encoding enc)
            {
                StringBuilder res = new StringBuilder("");

                if (curNode.NodeType == RtfNodeType.Root)
                    res.Append("");
                else if (curNode.NodeType == RtfNodeType.Group)
                    res.Append("{");
                else
                {
                    if (curNode.NodeType != RtfNodeType.Text)
                    {
                        res.Append("\\");
                    }
                    else  //curNode.NodeType == RtfNodeType.Text
                    {
                        if (prevNode == null || prevNode.NodeType == RtfNodeType.Control || prevNode.NodeType == RtfNodeType.Text)
                        {
                            res.Append("");
                        }
                        else //antNode.NodeType == RtfNodeType.KEYWORD
                        {
                            res.Append(" ");
                        }
                    }

                    AppendEncoded(res, curNode.NodeKey, enc);

                    if (curNode.HasParameter)
                    {
                        if (curNode.NodeType == RtfNodeType.Keyword)
                        {
                            res.Append(Convert.ToString(curNode.Parameter));
                        }
                        else if (curNode.NodeType == RtfNodeType.Control)
                        {
                            //Si es un caracter especial como las vocales acentuadas
                            if (curNode.NodeKey == "\'")
                            {						
								res.Append(GetHexa(curNode.Parameter));
                            }
                        }
                    }
                }

                //Se obtienen los nodos hijos
                RtfNodeCollection children = curNode.ChildNodes;

                for (int i = 0; i < children.Count; i++)
                {
                    RtfTreeNode node = children[i];

                    if (i > 0)
                        res.Append(getRtfInm(node, children[i - 1], enc));
                    else
                        res.Append(getRtfInm(node, null, enc));
                }

                if (curNode.NodeType == RtfNodeType.Group)
                {
                    res.Append("}");
                }

                return res.ToString();
            }

            /// <summary>
            /// Concatena dos cadenas utilizando la codificaci�n del documento.
            /// </summary>
            /// <param name="res">Cadena original.</param>
            /// <param name="s">Cadena a a�adir.</param>
            /// <param name="enc">Codificaci�n del documento.</param>
            private void AppendEncoded(StringBuilder res, string s, Encoding enc)
            {
                //Contributed by Jan Stuchl�k

                for (int i = 0; i < s.Length; i++)
                {
                    int code = Char.ConvertToUtf32(s, i);

                    if (code >= 128 || code < 32)
                    {
                        res.Append(@"\'");
                        byte[] bytes = enc.GetBytes(new char[] { s[i] });
                        res.Append(GetHexa(bytes[0]));
                    }
                    else
                    {
                        res.Append(s[i]);
                    }
                }
            }

            /// <summary>
            /// Obtiene el c�digo hexadecimal de un entero.
            /// </summary>
            /// <param name="code">N�mero entero.</param>
            /// <returns>C�digo hexadecimal del entero pasado como par�metro.</returns>
            private string GetHexa(int code)
            {
                //Contributed by Jan Stuchl�k

                string hexa = Convert.ToString(code, 16);

                if (hexa.Length == 1)
                {
                    hexa = "0" + hexa;
                }

                return hexa;
            }

            /// <summary>
            /// Actualiza las propiedades Root y Tree de un nodo (y sus hijos) con las del nodo actual.
            /// </summary>
            /// <param name="node">Nodo a actualizar.</param>
            private void updateNodeRoot(RtfTreeNode node)
            {
                //Se asigna el nodo ra�z del documento
                node.root = this.root;

                //Se asigna el �rbol propietario del nodo
                node.tree = this.tree;

                //Se actualizan recursivamente los hijos del nodo actual
                foreach (RtfTreeNode nod in node.children)
                {
                    updateNodeRoot(nod);
                }
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Devuelve el nodo ra�z del �rbol del documento.
            /// </summary>
            /// <remarks>
            /// �ste no es el nodo ra�z del �rbol, sino que se trata simplemente de un nodo ficticio  de tipo ROOT del que parte el resto del �rbol RTF.
            /// Tendr� por tanto un solo nodo hijo de tipo GROUP, raiz real del �rbol.
			/// </remarks>
            public RtfTreeNode RootNode
            {
                get
                {
                    return root;
                }
				set
				{
					root = value;
				}
            }

            /// <summary>
            /// Devuelve el nodo padre del nodo actual.
            /// </summary>
            public RtfTreeNode ParentNode
            {
                get
                {
                    return parent;
                }
				set
				{
					parent = value;
				}
            }

            /// <summary>
            /// Devuelve el �rbol Rtf al que pertenece el nodo.
            /// </summary>
            public RtfTree Tree
            {
                get
                {
                    return tree;
                }
                set
                {
                    tree = value;
                }
            }

            /// <summary>
            /// Devuelve el tipo del nodo actual.
            /// </summary>
            public RtfNodeType NodeType
            {
                get
                {
                    return type;
                }
				set
				{
					type = value;
				}
            }

            /// <summary>
            /// Devuelve la palabra clave, s�mbolo de Control o Texto del nodo actual.
            /// </summary>
            public string NodeKey
            {
                get
                {
                    return key;
                }
                set
                {
                    key = value;
                }
            }

            /// <summary>
            /// Indica si el nodo actual tiene par�metro asignado.
            /// </summary>
            public bool HasParameter
            {
                get
                {
                    return hasParam;
                }
                set
                {
                    hasParam = value;
                }
            }

            /// <summary>
            /// Devuelve el par�metro asignado al nodo actual.
            /// </summary>
            public int Parameter
            {
                get
                {
                    return param;
                }
                set
                {
                    param = value;
                }
            }

            /// <summary>
            /// Devuelve la colecci�n de nodos hijo del nodo actual.
            /// </summary>
            public RtfNodeCollection ChildNodes
            {
                get
                {
                    return children;
                }
            }

            /// <summary>
            /// Devuelve el primer nodo hijo cuya palabra clave sea la indicada como par�metro.
            /// </summary>
            /// <param name="keyword">Palabra clave buscada.</param>
            /// <returns>Primer nodo hijo cuya palabra clave sea la indicada como par�metro. En caso de no existir se devuelve null.</returns>
            public RtfTreeNode this[string keyword]
            {
                get
                {
                    return this.SelectSingleChildNode(keyword);
                }
            }

            /// <summary>
            /// Devuelve el primer nodo hijo del nodo actual.
            /// </summary>
            public RtfTreeNode FirstChild
            {
                get
                {
                    if (children.Count > 0)
                        return children[0];
                    else
                        return null;
                }
            }

            /// <summary>
            /// Devuelve el �ltimo nodo hijo del nodo actual.
            /// </summary>
            public RtfTreeNode LastChild
            {
                get
                {
                    if (children.Count > 0)
                        return children[children.Count - 1];
                    else
                        return null;
                }
            }

            /// <summary>
            /// Devuelve el nodo hermano siguiente del nodo actual (Dos nodos son hermanos si tienen el mismo nodo padre [ParentNode]).
            /// </summary>
            public RtfTreeNode NextSibling
            {
                get
                {
                    int currentIndex = parent.children.IndexOf(this);

                    if (parent.children.Count > currentIndex + 1)
                        return parent.children[currentIndex + 1];
                    else
                        return null;
                }
            }

            /// <summary>
            /// Devuelve el nodo hermano anterior del nodo actual (Dos nodos son hermanos si tienen el mismo nodo padre [ParentNode]).
            /// </summary>
            public RtfTreeNode PreviousSibling
            {
                get
                {
                    int currentIndex = parent.children.IndexOf(this);

                    if (currentIndex > 0)
                        return parent.children[currentIndex - 1];
                    else
                        return null;
                }
            }

            /// <summary>
            /// Devuelve el c�digo RTF del nodo actual y todos sus nodos hijos.
            /// </summary>
            public string Rtf
            {
                get
                {
                    return getRtf();
                }
            }

            #endregion
        }
    }
}
