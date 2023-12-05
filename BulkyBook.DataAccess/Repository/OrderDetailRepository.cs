using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository.IRepository;
using F2Play.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2Play.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;

        private readonly ILogger _logger;

        public OrderDetailRepository
(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }





        public void Update(OrderDetail obj)
        {
            _db.OrderDetail.Update(obj);
        }
    }
}
