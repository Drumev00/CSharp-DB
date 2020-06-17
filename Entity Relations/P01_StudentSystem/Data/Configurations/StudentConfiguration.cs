using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
	public class StudentConfiguration : IEntityTypeConfiguration<Student>
	{
		public void Configure(EntityTypeBuilder<Student> entity)
		{
			entity.HasKey(s => s.StudentId);

			entity
			.Property(s => s.Name)
			.HasMaxLength(100)
			.IsRequired()
			.IsUnicode();

			entity
			.Property(s => s.PhoneNumber)
			.HasMaxLength(10)
			.IsFixedLength()
			.IsRequired(false)
			.IsUnicode(false);

			entity
			.Property(s => s.RegisteredOn)
			.HasColumnType("DATETIME2")
			.IsRequired();

			entity
			.Property(s => s.Birthday)
			.HasColumnType("DATE")
			.IsRequired(false);

			entity
			.HasMany(s => s.CourseEnrollments)
			.WithOne(sc => sc.Student)
			.HasForeignKey(sc => sc.StudentId)
			.IsRequired();


			entity
			.HasMany(s => s.HomeworkSubmissions)
			.WithOne(h => h.Student)
			.HasForeignKey(h => h.StudentId);
		}
	}
}
