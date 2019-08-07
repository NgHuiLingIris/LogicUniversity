using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogicUniversity.Context;
using LogicUniversity.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;



namespace LogicUniversity.Controllers
{
    public class PurchaseOrderController : Controller
    {
        LogicUniversityContext db = new LogicUniversityContext();

        public ActionResult Index()
        {
            Order order = new Order
            {
                POnumber = 10000,
                DateOrdered = DateTime.Now,
                DateDelivery = DateTime.Now.AddDays(3)
            };
            db.Orders.Add(order);
            db.SaveChanges();
            return View();
        }

        public async Task<ActionResult> PlaceOrder(string supplier, List<PredictViewModel> predModel)
        {
            Supplier supplier1 = new Supplier();
            List<Products> products = new List<Products>();

            ViewData["SupplierName"] = supplier;

            //fetch largest PO number from database, and generate next for displaying 
            var maxPO = (from c in db.Orders
                         select c).Max(c => c.POnumber);
            maxPO++;
            ViewData["maxPO"] = maxPO;

            //fetch list of products from Supplier 
            var quest = from a in db.Suppliers
                        join b in db.Products
                        on a.SupplierId equals b.Supplier1
                        where a.SupplierName == supplier
                        select b;
            products = quest.ToList();
            supplier1.Products = products;

            using (var client = new HttpClient())
            {

                //Machine learning:
                predModel = new List<PredictViewModel>();
                foreach (Products p in products)
                {
                    PredictViewModel element = new PredictViewModel
                    {
                        ItemCode = int.Parse(p.ItemCode.Substring(1, p.ItemCode.Length - 1)),
                        Month = DateTime.Now.Month,
                    };
                    predModel.Add(element);
                }

                HttpResponseMessage res = await client.PostAsJsonAsync("http://127.0.0.1:5000/", predModel);

                if (res.IsSuccessStatusCode)
                {
                    // pass the result by setting the Viewdata property
                    // have to read as string for the data in response.body
                    ViewData["Message"] = res.Content.ReadAsStringAsync().Result;

                    //to deserialize the message in to string array
                    string message = res.Content.ReadAsStringAsync().Result;
                    Debug.WriteLine(message);
                    string[] split = message.Split(new Char[] { '"', '[', ']', ',' });
                    split = split.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    split = split.Where(x => x != "\n").ToArray();
                    Debug.WriteLine(split.Length);
                    List<string> outcomes = new List<string>();
                    foreach (string s in split)
                    {
                        Debug.WriteLine("it is: " + s);
                        outcomes.Add(s);
                    }

                    ViewData["outcomes"] = outcomes;
                    supplier1.Products = products;
                    return View(supplier1);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public ActionResult PlaceOrderSuccess(FormCollection form)
        {
            //fetch product list base on supplier name
            string supplier = form["Supplier"];
            var quest = from a in db.Suppliers
                        join b in db.Products
                        on a.SupplierId equals b.Supplier1
                        where a.SupplierName == supplier
                        select b;
            List<Products> AllProducts = quest.ToList();

            //generate a string list of user input on reorder quantity
            List<string> ReOrderQtyList = new List<string>();
            for (int i = 0; i < AllProducts.Count; i++)
            {
                ReOrderQtyList.Add(form["Products[" + i + "].ReorderQty"]);
            }

            List<ReorderResult> tempList = new List<ReorderResult>();
            for (int i = 0; i < AllProducts.Count; i++)
            {
                tempList.Add(new ReorderResult
                {
                    Description = AllProducts[i].Description,
                    UOM = AllProducts[i].UOM,
                    ReorderQty = double.Parse(ReOrderQtyList[i])
                });
            }
            //remove the products that have zero reorder quantity         
            tempList.RemoveAll(l => l.ReorderQty == 0);

            ViewData["Products"] = tempList;

            //update database
            Order order = new Order
            {
                SupplierCode = supplier,
                Status = "Pending",
                DateOrdered = DateTime.Now,
                DateDelivery = DateTime.Now.AddDays(3)
            };
            db.Orders.Add(order);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            for (int i = 0; i < AllProducts.Count; i++)
            {
                orderDetails.Add(new OrderDetail
                {
                    PONumber = order.POnumber,
                    ItemCode = AllProducts[i].ItemCode,
                    Quantity = double.Parse(ReOrderQtyList[i]),
                });
            }

            orderDetails.RemoveAll(l => l.Quantity == 0);
            db.OrderDetails.AddRange(orderDetails);

            db.SaveChanges();
            return View();
        }
    }

    //Temporary entity to store information on reordering
    public class ReorderResult
    {
        public string Description { get; set; }
        public string UOM { get; set; }
        public double ReorderQty { get; set; }
    }
}