using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
	[XmlType("Users")]
	public class ExportUsersAndProductsFinalResultDto
	{
		[XmlElement("count")]
		public int Count { get; set; }
		[XmlArray("users")]
		public ExportUsersAndProductsUserDto[] Users { get; set; }
	}
}
