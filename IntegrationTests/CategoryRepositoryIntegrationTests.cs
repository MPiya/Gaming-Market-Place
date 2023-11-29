using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository;
using F2Play.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace F2Play.Tests
{
    public class CategoryRepositoryIntegrationTests : IDisposable
    {
        private ApplicationDbContext _db;
        private CategoryRepository _categoryRepository;
        private string ConnectionString;

        public CategoryRepositoryIntegrationTests()
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
            _categoryRepository = new CategoryRepository(_db);
            _db.Database.EnsureCreated();
            SeedTestData();
        }

        private void SeedTestData()
        {
            var categories = new[]
            {
                new Category { Name = "Category1" },
                new Category { Name = "Category2" },
                new Category { Name = "Category3" },
            };

            _db.Categories.AddRange(categories);
            _db.SaveChanges();
        }

        [Fact]
        public void GetAllCategories_ReturnsAllCategories()
        {
            var categories = _categoryRepository.GetAll();
            Assert.NotNull(categories);
          
        }

        [Fact]
        public void GetCategoryById_ReturnsCorrectCategory()
        {
            var categoryId = 3;
            var category = _categoryRepository.GetFirstOrDefault(c => c.Id == categoryId);
            Assert.NotNull(category);
            Assert.Equal(categoryId, category.Id);
        }

        [Fact]
        public void GetCategoryById_WithInvalidId_ReturnsNull()
        {
            var invalidCategoryId = 99;
            var category = _categoryRepository.GetFirstOrDefault(c => c.Id == invalidCategoryId);
            Assert.Null(category);
        }

        [Fact]
        public void AddCategory_SavesToDatabase()
        {
            var newCategory = new Category { Name = "NewCategory" };
            _categoryRepository.Add(newCategory);
            _db.SaveChanges();

            var retrievedCategory = _categoryRepository.GetFirstOrDefault(c => c.Name == "NewCategory");
            Assert.NotNull(retrievedCategory);
            Assert.Equal(newCategory.Name, retrievedCategory.Name);
        }

        [Fact]
        public void RemoveCategory_RemovesFromDatabase()
        {
            var categoryIdToRemove = 1;
            var categoryToRemove = _categoryRepository.GetFirstOrDefault(c => c.Id == categoryIdToRemove);
            _categoryRepository.Remove(categoryToRemove);
            _db.SaveChanges();

            var removedCategory = _categoryRepository.GetFirstOrDefault(c => c.Id == categoryIdToRemove);
            Assert.Null(removedCategory);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
