using AutoMapper;
using System.Globalization;

using CarDealer.Models;
using CarDealer.Dtos.Import;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
			//Import
			//Suppliers
			this.CreateMap<ImportSuppliers, Supplier>();
			//Parts
			this.CreateMap<ImportParts, Part>();
			//Customers
			this.CreateMap<ImportCustomers, Customer>()
				.ForMember(x => x.BirthDate, y => y.MapFrom(x => x.BirthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
			//Sales
			this.CreateMap<ImportSales, Sale>();
        }
    }
}
