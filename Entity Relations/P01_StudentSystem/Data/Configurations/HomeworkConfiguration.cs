using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
	public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
	{
		public void Configure(EntityTypeBuilder<Homework> entity)
		{
			entity.HasKey(h => h.HomeworkId);

			entity
			.Property(h => h.Content)
			.IsUnicode(false);

			entity
			.Property(h => h.ContentType)
			.IsRequired();

			entity
			.Property(h => h.SubmissionTime)
			.HasColumnType("DATETIME2")
			.IsRequired();
		}
	}
}
