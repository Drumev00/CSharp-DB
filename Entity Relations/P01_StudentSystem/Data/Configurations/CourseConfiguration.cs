using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
	public class CourseConfiguration : IEntityTypeConfiguration<Course>
	{
		public void Configure(EntityTypeBuilder<Course> entity)
		{
			entity.HasKey(c => c.CourseId);

			entity
			.Property(c => c.Name)
			.HasMaxLength(80)
			.IsRequired()
			.IsUnicode();

			entity
			.Property(c => c.Description)
			.IsRequired(false)
			.IsUnicode();

			entity
			.Property(c => c.StartDate)
			.HasColumnType("DATE")
			.IsRequired();

			entity
			.Property(c => c.EndDate)
			.HasColumnType("DATE")
			.IsRequired();

			entity
			.Property(c => c.Price)
			.IsRequired();

			entity
			.HasMany(c => c.StudentsEnrolled)
			.WithOne(sc => sc.Course)
			.HasForeignKey(sc => sc.CourseId)
			.IsRequired();

			entity
			.HasMany(c => c.Resources)
			.WithOne(r => r.Course)
			.HasForeignKey(r => r.CourseId);

			entity
			.HasMany(c => c.HomeworkSubmissions)
			.WithOne(h => h.Course)
			.HasForeignKey(h => h.CourseId);
		}
	}
}
