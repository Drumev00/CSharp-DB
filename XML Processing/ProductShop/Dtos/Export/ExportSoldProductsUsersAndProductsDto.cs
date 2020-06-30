using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
	[XmlType("SoldProducts")]
	public class ExportSoldProductsUsersAndProductsDto
	{
		[XmlElement("count")]
		public int Count { get; set; }
		[XmlArray("products")]
		public ExportUsersAndProductsProductDto[] Products { get; set; }
	}
}
