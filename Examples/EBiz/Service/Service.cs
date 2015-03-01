using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using XData;
using Service.Common;
using Service.EBiz;
using Service.WebApi;

namespace Service.Common {
    partial class Money {
        public Money() { }
        public Money(string kind, decimal value) {
            AT_Kind = kind;
            Children = value;
        }
        public decimal GetValueAsUSD() {
            decimal value = Children;
            switch (AT_Kind.Value) {
                case MoneyKind.E_USD:
                    return value;
                case MoneyKind.E_EUR:
                    return decimal.Round(value * 1.12M, 2);
                case MoneyKind.E_CNY:
                    return decimal.Round(value * 0.16M, 2);
                default: throw new ArgumentException();
            }
        }
    }
}

namespace Service.EBiz {
    partial class Contact {
        protected Contact() { }
        protected Contact(int id, string name, string email, DateTimeOffset regDate,
            IEnumerable<Phone> phones, NormalAddress normalAddress, SpatialAddress spatialAddress) {
            AT_Id = id;
            AT_Name = name;
            AT_Email = email;
            AT_RegDate = regDate;
            EnsureC_PhoneList().AddRange(phones, (item, phone) => item.Type = phone);
            if (normalAddress != null) {
                EnsureC_Address().CT_NormalAddress = normalAddress;
            }
            else {
                EnsureC_Address().CT_SpatialAddress = spatialAddress;
            }
        }
    }
    partial class Customer {
        public Customer() { }
        public Customer(int id, string name, string email, DateTimeOffset regDate,
            IEnumerable<Phone> phones, NormalAddress normalAddress, SpatialAddress spatialAddress,
            string reputation, params Order[] orders)
            : base(id, name, email, regDate, phones, normalAddress, spatialAddress) {
            AT_Reputation = reputation;
            if (orders != null) {
                EnsureC_OrderList().AddRange(orders, (item, order) => item.Type = order);
            }
        }
    }
    partial class Supplier {
        public Supplier() { }
        public Supplier(int id, string name, string email, DateTimeOffset regDate,
            IEnumerable<Phone> phones, NormalAddress normalAddress, SpatialAddress spatialAddress,
            string bankAccount, IEnumerable<int> productIds)
            : base(id, name, email, regDate, phones, normalAddress, spatialAddress) {
            AT_BankAccount = bankAccount;
            if (productIds != null) {
                AT_ProductIdList = new PositiveInt32List().AddRange(productIds.Select(i => (PositiveInt32)i));
            }
        }
    }
    partial class Order {
        public Order() { }
        public Order(int id, DateTimeOffset orderDate, bool? urgent, params OrderDetail[] details) {
            AT_Id = id;
            AT_OrderDate = orderDate;
            if (urgent != null) {
                AT_Urgent = urgent.Value;
            }
            decimal totalPrice = 0;
            var orderDetailList = EnsureC_OrderDetailList();
            foreach (var detail in details) {
                orderDetailList.CreateAndAddItem().Type = detail;
                totalPrice += ((int)detail.AT_Quantity) * detail.CT_UnitPrice.GetValueAsUSD();
            }
            CT_TotalPrice = new Money { AT_Kind = MoneyKind.E_USD, Children = totalPrice };
        }
    }
}

namespace Service.WebApi {
    partial class Contacts {
        public Contacts() { }
        public Contacts(IEnumerable<Contact> contacts) {
            if (contacts != null) {
                var type = new ContactsType();
                type.EnsureC_ContactList().AddRange(contacts, (item, contact) => item.Type = contact);
                Type = type;
            }
        }
    }
    partial class Products {
        public Products() { }
        public Products(IEnumerable<Product> products) {
            if (products != null) {
                var type = new ProductsType();
                type.EnsureC_ProductList().AddRange(products, (item, product) => item.Type = product);
                Type = type;
            }
        }
    }
}

