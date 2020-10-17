using System;
using System.Collections.Generic;
using static System.Environment;

//Exception List
// 1 - Bad initialization
// 2 - Product not in stock
// 3 - Price < 0
// 4 - Shop not available

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Some tests
            Product chocolate = new Product("Milk chocolate");
            Product bread = new Product("White bread");
            Product jacket = new Product("Denim jacket");
            Product picture = new Product("Wall decor");
            Product skateboard = new Product("Blue skateboard");
            Product pencils = new Product("Colored pencils");
            Product canvas = new Product("Blank canvas");
            Product phonecase = new Product("Phonecase");
            Product teapot = new Product("Teapot");
            Product socks = new Product("Pair of socks");

            ShoppingAisle test = new ShoppingAisle();
            test.AddShop("Target", "8950 Charleston Blvd");
            test.AddShop("Auchan", "Borovaya 47");
            test.AddShop("Aliexpress", "969 West Wen Yi Road");
            test.PrintShops();
            
            test.AddProduct(0, chocolate.GetId(), chocolate, 300, 69.99);
            test.AddProduct(0, picture.GetId(), picture, 90, 150.0);
            test.AddProduct(0, skateboard.GetId(), skateboard, 20, 5699.99);
            test.AddProduct(0, pencils.GetId(), pencils, 300, 200.0);
            test.AddProduct(0, canvas.GetId(), canvas, 100, 259.99);
            test.AddProduct(0, jacket.GetId(), jacket, 10, 6999.99);
            test.AddProduct(1, bread.GetId(), bread, 50, 30.0);
            test.AddProduct(1, chocolate.GetId(), chocolate, 120, 58.99);
            test.AddProduct(1, pencils.GetId(), pencils, 300, 150.99);
            test.AddProduct(1, canvas.GetId(), canvas, 100, 499.99);
            test.AddProduct(1, phonecase.GetId(), phonecase, 100, 400.0);
            test.AddProduct(2, jacket.GetId(), jacket, 120, 1080.35);
            test.AddProduct(2, phonecase.GetId(), phonecase, 300, 320.01);
            test.AddProduct(2, teapot.GetId(), teapot, 320, 540.0);
            test.AddProduct(2, skateboard.GetId(), skateboard, 50, 2500.0);
            test.AddProduct(2, socks.GetId(), socks, 450, 220.0);


            var cart = new Dictionary<Product, int>
            {
                [chocolate] = 1,
                [canvas] = 2
            };


            //Should output Target
            Console.WriteLine(test.FindMin(cart).GetName());

            PrintCart(test.GetCart(1, 400.0));

        }
        //Utility method
        public static void PrintCart(Dictionary<Product, int> cart)
        {
            foreach(KeyValuePair<Product, int> entry in cart)
            {
                Console.WriteLine(entry.Key.GetName() + " = " + entry.Value);
            }
        }
    }

    public class ShoppingAisle
    {
        IDictionary<int, Shop> shops = new Dictionary<int, Shop>();

        public void PrintShops()
        {
            foreach(KeyValuePair<int, Shop> entry in shops)
            {
                Console.WriteLine(entry.Value.GetName() + " " + entry.Value.GetId());
            }
        }
        public int AddShop(string name, string address)
        {
            Shop shop = new Shop(name, address);
            try
            {
                
                this.shops.Add(shop.GetId(), shop);
                
            }
            catch (Exception ex)
            {
                throw new LabException("This shop already exists!");
            }
            return shop.GetId();
        }
        public bool ShopAvailable(int id)
        {
            if (shops.ContainsKey(id))
            {
                return true;
            }
            return false;
        }
        public void AddProduct(int id, int id2, Product product, int amount, double pricePerUnit)
        {
            try
            {
                if (ShopAvailable(id))
                {
                    shops[id].AddProduct(id2, product, amount, pricePerUnit);
                }
                else
                {
                    throw new LabException("No such shop available");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(4);
            }
        }
        //Finds Shop with the lowest prices
        public Shop FindMin(int id)
        {
            double minPrice = double.MaxValue;
            Shop foundShop = new Shop("default", "default");
            int shopId = foundShop.GetId();
            foreach(KeyValuePair<int, Shop> entry in shops)
            {
                if (entry.Value.Available(id))
                {
                    if (entry.Value.GetUnitPrice(id) < minPrice)
                    {
                        minPrice = entry.Value.GetUnitPrice(id);
                        foundShop = entry.Value;
                    }
                }
            }
            try
            {
                if (foundShop.GetId() == shopId)
                {
                    throw new LabException("No such shop found: FindMin for Product: " + id);
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(5);
            }
            return foundShop;
        }
        public Shop FindMin(Dictionary<Product, int> cart)
        {
            double minPrice = double.MaxValue;
            Shop foundShop = new Shop("default", "default");
            int id = foundShop.GetId();
            foreach (KeyValuePair<int, Shop> entry in shops)
            {
                if (entry.Value.InStock(cart))
                {
                    if (entry.Value.EstimatePrice(cart) < minPrice)
                    {
                        minPrice = entry.Value.EstimatePrice(cart);
                        foundShop = entry.Value;
                    }
                }
            }
            try
            {
                if (foundShop.GetId() == id)
                {
                    throw new LabException("No such shop found: FindMin(cart)");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(5);
            }
            return foundShop;
        }
        //Finds what to buy for a specific price in the given shop
        public Dictionary<Product, int> GetCart(int id, double price)
        {
            
            
            try
            {
                if (ShopAvailable(id))
                {
                    shops[id].GetCart(price);
                }
                else
                {
                    throw new LabException("No such shop available");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(4);
            }
            
            return shops[id].GetCart(price);
        }
        //Buys the given amount of productsin the cart
        public double Buy(int id, Dictionary<Product, int> cart)
        {
            try
            {
                if (ShopAvailable(id))
                {
                    if (shops[id].InStock(cart))
                    {
                        shops[id].Buy(cart);
                    }
                    else
                    {
                        throw new LabException("Some products from the cart are not in Stock");
                    }

                }
                else
                {
                    throw new LabException("No such shop available");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(4);
            }
            
            
            return -1; //Not in Stock
        }
        //Modifies the UnitPrice of the given product
        public void SetUnitPrice(int id, double price)
        {
            try
            {
                if (ShopAvailable(id))
                {
                    shops[id].SetUnitPrice(id, price);
                }
                else
                {
                    throw new LabException("No such shop available");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(4);
            }
        }
    }

    public class Shop
    {
        private readonly int id;
        private readonly string name;
        private readonly string address;
        private static int idCounter = 0;
        
        IDictionary<Product, Properties> stockProducts = new Dictionary<Product, Properties>();
        IDictionary<int, Properties> stock = new Dictionary<int, Properties>();

        public Shop(string name, string address)
        {
            this.id = Shop.idCounter++;
            this.name = name;
            this.address = address;
        }
        
        public bool InStock(int id, int amount)
        {
            if (Available(id))
            {
                if (this.stock[id].GetAmount() >= amount)
                {
                    return true;
                }
            }
            return false;
        }
        public bool InStock(Dictionary<Product, int> cart)
        {
            foreach (KeyValuePair<Product, int> entry in cart)
            {
                if (! this.InStock(entry.Key.GetId(), entry.Value))
                {
                    return false;
                }
            }
            return true;
        }
        public bool Available(int id)
        {
            if (this.stock.ContainsKey(id))
            {
                return true;
            }
            return false;
        }
        public void AddProduct(int id, Product product, int amount, double pricePerUnit)
        {
            if (Available(id))
            {
                this.stockProducts[product].SetUnitPrice(pricePerUnit);
                this.stockProducts[product].SetAmount(amount);
                this.stock[id].SetUnitPrice(pricePerUnit);
                this.stock[id].SetAmount(amount);
            }
            else
            {
                this.stock.Add(id, new Properties(amount, pricePerUnit));
                this.stockProducts.Add(product, new Properties(amount, pricePerUnit));
            }
        }
        public double GetUnitPrice(int id)
        {
            try
            {
                if (Available(id))
                {
                    return stock[id].GetUnitPrice();
                }
                else
                {
                    throw new LabException("GetUnitPrice error: Product " + id + " not in Stock");
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(2);
            }
            return -1.0;
        }
        public void SetUnitPrice(int id, double price)
        {
            try
            {
                if (price < 0)
                {
                    throw new LabException("Price can't be set < 0. Product: " + id + ", Shop: " + this.name);
                }
                if (Available(id))
                {
                    stock[id].SetUnitPrice(price);
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(3);
            }
            
        }
        public double EstimatePrice(Dictionary<Product, int> cart)
        {
            double price = 0.0;
            try
            {
                if (!this.InStock(cart))
                {
                    throw new LabException("Some products from the cart are not in Stock in " + this.name);
                }
                foreach (KeyValuePair<Product, int> entry in cart)
                {
                    price += stock[entry.Key.GetId()].CalcPrice(entry.Value);
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(2);
            }
            
            return price;
        }
        public Dictionary<Product, int> GetCart(double price)
        {
            var cart = new Dictionary<Product, int>();
            foreach (KeyValuePair<Product, Properties> entry in stockProducts)
            {
                cart.Add(entry.Key, entry.Value.GetMaxAmount(price));
            }
            return cart;
        }
        public double Buy(Dictionary<Product, int> cart)
        {
            double price = 0.0;
            try
            {
                if (!this.InStock(cart))
                {
                    throw new LabException("Some products from the cart are not in Stock in " + this.name);
                }
                foreach (KeyValuePair<Product, int> entry in cart)
                {
                    price += stock[entry.Key.GetId()].Buy(entry.Value);
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(2);
            }
            
            return price;
        }
        public int GetId()
        {
            return this.id;
        }
        public string GetName()
        {
            return this.name;
        }
        private class Properties
        {
            private int amount;
            private double pricePerUnit;
            public Properties(int amount, double pricePerUnit)
            {
                try
                {
                    if (amount < 0 || pricePerUnit < 0)
                    {
                        throw new LabException("Bad initialization in Properties amount: " + amount + ",  unitPrice: " + pricePerUnit);
                    }
                    this.amount = amount;
                    this.pricePerUnit = pricePerUnit;
                }
                catch (LabException exception)
                {
                    exception.PrintExit(1);
                }
            }
            public double CalcPrice(int amount)
            {
                return this.pricePerUnit * Convert.ToDouble(amount);
            }
            public int GetAmount()
            {
                return this.amount;
            }
            public double GetUnitPrice()
            {
                return this.pricePerUnit;
            }
            public void SetUnitPrice(double price)
            {
                this.pricePerUnit = price;
            }
            public void SetAmount(int amount)
            {
                this.amount = amount;
            }
            public int GetMaxAmount(double price)
            {
                int amount = Convert.ToInt32(Math.Floor(price / this.pricePerUnit));
                if (amount < this.amount)
                {
                    return amount;
                }
                else
                {
                    return this.amount;
                }
            }
            public double Buy(int amount)
            {
                this.amount -= amount;
                return this.pricePerUnit * Convert.ToDouble(amount);
            }
        }
        public void Print()
        {
            foreach(KeyValuePair<Product, Properties> entry in stockProducts)
            {
                Console.WriteLine("Product: " + entry.Key.GetName() + ", val: " + entry.Value.GetUnitPrice());
            }
        }
        
    }

    public class Product
    {
        private readonly int id;
        private readonly string name;
        private static int idCount = 0;
        public Product(string name)
        {
            try
            {
                if (name == "")
                {
                    throw new LabException("Bad initialization in Product, name: " + name);
                }
                this.id = idCount++;
                this.name = name;
            }
            catch (LabException exeption)
            {
                exeption.PrintExit(1);
            }
        }
        public string GetName()
        {
            return this.name;
        }
        public int GetId()
        {
            return this.id;
        }
    }

    public class LabException : Exception
    {
        public LabException(string msg)
        : base(msg)
        {
        }
        public void PrintExit(int code)
        {
            Console.WriteLine(this.Message);
            Exit(code);
        }
    }
}
