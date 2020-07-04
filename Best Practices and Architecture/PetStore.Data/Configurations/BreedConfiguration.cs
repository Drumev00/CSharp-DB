using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetStore.Data.Models;

namespace PetStore.Data.Configurations
{
	public class BreedConfiguration : IEntityTypeConfiguration<Breed>
	{
		public void Configure(EntityTypeBuilder<Breed> breed)
		{
			breed
				.Property(b => b.Name)
				.HasMaxLength(DataSettings.MaxNameLength)
				.IsRequired();
		}
	}
}
