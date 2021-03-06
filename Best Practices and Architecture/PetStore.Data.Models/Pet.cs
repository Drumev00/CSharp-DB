﻿using System;

using PetStore.Data.Models.Enumerations;

namespace PetStore.Data.Models
{
	public class Pet
	{
		public int Id { get; set; }
		public Gender Gender { get; set; }
		public DateTime BirthDate { get; set; }
		public decimal Price { get; set; }
		public string Description { get; set; }
		public int BreedId { get; set; }
		public Breed Breed { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; }
		public int? OrderId { get; set; }
		public Order Order { get; set; }
	}
}
