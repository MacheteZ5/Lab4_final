using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab4.Models;
using ArbolB;
using System.IO;

namespace Lab4.Controllers
{
    //José Montenegro 1229918
    //Fernando Oliva 1251518
    public class ArbolBController : Controller
    {
        // GET: ArbolB
        public ActionResult Index()
        {
            return View(new List<Inventario>());
        }
        static List<Inventario> lista = new List<Inventario>();
        static ArbolB.ArbolB arbol = new ArbolB.ArbolB();
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            int grado = Convert.ToInt32(Request.Form["grado"]);
            List<Inventario> inventario = new List<Inventario>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                //dirección del archivo
                string path = Server.MapPath("~/archivo/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                int contador = 0;
                string csvData = System.IO.File.ReadAllText(filePath);
                //El split del archivo es por columna
                foreach (string row in csvData.Split('\n'))
                {
                    if ((!string.IsNullOrEmpty(row)) && (contador != 0))
                    {
                        Inventario inventarios = new Inventario();
                        inventarios.ID = Convert.ToInt32(row.Split(';')[0]);
                        inventarios.Nombre = " " + row.Split(';')[1];
                        inventarios.Descripción = row.Split(';')[2];
                        inventarios.Casaproductora = row.Split(';')[3];
                        inventarios.Precio = row.Split(';')[4];
                        inventarios.Cantidad = Convert.ToInt32(row.Split(';')[5]);
                        inventario.Add(inventarios);
                        lista.Add(inventarios);
                    }
                    else
                    {
                        contador++;
                    }
                }
            }
            // al finalizar la lectura del archivo, los datos se ingresaron a una lista de forma ordenada
            // Se recorre la lista nodo por nodo para ingresar cada elemento que contenga el nodo dentro del arbol binario.
            
            string[] medicamentos = new string[grado+1];
            foreach (Inventario nodo in inventario)
            {
                Medicamentos med = new Medicamentos();
                med.ID = nodo.ID;
                med.Nombre = nodo.Nombre;
                med.Descripción = nodo.Descripción;
                med.Casaproductora = nodo.Casaproductora;
                med.Precio = nodo.Precio;
                med.Cantidad = nodo.Cantidad;
                arbol.Insertar(med.ID, med,grado);
            }
            return View(inventario);
        }
        public ActionResult Menú()
        {
            return View();
        }

        public ActionResult InOrden()
        {
            List<Medicamentos[]> extra = new List<Medicamentos[]>();
            arbol.Inorder();
            extra = arbol.retornar();
            List<Inventario> meds = new List<Inventario>();
            int x = 0;
            foreach(Medicamentos m in extra[x])
            {
                Inventario med = new Inventario();
                med.ID = m.ID;
                med.Nombre = m.Nombre;
                med.Descripción = m.Descripción;
                med.Casaproductora = m.Casaproductora;
                med.Precio = m.Precio;
                med.Cantidad = m.Cantidad;
                meds.Add(med);
                x++;
            }
            var model = from puntos in meds
                        select puntos;
            return View("InOrden", model);
        }
        static List<Inventario> ñ = new List<Inventario>(); 
        public ActionResult Pedidos()
        {
            string nombre = Request.Form["nombre"].ToString();
            
            foreach(Inventario m in lista)
            {
                if (m.Nombre == nombre)
                {
                    Inventario s = new Inventario();
                    s.ID = m.ID;
                    s.Nombre = m.Nombre;
                    s.Descripción = m.Descripción;
                    s.Casaproductora = m.Casaproductora;
                    s.Precio = m.Precio;
                    s.Cantidad = m.Cantidad;
                    Guid g;
                    g = Guid.NewGuid();
                    s.Guid = g;
                    ñ.Add(s);
                }
            }
            var model = from puntos in ñ
                        select puntos;
            return View("Pedidos", model);
        }
        
        public ActionResult Guides()
        {
            Guid g;
            g = Guid.NewGuid();
            List<Guid> dato = new List<Guid>();
            dato.Add(g);
             var model = from puntos in dato
                         select puntos;
            return View("Guides", model);
        }
        static List<string> ultimo = new List<string>();
        public ActionResult Json()
        {
            string intento;
            string docPath =
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string final = docPath + "\\Segundo año\\Primer Ciclo\\Estructura de Datos I Lab\\Lab4\\Archivo de Lectura";
            List<string> entrar = new List<string>();
            foreach (Inventario inventario in lista)
            {
                intento = Newtonsoft.Json.JsonConvert.SerializeObject(inventario);
                entrar.Add(intento);
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(final, "JsonSerialización.txt")))
            {
                foreach (string line in entrar)
                    outputFile.WriteLine(line);
            }
            if (ultimo.Count == 0)
            {
                ultimo = entrar;
            }
            return View();
        }
    }
}