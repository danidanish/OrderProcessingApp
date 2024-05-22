using OrderProcessingSystem.Models.Login;
using OrderProcessingSystem.Models.Products;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using Dapper;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using OrderProcessingSystem.Models.Orders;

namespace OrderProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Login()
        {
            if (Session["UserId"] == null)
            {
                ViewBag.Message = "Please Login!";
            }
            return View(new LoginViewModel());
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Error", "Username and password are required.");
                    return View(model);
                }
                if (IsValidUser(model.Username, model.Password))
                {
                    return RedirectToAction("Products", "Home");
                }
                else
                {
                    ModelState.AddModelError("Error", "Invalid username or password.");
                    return View(model);
                }
            }

            return View(model);
        }

        private bool IsValidUser(string username, string password)
        {
            List<Users> model = new List<Users>();
            string storedProcedureName = "sp_GetAllCustomers";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                model = connection.Query<Users>(storedProcedureName, commandType: CommandType.StoredProcedure).ToList();
            }
            foreach(var user in model)
            {
                if(username==user.Username && password == user.Password)
                {
                    Session["Username"] = user.Username;
                    Session["UserId"] = user.Id;
                    return true;
                }
            }
            return false;
        }
        public ActionResult Products()
        {
            List<Product> model = new List<Product>();
            string storedProcedureName = "sp_GetAllProducts";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                model = connection.Query<Product>(storedProcedureName, commandType: CommandType.StoredProcedure).ToList();
            }
            List<ProductViewModel> viewmodel = model.Select(u => new ProductViewModel
            {
                ProductPrice = u.Price,
                ProductName = u.Name,
                ProductId = u.Id
            }).ToList();
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult Products(List<ProductViewModel> OrderItems)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            OrderItems = OrderItems.FindAll(u => u.Quantity != 0);
            if (ModelState.IsValid)
            {
                string storedProcedureName = "sp_InsertOrder";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach(var param in OrderItems)
                    {
                        var parameters = new
                        {
                            ProductId = param.ProductId,
                            CustomerId = Session["UserId"],
                            Quantity = param.Quantity
                        };
                        connection.Query<ProductViewModel>(storedProcedureName, param: parameters, commandType: CommandType.StoredProcedure).ToList();
                    }
                }
                return RedirectToAction("OrderPlaced", "Home");
            }

            return View(OrderItems);
        }
        public ActionResult OrderPlaced()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<Orders> orders = new List<Orders>();
            string storedProcedureName = "sp_GetOrdersByCustomerId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var parameters = new
                {
                    CustomerId = Session["UserId"]
                };
                orders = connection.Query<Orders>(storedProcedureName, param: parameters, commandType: CommandType.StoredProcedure).ToList();
            }
            if (orders.Count==0)
            {
                ViewBag.Message = "Please Place the Order!";
            }
            else
            {
                ViewBag.Message = "Order Placed!!";
            }
            return View(orders);
        }
    }
}