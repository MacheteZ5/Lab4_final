using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ArbolB
{
    public class Nodo
    {
        //hijos
        public List<Nodo> Edges { get; private set; }
        //llaves, valores
        public List<Medicamentos> med { get; set; }
        public List<int> Keys { get; private set; }
        public Nodo Parent { get; set; }

        public Nodo(int key)
        {
            //lista de valores que tiene cada nodo
            Keys = new List<int>();
            Keys.Add(key);
            //lista de hijos que teiene cada nodo
            Edges = new List<Nodo>();
            med = new List<Medicamentos>();
        }
        //verificar si el valor esta dentro de las llaves del nodo
        public int HasKey(int k)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i] == k)
                {
                    return k;
                }
            }
            return -1;
        }
        //insertar hijo
        public void InsertEdge(Nodo edge)
        {
            for (int x = 0; x < Edges.Count; x++)
            {
                if (Edges[x].Keys[0] > edge.Keys[0])
                {
                    Edges.Insert(x, edge);
                    return;
                }
            }

            Edges.Add(edge);
            edge.Parent = this;
        }
        //eliminar hijo
        public bool RemoveEdge(Nodo n)
        {
            return Edges.Remove(n);
        }
        public Nodo RemoveEdge(int position)
        {
            Nodo edge = null;
            if (Edges.Count > position)
            {
                edge = Edges[position];
                edge.Parent = null;
                Edges.RemoveAt(position);
            }
            return edge;
        }
        //obtener hijo
        public Nodo GetEdge(int position)
        {
            if (position < Edges.Count)
            {
                return Edges[position];
            }
            else
            {
                return null;
            }
        }
        //encontrar posicion del hijo 
        public int FindEdgePosition(int k)
        {
            if (Keys.Count != 0)
            {
                int left = 0;
                for (int x = 0; x < Keys.Count; x++)
                {
                    if (left <= k && k < Keys[x])
                    {
                        return x;
                    }
                    else
                    {
                        left = Keys[x];
                    }
                }

                if (k > Keys[Keys.Count - 1])
                {
                    return Keys.Count;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 0;
            }

        }
        //empujar
        public void Push(int k)
        {
            if (Keys.Count == 3)
            {
                throw new InvalidOperationException("Cannot push value into a 3 keys node");
            }

            if (Keys.Count == 0)
            {
                Keys.Add(k);
            }
            else
            {
                int left = 0;
                for (int x = 0; x < Keys.Count; x++)
                {
                    if (left <= k && k < Keys[x])
                    {
                        Keys.Insert(x, k);
                        return;
                    }
                    else
                    {
                        left = Keys[x];
                    }
                }
                Keys.Add(k);
            }
        }
        //sistema de archivos en el espacio de usuario
        public void Fuse(Nodo n1)
        {
            int totalKeys = n1.Keys.Count;
            int totalEdges = n1.Edges.Count;

            totalKeys += this.Keys.Count;
            totalEdges += this.Edges.Count;

            if (totalKeys > 3)
            {
                throw new InvalidOperationException("Total keys of all nodes exceeded 3");
            }


            if (totalEdges > 4)
            {
                throw new InvalidOperationException("Total edges of all nodes exceeded 4");
            }


            for (int x = 0; x < n1.Keys.Count; x++)
            {
                int k = n1.Keys[x];
                this.Push(k);
            }

            for (int x = Edges.Count - 1; x >= 0; x--)
            {
                Nodo e = n1.RemoveEdge(x);
                this.InsertEdge(e);
            }
        }

        public void Fuse(Nodo n1, Nodo n2)
        {
            int totalKeys = n1.Keys.Count;
            int totalEdges = n1.Edges.Count;

            totalKeys += n2.Keys.Count;
            totalEdges += n2.Edges.Count;
            totalKeys += this.Keys.Count;
            totalEdges += this.Edges.Count;

            if (totalKeys > 3)
            {
                throw new InvalidOperationException("Total keys of all nodes exceeded 3");
            }

            if (totalEdges > 4)
            {
                throw new InvalidOperationException("Total edges of all nodes exceeded 4");
            }

            this.Fuse(n1);
            this.Fuse(n2);
        }
        public Nodo[] Split()
        {
            if (Keys.Count != 2)
            {
                throw new InvalidOperationException(string.Format("This node has {0} keys, can only split a 2 keys node", Keys.Count));
            }

            Nodo newRight = new Nodo(Keys[1]);

            for (int x = 2; x < Edges.Count; x++)
            {
                newRight.Edges.Add(this.Edges[x]);
            }

            for (int x = Edges.Count - 1; x >= 2; x--)
            {
                this.Edges.RemoveAt(x);
            }

            for (int x = 1; x < Keys.Count; x++)
            {
                Keys.RemoveAt(x);
            }

            return new Nodo[] { this, newRight };
        }
        public int? Pop(int position)
        {
            if (Keys.Count == 1)
            {
                throw new InvalidOperationException("Cannot pop value from a 1 key node");
            }

            if (position < Keys.Count)
            {
                int k = Keys[position];
                Keys.RemoveAt(position);

                return k;
            }
            return null;
        }
        //atravesar
        public Nodo Traverse(int k)
        {
            int pos = FindEdgePosition(k);

            if (pos < Edges.Count && pos > -1)
            {
                return Edges[pos];
            }
            else
            {
                return null;
            }
        }
    }
}
