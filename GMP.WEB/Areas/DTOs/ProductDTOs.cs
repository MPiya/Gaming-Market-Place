using F2Play.Models;

namespace F2Play.WebApp.Areas.DTOs
{
    public class ProductDTOs
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public double ListPrice { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public static ProductDTOs MapProductToDTO(Product p)
        {
            return new ProductDTOs
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Company = p.Company,
                ListPrice = p.ListPrice,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                Category = p.Category
            };
        }
    }
}
