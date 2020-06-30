﻿using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
	[XmlType("Sale")]
	public class ImportSales
	{
		[XmlElement("carId")]
		public int CarId { get; set; }
		[XmlElement("customerId")]
		public int CustomerId { get; set; }
		[XmlElement("discount")]
		public decimal Discount { get; set; }
	}
}