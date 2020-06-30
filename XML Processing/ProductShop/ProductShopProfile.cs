using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
			//Import

			//Users
			this.CreateMap<ImportUserDto, User>();

			//Products
			this.CreateMap<ImportProductDto, Product>();

			//Categories
			this.CreateMap<ImportCategoryDto, Category>();

			//CategoryProduct
			this.CreateMap<ImportCategoryProductsDto, CategoryProduct>();

			//Export

			//Products
			this.CreateMap<Product, ExportProductsInRange>()
				.ForMember(x => x.Buyer, y => y.MapFrom(x => x.Buyer.FirstName + " " + x.Buyer.LastName));
        }
    }
}
