using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab4.Models
{
    public class Inventario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripción { get; set; }
        public string Casaproductora { get; set; }
        public string Precio { get; set; }
        public int Cantidad { get; set; }
        public Guid Guid { get; set; }
    }
}