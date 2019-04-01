using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ArbolB
{
    public class ArbolB
    {
        public Nodo Raiz { get; set; }
        public ArbolB()
        {
            Raiz = null;
        }
        public void Insertar(int value, Medicamentos medi, int grado)
        {
            if (Raiz == null)
            {
                Raiz = new Nodo(grado);
                Raiz.med.Add(medi);
                return;
            }
            Nodo actual = Raiz;
            Nodo padre = null;
            while (actual != null)
            {
                if (actual.Keys.Count == 3)
                {
                    if (padre == null)
                    {
                        int k = actual.Pop(1).Value;
                        Nodo nuevaRaiz = new Nodo(k);
                        Nodo[] newNodos = actual.Split();
                        nuevaRaiz.InsertEdge(newNodos[0]);
                        nuevaRaiz.InsertEdge(newNodos[1]);
                        Raiz = nuevaRaiz;
                        actual = nuevaRaiz;
                    }
                    else
                    {
                        int? k = actual.Pop(1);
                        if (k != null)
                        {
                            padre.Push(k.Value);
                        }
                        Nodo[] nNodos = actual.Split();
                        int pos1 = padre.FindEdgePosition(nNodos[1].Keys[0]);
                        padre.InsertEdge(nNodos[1]);
                        int posActual = padre.FindEdgePosition(value);
                        actual = padre.GetEdge(posActual);

                    }
                }
                padre = actual;
                actual = actual.Traverse(value);
                padre.med.Add(medi);
                if (actual == null)
                {
                    padre.Push(value);
                }
            }
        }

        public Nodo Find(int k)
        {
            Nodo curr = Raiz;

            while (curr != null)
            {
                if (curr.HasKey(k) >= 0)
                {
                    return curr;
                }
                else
                {
                    int p = curr.FindEdgePosition(k);
                    curr = curr.GetEdge(p);
                }
            }

            return null;
        }
        public void Remove(int k)
        {
            Nodo curr = Raiz;
            Nodo parent = null;
            while (curr != null)
            {
                if (curr.Keys.Count == 1)
                {
                    if (curr != Raiz)
                    {
                        int cK = curr.Keys[0];
                        int edgePos = parent.FindEdgePosition(cK);

                        bool? takeRight = null;
                        Nodo sibling = null;

                        if (edgePos > -1)
                        {
                            if (edgePos < 3)
                            {
                                sibling = parent.GetEdge(edgePos + 1);
                                if (sibling.Keys.Count > 1)
                                {
                                    takeRight = true;
                                }
                            }

                            if (takeRight == null)
                            {
                                if (edgePos > 0)
                                {
                                    sibling = parent.GetEdge(edgePos - 1);
                                    if (sibling.Keys.Count > 1)
                                    {
                                        takeRight = false;
                                    }
                                }
                            }

                            if (takeRight != null)
                            {
                                int? pK = 0;
                                int? sK = 0;

                                if (takeRight.Value)
                                {
                                    pK = parent.Pop(edgePos).Value;
                                    sK = sibling.Pop(0).Value;

                                    if (sibling.Edges.Count > 0)
                                    {
                                        Nodo edge = sibling.RemoveEdge(0);
                                        curr.InsertEdge(edge);
                                    }
                                }
                                else
                                {
                                    pK = parent.Pop(edgePos).Value;
                                    sK = sibling.Pop(sibling.Keys.Count - 1).Value;

                                    if (sibling.Edges.Count > 0)
                                    {
                                        Nodo edge = sibling.RemoveEdge(sibling.Edges.Count - 1);
                                        curr.InsertEdge(edge);
                                    }
                                }

                                parent.Push(sK.Value);
                                curr.Push(pK.Value);
                            }
                            else
                            {
                                int? pK = null;
                                if (parent.Edges.Count >= 2)
                                {
                                    if (edgePos == 0)  
                                    {
                                        pK = parent.Pop(0);
                                    }
                                    else if (edgePos == parent.Edges.Count)//if n is the right most node take parent's right most key
                                    {
                                        pK = parent.Pop(parent.Keys.Count - 1);
                                    }
                                    else
                                    {
                                        pK = parent.Pop(1);
                                    }

                                    if (pK != null)
                                    {
                                        curr.Push(pK.Value);
                                        Nodo sib = null;
                                        if (edgePos != parent.Edges.Count)//use right sibling if it is not the rightmost node
                                        {
                                            sib = parent.RemoveEdge(edgePos + 1);
                                        }
                                        else
                                        {
                                            sib = parent.RemoveEdge(parent.Edges.Count - 1);
                                        }

                                        curr.Fuse(sib);
                                    }
                                }
                                else
                                {
                                    curr.Fuse(parent, sibling);
                                    Raiz = curr;
                                    parent = null;
                                }
                            }
                        }
                    }
                }

                int rmPos = -1;
                if ((rmPos = curr.HasKey(k)) >= 0)
                {
                    if (curr.Edges.Count == 0)
                    {
                        if (curr.Keys.Count == 0)
                        {
                            parent.Edges.Remove(curr);
                        }
                        else
                        {
                            curr.Pop(rmPos);
                        }
                    }
                    else
                    {
                        Nodo successor = Min(curr.Edges[rmPos]);
                        int sK = successor.Keys[0];
                        if (successor.Keys.Count > 1)
                        {
                            successor.Pop(0);
                        }
                        else
                        {
                            if (successor.Edges.Count == 0)
                            {
                                Nodo p = successor.Parent;
                                p.RemoveEdge(successor);
                            }
                            else
                            {
                            }
                        }
                    }

                    curr = null;
                }
                else
                {
                    int p = curr.FindEdgePosition(k);
                    parent = curr;
                    curr = curr.GetEdge(p);
                }
            }

        }
        public Nodo Min(Nodo n = null)
        {
            if (n == null)
            {
                n = Raiz;
            }

            Nodo curr = n;
            if (curr != null)
            {
                while (curr.Edges.Count > 0)
                {
                    curr = curr.Edges[0];
                }
            }

            return curr;
        }
        static List<Medicamentos[]> dato = new List<Medicamentos[]>();
        public int[] Inorder(Nodo n = null)
        {
            if (n == null)
            {
                n = Raiz;
            }
            List<int> items = new List<int>();
            Tuple<Nodo, int> curr = new Tuple<Nodo, int>(n, 0);
            Stack<Tuple<Nodo, int>> stack = new Stack<Tuple<Nodo, int>>();
            while (stack.Count > 0 || curr.Item1 != null)
            {
                Medicamentos[] s = new Medicamentos[1000];
                if (curr.Item1 != null)
                {
                    stack.Push(curr);
                    Nodo leftChild = curr.Item1.GetEdge(curr.Item2);
                    s = curr.Item1.med.ToArray();
                    dato.Add(s);
                    curr = new Tuple<Nodo, int>(leftChild, 0);
                }
                else
                {
                    curr = stack.Pop();
                    Nodo currNode = curr.Item1;
                    s = curr.Item1.med.ToArray(); 
                    dato.Add(s);
                    if (curr.Item2 < currNode.Keys.Count)
                    {
                        items.Add(currNode.Keys[curr.Item2]);
                        curr = new Tuple<Nodo, int>(currNode, curr.Item2 + 1);
                    }
                    else
                    {
                        Nodo rightChild = currNode.GetEdge(curr.Item2 + 1);
                        curr = new Tuple<Nodo, int>(rightChild, curr.Item2 + 1);
                    }
                }
            }
            
            return items.ToArray();
        }
        public List<Medicamentos[]> retornar()
        {
            return dato;
        }
    }
}