public static class WebApiSimulation {
    private static readonly Contact[] _contacts = new Contact[] {
        new Customer(1, "Tank", "tank@example.com", DateTimeOffset.UtcNow - TimeSpan.FromDays(30), 
            new Phone[] {
                new Phone { AT_Kind= PhoneKind.E_Home, Children="12345678" }
            },
            new NormalAddress { AT_Country="China", AT_State="Sichuan", AT_City="Suining", AT_Address="somewhere", AT_ZipCode="629000" }, null,
            Reputation.E_Bronze,
            new Order(1, DateTimeOffset.UtcNow - TimeSpan.FromHours(235), true,
                new OrderDetail { AT_ProductId = 1, AT_Quantity = 1, CT_UnitPrice =new Money(MoneyKind.E_CNY, 3798.99M) },
                new OrderDetail { AT_ProductId = 2, AT_Quantity = 1, CT_UnitPrice =new Money(MoneyKind.E_CNY, 5600M) },
                new OrderDetail { AT_ProductId = 3, AT_Quantity = 2, CT_UnitPrice =new Money(MoneyKind.E_CNY, 199.99M) }
                ),
            new Order(2, DateTimeOffset.UtcNow - TimeSpan.FromHours(46), true,
                new OrderDetail { AT_ProductId = 3, AT_Quantity = 1, CT_UnitPrice =new Money(MoneyKind.E_CNY, 200M) }
                )
            ),
        new Customer(2, "Mike", "mike@example.com", DateTimeOffset.UtcNow - TimeSpan.FromDays(410), 
            new Phone[] {
                new Phone { AT_Kind= PhoneKind.E_Work, Children="23456789" },
            },
            null, new SpatialAddress{AT_Longitude=102.345M, AT_Latitude = 33.543M},
            Reputation.E_Gold
            ),
        new Supplier(3, "Eric", "eric@example.com", DateTimeOffset.UtcNow - TimeSpan.FromDays(556), 
            new Phone[] {
                new Phone { AT_Kind= PhoneKind.E_Work, Children="34567890" },
                new Phone { AT_Kind= PhoneKind.E_Home, Children="45678901" },
            },
            null, new SpatialAddress{ AT_Longitude = -113.567M, AT_Latitude = 45.218M},
            "22334455667788", new int[]{1,2,3}
            ),
    };
    private static readonly Product[] _products = new Product[]{
        new SportEquipment { AT_Id = 1, AT_Name = "Mountain bike", AT_StockQuantity= 10, 
            CT_Price=new Money(MoneyKind.E_CNY, 3998M),
            CT_Image=new Image{AT_Mime = "image/jpeg", Children= new byte[] { 1, 2, 3, 4, 5 }},
            AT_Applicability = "adults only!"
        },
        new SportEquipment { AT_Id = 2, AT_Name = "Road bike", AT_StockQuantity= 5, 
            CT_Price=new Money(MoneyKind.E_CNY, 5700M),
            CT_Image=new Image{AT_Mime = "image/jpeg", Children= new byte[] { 2, 3, 4, 5, 6}},
            AT_Applicability = "adults only!"
        },
        new Shoe { AT_Id = 3, AT_Name = "Outdoor shoe", AT_StockQuantity= 99, 
            CT_Price=new Money(MoneyKind.E_CNY, 250M),
            CT_Image=new Image{AT_Mime = "image/png", Children= new byte[] { 3, 4, 5, 6, 7}},
            AT_Gender = Gender.E_Man,
            CT_Size =new ShoeSize{AT_Unit = ShoeUnit.E_CM, Children = 26.5M}
        },

    };

    [Conditional("DEBUG")]
    private static void Validate(XObject obj, DiagContext ctx) {
        if (!obj.TryValidate(ctx)) {
            //if validation fails, this means the program has bugs
            DumpAndAssert(ctx);
        }
    }
    private static void Save(XGlobalElement element, string path) {
        using (var writer = new StreamWriter(path)) {
            element.Save(writer, "    ", "\r\n");
        }
    }
    private static void DumpAndAssert(DiagContext ctx) {
        foreach (var diag in ctx) {
            Console.WriteLine(diag.ToString());
        }
        Debug.Assert(false);
    }
    public static string GetContacts() {
        var contacts = new Contacts(_contacts);
        Validate(contacts, new DiagContext());
        Save(contacts, "Contacts.txt");
        return "Contacts.txt";
    }
    public static string GetProducts() {
        var products = new Products(_products);
        Validate(products, new DiagContext());
        Save(products, "Products.txt");
        return "Products.txt";
    }
}
