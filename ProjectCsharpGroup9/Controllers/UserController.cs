﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectCsharpGroup9.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http;

namespace ProjectCsharpGroup9.Controllers
{
	public class UserController : Controller
	{
		AppDbContext _dbContext;
		HttpClient _client = new HttpClient();
		public UserController()
		{
			_dbContext = new AppDbContext();
		}
        public IActionResult SignUp() // view đăng ký
        {
            return View();
        }
        [HttpPost()]
		public ActionResult SignUp(User user) // action đăng ký
		{
			try
			{
                user.UserID = Guid.NewGuid();
                user.RoleID = 2;
                if (CalculateAge(user.BirthDay) < 15)
                {
                    ModelState.AddModelError("BirthDay", "Tuổi của bạn chưa đủ 15.");
                    return View(user);
                }
                if (_dbContext.Users.Any(u => u.UserName == user.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(user);
                }
                if (_dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(user);
                }
                if (_dbContext.Users.Any(u => u.PhoneNumber == user.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại.");
                    return View(user);
                }

                string Url = $@"https://localhost:7276/api/User/Create-User";
				var response = _client.PostAsJsonAsync(Url, user).Result;
				if (response.IsSuccessStatusCode)
				{
                    TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("Login");
                }
				else
				{
					return View();
				}

            }
			catch (Exception)
			{
				return BadRequest();
			}

		}
		public IActionResult Login(string username, string password) //action đăng nhập
		{
			if (username == null && password == null) { return View(); }
			else
			{
				var data = _dbContext.Users.FirstOrDefault(p => p.UserName == username && p.Password == password);
				if (data == null)
				{
					TempData["status"] = "Đăng nhập thất bại XD";
					return RedirectToAction("Login");
				}
				else
				{
                    var jsonData = JsonConvert.SerializeObject(data);
                    HttpContext.Session.SetString("user", jsonData);

                    // Redirect based on role
                    if (data.RoleID == 1)
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    else if (data.RoleID == 2)
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
						// Handle other roles or invalid roles
						return BadRequest();
                    }
                }
			}
		}
		public IActionResult LogOut() 
		{
            HttpContext.Session.Remove("user"); // Xóa thông tin đăng nhập từ session
            return RedirectToAction("Login");
        }
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
