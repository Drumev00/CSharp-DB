using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetStore.Data.Models;

namespace PetStore.Data.Configurations
{
	public class ToyConfiguration : IEntityTypeConfiguration<Toy>
	{
		public void Configure(EntityTypeBuilder<Toy> toy)
		{
			toy
				.Property(t => t.Name)
				.HasMaxLength(DataSettings.MaxNameLength)
				.IsRequired();

			toy
				.Property(t => t.Description)
				.HasMaxLength(DataSettings.DescriptionMaxLength);
		}
	}
}
