using PrpProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrpProject.Controllers
{
    public class HomeController : Controller
    {
        ProjectContext context = new ProjectContext();

        static User user;
        static Wallet wallet;
        static MoneyManagerItem item;

        public ActionResult Index()
        {
            ForViewBug();
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Login, string Password)
        {
            // получаем из бд все объекты User
            IEnumerable<User> users = context.Users;
            //List<User> us = context.Users.ToList<User>();
            // передаем все полученный объекты в динамическое свойство Users в ViewBag
            ViewBag.Users = users;
            foreach (var items in ViewBag.Users)
            {
                if (items.Login == Login && items.Password == Password)
                {
                    user = items;

                    ForViewBug();

                    return View("Index");    
                }
            }
            // возвращаем представление
            return View("Login");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(string name, string surname, string Login, string Password, string PasswordS)
        {
            IEnumerable<User> users = context.Users;
            ViewBag.Users = users;

            foreach (var items in ViewBag.Users)
            {
                if (items.Login == Login)
                    return View("Registration");
            }
            if (Password == PasswordS)
            {
                context.Users.Add(new User { Login = Login, Password = Password, Name = name, Surname = surname, Money = 0 });
                context.SaveChanges();

                ViewBag.RegistrationMessage = "Вы успешно зарегистрировались";
                foreach (var us in context.Users)
                {
                    if (us.Login == Login)
                        user = us;
                }
                AddInitialItems(user.Id);
                return View();
            }
            else
            {
                ViewBag.RegistrationMessage = "Пароли не совпадают";
                return View("Registration");
            }
        }

        public void AddInitialItems(int userId)
        {
            wallet = new Wallet();
            wallet.Name = "Wallet";
            wallet.UserId = userId;
            wallet.Money = 0;
            context.Wallets.Add(wallet);
            wallet = new Wallet();
            wallet.Name = "Bank Account";
            wallet.UserId = userId;
            wallet.Money = 0;
            context.Wallets.Add(wallet);

            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = true;
            item.Name = "Income";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);

            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Groceries";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Transport";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Shopping";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Eating outside";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "House";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Entertainment";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);
            item = new MoneyManagerItem();
            item.UserId = userId;
            item.State = false;
            item.Name = "Services";
            item.Balance = 0;
            context.MoneyManagerItems.Add(item);

            context.SaveChanges();

        }

        public ActionResult AddIncome()
        {
            ForViewBug();
            return View();
        }
        [HttpPost]
        public ActionResult AddIncome(string name)
        {
            MoneyManagerItem item = new MoneyManagerItem();
            item.Name = name;
            item.State = true;
            item.UserId = user.Id;
            context.MoneyManagerItems.Add(item);
            context.SaveChanges();

            ForViewBug();

            return View("Index");
        }

        public ActionResult AddWallet()
        {
            ForViewBug();
            return View();
        }

        [HttpPost]
        public ActionResult AddWallet(string name)
        {
            Wallet item = new Wallet();
            item.Name = name;
            item.UserId = user.Id;
            item.Money = 0;
            context.Wallets.Add(item);
            context.SaveChanges();

            ForViewBug();

            return View("Index");
        }

        public ActionResult AddRashod()
        {
            ForViewBug();
            return View();
        }
        [HttpPost]
        public ActionResult AddRashod(string name)
        {
            MoneyManagerItem item = new MoneyManagerItem();
            item.Name = name;
            item.State = false;
            item.UserId = user.Id;
            context.MoneyManagerItems.Add(item);
            context.SaveChanges();

            ForViewBug();

            return View("Index");
        }

        public ActionResult Operation(int id)
        {
            item = (MoneyManagerItem)context.MoneyManagerItems.FirstOrDefault(item => item.Id == id);
            ViewBag.Income = context.MoneyManagerItems.Where(item => item.Id == id);
            if (item.State)
            {
                ViewBag.Operation = "доход";
            }
            else
            {
                ViewBag.Operation = "расход";
            }
            ForViewBug();
            return View();
        }

        [HttpPost]
        public ActionResult Operation(string select, string income)
        {
            int id = Convert.ToInt32(select);
            wallet = context.Wallets.FirstOrDefault(wal => wal.Id == id);
            if (item.State)
            {
                user.Money += Convert.ToInt32(income);
                wallet.Money += Convert.ToInt32(income);
                item.Balance += Convert.ToInt32(income);
            }
            else
            {
                user.Money -= Convert.ToInt32(income); 
                wallet.Money -= Convert.ToInt32(income);
                item.Balance -= Convert.ToInt32(income);
            }
            foreach (var b in context.Users)
            {
                if (b.Id == user.Id)
                    b.Money = user.Money;
            }
            foreach (var b in context.MoneyManagerItems.Where(i => i.UserId == user.Id))
            {
                if (b.Id == item.Id)
                    b.Balance = item.Balance;
            }

            Hystory hystory = new Hystory();
            hystory.MoneyManagerItemId = item.Id;
            hystory.Name = item.Name;
            hystory.Operation = item.State ? "+" + income : "-" + income;
            hystory.UserId = user.Id;
            hystory.Date = DateTime.Now;
            hystory.WalletName = wallet.Name;

            context.Hystories.Add(hystory);
            context.SaveChanges();

            ForViewBug();

            return View("Index");
        }

        public ActionResult WalletOperation(int id)
        {
            foreach (var wal in context.Wallets.Where(w => w.UserId == user.Id))
            {
                if (wal.Id == id)
                    wallet = wal;
            }
            return View(wallet);
        }

        [HttpPost]
        public ActionResult WalletOperation(string money)
        {
            wallet.Money = Convert.ToDecimal(money);

            foreach (var wal in context.Wallets.Where(w => w.UserId == user.Id))
            {
                if (wal.Id == wallet.Id)
                    wal.Money = wallet.Money;
            }
            context.SaveChanges();
            ForViewBug();
            return View("Index");

        }

        public void ForViewBug()
        {
            ViewBag.Hystory = context.Hystories.Where(hys => hys.UserId == user.Id);
            ViewBag.Items = context.MoneyManagerItems.Where(it => it.UserId == user.Id);
            ViewBag.Wallets = context.Wallets.Where(wal => wal.UserId == user.Id);
            ViewBag.User = user;
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult ManagerCategories()
        {
            ViewBag.Items = context.MoneyManagerItems.Where(it => it.UserId == user.Id);
            return View();
        }

        public ActionResult DeleteWallet(int id)
        {
            foreach (var wal in context.Wallets.Where(w => w.UserId == user.Id))
            {
                if(wal.Id == id)
                    context.Entry(wal).State = System.Data.Entity.EntityState.Deleted;
            }
            context.SaveChanges();
            ForViewBug();
            return View("ManagerWallets");
        }

        public ActionResult EditWallet(int id)
        {
            wallet = context.Wallets.FirstOrDefault(it => it.Id == id);
            return View(wallet);
        }

        [HttpPost]
        public ActionResult EditWallet(string name)
        {
            foreach (var category in context.Wallets.Where(it => it.UserId == user.Id))
            {
                if (category.Id == wallet.Id)
                {
                    category.Name = name;
                }
            }
            context.SaveChanges();
            ViewBag.Items = context.MoneyManagerItems.Where(it => it.UserId == user.Id);
            ForViewBug();
            return View("ManagerWallets");
        }

        public ActionResult DeleteCategory(int id)
        {
            foreach (var item in context.Hystories.Where(it => it.MoneyManagerItemId == id))
            {
                context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
            }
            foreach (var item in context.MoneyManagerItems.Where(it => it.UserId == user.Id))
            {
                if (item.Id == id)
                {
                    context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }
            }
            context.SaveChanges();
            ViewBag.Items = context.MoneyManagerItems.Where(it => it.UserId == user.Id);
            return View("ManagerCategories");
        }

        public ActionResult EditCategory(int id)
        {
            ViewBag.Item = context.MoneyManagerItems.FirstOrDefault(it => it.Id == id);
            item = context.MoneyManagerItems.FirstOrDefault(it => it.Id == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult EditCategory(string name)
        {
            foreach (var category in context.MoneyManagerItems.Where(it => it.UserId == user.Id))
            {
                if (category.Id == item.Id)
                {
                    category.Name = name;
                }
            }
            context.SaveChanges();
            ViewBag.Items = context.MoneyManagerItems.Where(it => it.UserId == user.Id);
            return View("ManagerCategories");
        }

        public ActionResult ManagerWallets()
        {
            ForViewBug();
            return View();
        }

        public ActionResult ManagerAccount()
        {
            return View(user);
        }
        [HttpPost]
        public ActionResult ManagerAccount(string name, string surname, string login, string oldPassword, string newPassword, string newPasswordS)
        {
            foreach (var us in context.Users.Where(it => it.Id == user.Id))
            {
                if (name != null)
                    us.Name = name;
                if (surname != null)
                    us.Surname = surname;
                if (login != null)
                    us.Login = login;
                if (oldPassword != null && newPassword != null && newPasswordS != null && newPassword == newPasswordS)
                    us.Password = newPassword;
                user = us;
            }
            context.SaveChanges();
            return View("Settings");
        }

        public ActionResult History()
        {
            List<Hystory> list = context.Hystories.Where(his => his.UserId == user.Id).ToList<Hystory>();
            list.Reverse();
            ViewBag.Histories = list;
            return View();
        }
    }
}