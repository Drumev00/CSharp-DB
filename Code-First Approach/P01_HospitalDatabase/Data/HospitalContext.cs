using Microsoft.EntityFrameworkCore;

using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
	public class HospitalContext : DbContext
	{
		public HospitalContext()
		{
		}
		public HospitalContext(DbContextOptions options) 
			: base(options)
		{
		}

		public DbSet<Patient> Patients { get; set; }
		public DbSet<Visitation> Visitations { get; set; }
		public DbSet<Diagnose> Diagnoses { get; set; }
		public DbSet<Medicament> Medicaments { get; set; }
		public DbSet<PatientMedicament> PatientMedicament { get; set; }
		public DbSet<Doctor> Doctors { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if(!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(Configuring.ConnectionString);
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Patient>(entity =>
			{
				entity.HasKey(p => p.PatientId);

				entity
				.Property(p => p.FirstName)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(p => p.LastName)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(p => p.Address)
				.HasMaxLength(250)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(p => p.Email)
				.HasMaxLength(80)
				.IsRequired()
				.IsUnicode(false);

				entity
				.Property(p => p.HasInsurance);

				entity
				.HasMany(p => p.Prescriptions)
				.WithOne(pm => pm.Patient)
				.HasForeignKey(pm => pm.PatientId);
			});

			modelBuilder.Entity<Visitation>(entity =>
			{
				entity.HasKey(v => v.VisitationId);

				entity
				.Property(v => v.Date)
				.HasColumnType("DATETIME2")
				.IsRequired();

				entity
				.Property(v => v.Comments)
				.HasMaxLength(250)
				.IsRequired(false)
				.IsUnicode();

				entity
				.HasOne(v => v.Patient)
				.WithMany(p => p.Visitations)
				.HasForeignKey(v => v.PatientId);
			});

			modelBuilder.Entity<Diagnose>(entity =>
			{
				entity.HasKey(d => d.DiagnoseId);

				entity
				.Property(d => d.Name)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(d => d.Comments)
				.HasMaxLength(250)
				.IsRequired(false)
				.IsUnicode(false);

				entity
				.HasOne(d => d.Patient)
				.WithMany(p => p.Diagnoses)
				.HasForeignKey(d => d.PatientId);
			});

			modelBuilder.Entity<Medicament>(entity =>
			{
				entity.HasKey(m => m.MedicamentId);

				entity
				.Property(m => m.Name)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.HasMany(m => m.Prescriptions)
				.WithOne(pm => pm.Medicament)
				.HasForeignKey(pm => pm.MedicamentId);
			});

			modelBuilder.Entity<PatientMedicament>(entity => 
			{
				entity.HasKey(pm => new
				{
					pm.MedicamentId,
					pm.PatientId
				});
			});

			modelBuilder.Entity<Doctor>(entity =>
			{
				entity.HasKey(d => d.DoctorId);

				entity
				.Property(d => d.Name)
				.HasMaxLength(100)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(d => d.Specialty)
				.HasMaxLength(100)
				.IsRequired()
				.IsUnicode();

				entity
				.HasMany(d => d.Visitations)
				.WithOne(v => v.Doctor)
				.HasForeignKey(v => v.DoctorId);
			});
		}
	}
}
