using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gerador_de_Pedidos.Pedidos.Models.Enums;

namespace Gerador_de_Pedidos.Pedidos.Models
{
    public class Warranty : Order
    {
        public string EquipamentDefect { get; set; }
        public string EquipmentSerialNumber { get; set; }
        public int NumberInvoice { get; set; }
        public int KeyExternalInvoice { get; set; }
        public TypeInvoice Invoice { get; set; }
        public Warranty(int codProduct, string nameProduct) { }

        public Warranty(int codProduct, string nameProduct, double valueProduct, int quantityProduct, string versionproduct, double shippingCost, string equipamentDefect, string equipmentSerialNumber, int numberInvoice, TypeInvoice invoice) : base(codProduct, nameProduct, valueProduct, quantityProduct, versionproduct, shippingCost)
        {
            Invoice = invoice;
            EquipamentDefect = equipamentDefect;
            NumberInvoice = numberInvoice;
        }
    }
}
