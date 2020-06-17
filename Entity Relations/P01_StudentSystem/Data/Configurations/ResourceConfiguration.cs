using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
	public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
	{
		public void Configure(EntityTypeBuilder<Resource> entity)
		{
			entity.HasKey(r => r.ResourceId);

			entity
			.Property(r => r.Name)
			.HasMaxLength(50)
			.IsRequired()
			.IsUnicode();

			entity
			.Property(r => r.Url)
			.IsRequired()
			.IsUnicode(false);

			entity
			.Property(r => r.ResourceType)
			.IsRequired();
		}
	}
}
