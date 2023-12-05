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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        private ILogger _logger;


        public ApplicationUserRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }

    }
}
