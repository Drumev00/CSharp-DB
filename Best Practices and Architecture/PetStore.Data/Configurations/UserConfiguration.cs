using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetStore.Data.Models;

namespace PetStore.Data.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> user)
		{
			user
				.Property(u => u.Name)
				.HasMaxLength(DataSettings.MaxNameLength)
				.IsRequired();

			user
				.Property(u => u.Email)
				.HasMaxLength(DataSettings.EmailMaxLength)
				.IsRequired();

			user
				.HasIndex(u => u.Email)
				.IsUnique();
		}
	}
}
