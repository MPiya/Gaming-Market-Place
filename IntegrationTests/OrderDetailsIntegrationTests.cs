using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository;
using F2Play.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Transactions;
using Xunit;

namespace F2Play.Tests
{
    public class OrderDetailRepositoryIntegrationTests : IDisposable
    {
        private ApplicationDbContext _db;
        private OrderDetailRepository _orderDetailRepository;
        private string ConnectionString;
        private readonly ILogger _logger;
        private TransactionScope _transaction;

        public OrderDetailRepositoryIntegrationTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            ConnectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            _db = new ApplicationDbContext(options);
            _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            _orderDetailRepository = new OrderDetailRepository(_db, _logger);
            _db.Database.EnsureCreated();
            _transaction = new TransactionScope();
        }
        [Fact]
        public void AddOrderDetail_SavesToDatabase()
        {
            // Happy Case
            var applicationUser = new ApplicationUser
            {
                // Set properties for ApplicationUser
                Id = "user1", // Assuming this is a valid user Id
                Name = "John Doe",
                StreetAddress = "123 Main St",
                City = "ExampleCity",
                State = "CA",
                PostalCode = "12345",
                CompanyId = 1, // Assuming this is a valid company Id
                               // ... (set other properties as needed)
            };

            var orderHeader = new OrderHeader
            {
                // Set properties for OrderHeader
                ApplicationUser = applicationUser,
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now.AddDays(2),
                OrderTotal = 100.00,
                OrderStatus = "Processing",
                PaymentStatus = "Pending",
                TrackingNumber = "ABC123",
                Carrier = "UPS",
                PaymentDate = DateTime.Now,
                PaymentDueDate = DateTime.Now.AddDays(7),
                SessionId = "Session123",
                PaymentIntentId = "PaymentIntent123",
                PhoneNumber = "1234567890",
                StreetAddress = "123 Main St",
                City = "ExampleCity",
                State = "CA",
                PostalCode = "12345",
                Name = "John Doe",
                // ... (set other properties as needed)
            };

            var product = new Product
            {
                // Set properties for Product
                Title = "Product1",
                Company = "Company1",
                ListPrice = 29.99,
                Price = 19.99,
                ImageUrl = "test",
                CategoryId = 55
                // ... (set other properties as needed)
            };

            var newOrderDetail = new OrderDetail
            {
                OrderHeader = orderHeader,
                Product = product,
                Count = 3,
                Price = 19.99
            };

            _orderDetailRepository.Add(newOrderDetail);
            _db.SaveChanges();

            var retrievedOrderDetail = _orderDetailRepository.GetFirstOrDefault(od => od.Id == newOrderDetail.Id);
            Assert.NotNull(retrievedOrderDetail);
            Assert.Equal(newOrderDetail.Id, retrievedOrderDetail.Id);

           
        }

    

        [Fact]
        public void UpdateOrderDetail_UpdatesInDatabase()
        {
            // Happy Case
            var orderDetailIdToUpdate = 38;
            var orderDetailToUpdate = _orderDetailRepository.GetAll(od => od.Id == orderDetailIdToUpdate).FirstOrDefault();

                orderDetailToUpdate.Count = 10; // Modify a property
                _orderDetailRepository.Update(orderDetailToUpdate);
                _db.SaveChanges();

                var updatedOrderDetail = _orderDetailRepository.GetFirstOrDefault(od => od.Id == orderDetailIdToUpdate);
                Assert.NotNull(updatedOrderDetail);
                Assert.Equal(10, updatedOrderDetail.Count);
            
          
        }


        [Fact]
        public void RemoveOrderDetail_RemovesFromDatabase()
        {
            // Happy Case
            var orderDetailIdToRemove = 38;
            var orderDetailToRemove = _orderDetailRepository.GetFirstOrDefault(od => od.Id == orderDetailIdToRemove);
            _orderDetailRepository.Remove(orderDetailToRemove);
            _db.SaveChanges();

            var removedOrderDetail = _orderDetailRepository.GetFirstOrDefault(od => od.Id == orderDetailIdToRemove);
            Assert.Null(removedOrderDetail);

            // Unhappy Case
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Attempt to remove a null OrderDetail
                OrderDetail nullOrderDetail = null;
                _orderDetailRepository.Remove(nullOrderDetail);
                _db.SaveChanges();
            });
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _db.Dispose();
        }
    }
}
