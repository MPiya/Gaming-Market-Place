
using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository.IRepository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2Play.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private ILogger _logger;

        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; }

        public ICompanyRepository Company { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }


        public UnitOfWork(ApplicationDbContext db, ILogger logger)
        {

            _db = db;
            _logger = logger;


            Category = new CategoryRepository(_db, _logger);
            Product = new ProductRepository(_db, _logger);
            Company = new CompanyRepository(_db, _logger);
            ApplicationUser = new ApplicationUserRepository(_db, _logger);
            ShoppingCart = new ShoppingCartRepository(_db, _logger);
            OrderHeader = new OrderHeaderRepository(_db, _logger);
            OrderDetail = new OrderDetailRepository(_db, _logger);
        }


        //public IShoppingCartRepository ShoppingCart {  get; private set; }

        //public IApplicationUserRepository ApplicationUser {  get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
