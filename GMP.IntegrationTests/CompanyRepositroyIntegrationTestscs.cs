using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository;
using F2Play.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xunit;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Transactions;

namespace F2Play.Tests
{
    public class CompanyRepositoryIntegrationTests : IDisposable
    {
        private ApplicationDbContext _db;
        private readonly ILogger _logger;
        private CompanyRepository _companyRepository;
        private string ConnectionString;
        private TransactionScope _transaction;

        public CompanyRepositoryIntegrationTests()
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
            _companyRepository = new CompanyRepository(_db, _logger);
            _db.Database.EnsureCreated();
            SeedTestData();

            // Start a new transaction before each test
            _transaction = new TransactionScope();

        }

        private void SeedTestData()
        {
            var companies = new[]
            {
                new Company { Name = "Company1" },
                new Company { Name = "Company2" },
                new Company { Name = "Company3" },
            };

            _db.Companies.AddRange(companies);
            _db.SaveChanges();
        }

        [Fact]
        public void GetAllCompanies_ReturnsAllCompanies()
        {
            var companies = _companyRepository.GetAll();
            Assert.NotNull(companies);
          
        }

        [Fact]
        public void GetCompanyById_ReturnsCorrectCompany()
        {
            var companyId = 8;
            var company = _companyRepository.GetFirstOrDefault(c => c.Id == companyId);
            Assert.NotNull(company);
            Assert.Equal(companyId, company.Id);
        }

        [Fact]
        public void GetCompanyById_WithInvalidId_ReturnsNull()
        {
            var invalidCompanyId = 90;
            var company = _companyRepository.GetFirstOrDefault(c => c.Id == invalidCompanyId);
            Assert.Null(company);
        }

        [Fact]
        public void AddCompany_SavesToDatabase()
        {
            var newCompany = new Company { Name = "NewCompany" };
            _companyRepository.Add(newCompany);
            _db.SaveChanges();

            var retrievedCompany = _companyRepository.GetFirstOrDefault(c => c.Name == "NewCompany");
            Assert.NotNull(retrievedCompany);
            Assert.Equal(newCompany.Name, retrievedCompany.Name);
        }

        [Fact]
        public void UpdateCompany_UpdatesInDatabase()
        {
            var companyIdToUpdate = 8;
            var companyToUpdate = _companyRepository.GetFirstOrDefault(c => c.Id == companyIdToUpdate);
            companyToUpdate.Name = "UpdatedCompanyName";
            _companyRepository.Update(companyToUpdate);
            _db.SaveChanges();

            var updatedCompany = _companyRepository.GetFirstOrDefault(c => c.Id == companyIdToUpdate);
            Assert.NotNull(updatedCompany);
            Assert.Equal("UpdatedCompanyName", updatedCompany.Name);
        }

        [Fact]
        public void RemoveCompany_RemovesFromDatabase()
        {
            var companyIdToRemove = 7;
            var companyToRemove = _companyRepository.GetFirstOrDefault(c => c.Id == companyIdToRemove);
            _companyRepository.Remove(companyToRemove);
            _db.SaveChanges();

            var removedCompany = _companyRepository.GetFirstOrDefault(c => c.Id == companyIdToRemove);
            Assert.Null(removedCompany);
        }

        public void Dispose()
        {
            // Roll back the transaction to undo changes made during the test
            _transaction.Dispose();

            // Dispose the DbContext
            _db.Dispose();
        }
    }
}
