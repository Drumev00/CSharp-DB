using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
	public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
	{
		public void Configure(EntityTypeBuilder<StudentCourse> entity)
		{
			entity.HasKey(sc => new { sc.StudentId, sc.CourseId });
		}
	}
}
