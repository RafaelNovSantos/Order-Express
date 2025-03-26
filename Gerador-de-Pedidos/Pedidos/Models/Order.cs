using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerador_de_Pedidos.Pedidos.Models
{
    public abstract class Order
    {
        public int CodProduct { get; set; }
        public string NameProduct { get; set; }
        public double ValueProduct { get; set; }
        public int QuantityProduct { get; set; }
        public string Versionproduct { get; set; }
        public double ShippingCost { get; set; }

        public Order()
        {
        }

        protected Order(int codProduct, string nameProduct, double valueProduct, int quantityProduct, string versionproduct, double shippingCost)
        {
            CodProduct = codProduct;
            NameProduct = nameProduct;
            ValueProduct = valueProduct;
            QuantityProduct = quantityProduct;
            Versionproduct = versionproduct;
            ShippingCost = shippingCost;
        }
    }
}
