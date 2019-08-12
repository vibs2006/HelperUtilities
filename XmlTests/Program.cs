using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace XmlTests
{
    public static class util
    {
        public static T ParseXml<T>(this string value) where T : class
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textReader = new StringReader(value))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }

    class Program
    {
      

        static void Main(string[] args)
        {
            var path = Directory.GetCurrentDirectory() + @"\xmldata2.xml";
            var xmlInputData = File.ReadAllText(path);

            //Customer customer =util.ParseXml<Customer>(xmlInputData);
           // object customer = util.ParseXml<object>(xmlInputData);
            XDocument doc = XDocument.Parse(xmlInputData); //or XDocument.Load(path)

            string jsonText = JsonConvert.SerializeXNode(doc);

            dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

            Console.ReadLine();
        }
    }


    
    public class Product
    {
    
        public string ProductID { get; set; }
    
        public string ProductName { get; set; }
    
        public string SupplierID { get; set; }
    
        public string CategoryID { get; set; }
    
        public string QuantityPerUnit { get; set; }
    
        public string UnitPrice { get; set; }
        
        public string UnitsInStock { get; set; }
        
        public string UnitsOnOrder { get; set; }
        
        public string ReorderLevel { get; set; }
        
        public string Discontinued { get; set; }
    }

    
    public class Order_Detail
    {
    
        public string OrderID { get; set; }
    
        public string ProductID { get; set; }
        
        public string UnitPrice { get; set; }
        
        public string Quantity { get; set; }
        
        public string Discount { get; set; }
        
        public Product Product { get; set; }
    }

    
    public class Order_Details
    {
        public List<Order_Detail> Order_Detail { get; set; }
    }

    
    public class Order
    {
    
        public string OrderID { get; set; }
    
        public string CustomerID { get; set; }
    
        public string EmployeeID { get; set; }
    
        public string OrderDate { get; set; }
    
        public string RequiredDate { get; set; }
        
        public string ShippedDate { get; set; }
        
        public string ShipVia { get; set; }
        
        public string Freight { get; set; }
        
        public string ShipName { get; set; }
        
        public string ShipAddress { get; set; }
        
        public string ShipCity { get; set; }
        
        public string ShipPostalCode { get; set; }
        
        public string ShipCountry { get; set; }
        
        public Order_Details Order_Details { get; set; }
    }

    
    public class Orders
    {    
        public List<Order> Order { get; set; }
        public Orders()
        {
            Order = new List<Order>();
        }
    }
    
    public class Customer
    {
     
        public string CustomerID { get; set; }
     
        public string CompanyName { get; set; }
     
        public string ContactName { get; set; }
     
        public string ContactTitle { get; set; }
     
        public string Address { get; set; }
        
        public string City { get; set; }
        
        public string PostalCode { get; set; }
        
        public string Country { get; set; }
        
        public string Phone { get; set; }
        
        public string Fax { get; set; }
        
        public Orders Orders { get; set; }
    }
}
