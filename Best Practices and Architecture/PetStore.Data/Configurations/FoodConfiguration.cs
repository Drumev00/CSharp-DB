using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetStore.Data.Models;

namespace PetStore.Data.Configurations
{
	public class FoodConfiguration : IEntityTypeConfiguration<Food>
	{
		public void Configure(EntityTypeBuilder<Food> food)
		{
			food
				.Property(f => f.Name)
				.HasMaxLength(DataSettings.MaxNameLength)
				.IsRequired();
		}
	}
}
