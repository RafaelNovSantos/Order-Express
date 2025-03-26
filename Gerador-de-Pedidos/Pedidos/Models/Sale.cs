using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerador_de_Pedidos.Pedidos.Models
{
    public class Sale : Order
    {
   
        public string Seller { get; set; }

        public Sale()
        {
        }

        public Sale(int codProduct, string nameProduct, double valueProduct, int quantityProduct, string versionproduct, double shippingCost, string seller) : base(codProduct, nameProduct, valueProduct, quantityProduct, versionproduct, shippingCost)
        {
            Seller = seller;
        }
    }
}
