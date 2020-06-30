using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
	[XmlType("partId")]
	public class ImportPartCar
	{
		[XmlAttribute("id")]
		public int PartId { get; set; }
	}
}
