using System;
using System.IO;
using System.Diagnostics;
using XData;
using Client.WebApi;

namespace Client.Common {
    partial class Money {
        public override string ToString() {
            return string.Format("{0} {1}", Children, AT_Kind);
        }
    }
    partial class ShoeSize {
        public override string ToString() {
            return string.Format("{0} {1}", Children, AT_Unit);
        }
    }

}

namespace Client.EBiz {
    partial class Contact {
        public virtual void Dump() {
            Console.WriteLine("\tId = {0}, Name = {1}, EMail = {2}, RegDate = {3}", AT_Id, AT_Name, AT_Email, AT_RegDate);
            foreach (var phone in C_PhoneList) {
                Console.WriteLine("\t{0} Phone = {1}", phone.Type.AT_Kind, phone.Type.Children);
            }
            var address = C_Address;
            if (address.C_NormalAddress != null) {
                var na = address.C_NormalAddress.Type;
                Console.WriteLine("\tCountry = {0}, State = {1}, City = {2}, Address = {3}",
                    na.AT_Country, na.A_State != null ? na.AT_State : null, na.AT_City, na.AT_Address);
            }
            else {
                var sa = address.C_SpatialAddress.Type;
                Console.WriteLine("\tLongitude = {0}, Latitude = {1}", sa.AT_Longitude, sa.AT_Latitude);
            }
        }
    }
    partial class Customer {
        public override void Dump() {
            Console.WriteLine("==Customer==");
            base.Dump();
            Console.WriteLine("\tReputation = {0}", AT_Reputation);
            var orderList = C_OrderList;
            if (orderList != null) {
                foreach (var orderMember in orderList) {
                    var order = orderMember.Type;
                    Console.WriteLine("\t\tOrder Id = {0}, TotalPrice = {1}", order.AT_Id, order.CT_TotalPrice);
                    foreach (var detailMember in order.C_OrderDetailList) {
                        var detail = detailMember.Type;
                        Console.WriteLine("\t\t\tDetail ProductId = {0}, UnitPrice = {1}, Quantity = {2}",
                            detail.AT_ProductId, detail.CT_UnitPrice, detail.AT_Quantity);
                    }
                }
            }
        }
    }
    partial class Supplier {
        public override void Dump() {
            Console.WriteLine("==Supplier==");
            base.Dump();
            Console.WriteLine("\tBankAccount = {0}, ProductIdList = {1}", AT_BankAccount,
                A_ProductIdList != null ? AT_ProductIdList : null);
        }
    }
    partial class Product {
        public virtual void Dump() {
            Console.WriteLine("\tId = {0}, Name = {1}, Price = {2}, StockQuantity = {3}", AT_Id, AT_Name, CT_Price, AT_StockQuantity);
        }
    }
    partial class SportEquipment {
        public override void Dump() {
            Console.WriteLine("==SportEquipment==");
            base.Dump();
        }
    }
    partial class Shoe {
        public override void Dump() {
            Console.WriteLine("==Shoe==");
            base.Dump();
            Console.WriteLine("\tGender = {0}, Size = {1}", AT_Gender, CT_Size);
        }
    }
}
static class Program {
    static void Main() {
        var contactListfilePath = WebApiSimulation.GetContactList();
        ContactList contactList;
        using (var reader = new StreamReader(contactListfilePath)) {
            var ctx = new DiagContext();
            if (!ContactList.TryLoadAndValidate(contactListfilePath, reader, ctx, out contactList)) {
                DumpAndAssert(ctx);
            }
        }
        foreach (var contactMember in contactList.Type.C_ContactList) {
            contactMember.Type.Dump();
        }
        //
        var productListfilePath = WebApiSimulation.GetProductList();
        ProductList productList;
        using (var reader = new StreamReader(productListfilePath)) {
            var ctx = new DiagContext();
            if (!ProductList.TryLoadAndValidate(productListfilePath, reader, ctx, out productList)) {
                DumpAndAssert(ctx);
            }
        }
        foreach (var productMember in productList.Type.C_ProductList) {
            productMember.Type.Dump();
        }
    }
    private static void DumpAndAssert(DiagContext ctx) {
        foreach (var diag in ctx) {
            Console.WriteLine(diag.ToString());
        }
        Debug.Assert(false);
    }

}
