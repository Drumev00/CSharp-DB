using Microsoft.EntityFrameworkCore;

using P03_FootballBetting.Data.Configurations;
using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data
{
	public class FootballBettingContext : DbContext
	{
		public FootballBettingContext()
		{

		}
		public FootballBettingContext(DbContextOptions options)
			: base(options)
		{

		}

		public DbSet<Color> Colors { get; set; }
		public DbSet<Town> Towns { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Position> Positions { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Bet> Bets { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if(!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(ConnectionConfiguration.ConnectionString);
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Team>(entity =>
			{
				entity.HasKey(t => t.TeamId);

				entity
				.Property(t => t.Name)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(t => t.LogoUrl)
				.HasMaxLength(300)
				.IsRequired(false)
				.IsUnicode(false);

				entity
				.Property(t => t.Initials)
				.HasMaxLength(3)
				.IsFixedLength()
				.IsRequired();

				entity
				.Property(t => t.Budget)
				.IsRequired();

				entity
				.HasOne(t => t.PrimaryKitColor)
				.WithMany(c => c.PrimaryKitTeams)
				.HasForeignKey(t => t.PrimaryKitColorId)
				.OnDelete(DeleteBehavior.Restrict);

				entity
				.HasOne(t => t.SecondaryKitColor)
				.WithMany(c => c.SecondaryKitTeams)
				.HasForeignKey(t => t.SecondaryKitColorId)
				.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Color>(entity =>
			{
				entity.HasKey(c => c.ColorId);

				entity
				.Property(c => c.Name)
				.IsRequired();
			});

			modelBuilder.Entity<Town>(entity =>
			{
				entity.HasKey(t => t.TownId);

				entity
				.Property(t => t.Name)
				.HasMaxLength(50)
				.IsRequired();

				entity
				.HasMany(to => to.Teams)
				.WithOne(te => te.Town)
				.HasForeignKey(te => te.TownId);
			});

			modelBuilder.Entity<Country>(entity =>
			{
				entity.HasKey(c => c.CountryId);

				entity
				.Property(c => c.Name)
				.HasMaxLength(30)
				.IsRequired();

				entity
				.HasMany(c => c.Towns)
				.WithOne(t => t.Country)
				.HasForeignKey(t => t.CountryId);
			});

			modelBuilder.Entity<Player>(entity =>
			{
				entity.HasKey(p => p.PlayerId);

				entity
				.Property(p => p.Name)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(p => p.SquadNumber)
				.IsRequired();

				entity
				.Property(p => p.IsInjured)
				.IsRequired();

				entity
				.HasOne(p => p.Team)
				.WithMany(t => t.Players)
				.HasForeignKey(p => p.TeamId);

				entity
				.HasMany(p => p.PlayerStatistics)
				.WithOne(ps => ps.Player)
				.HasForeignKey(ps => ps.PlayerId);
			});

			modelBuilder.Entity<Position>(entity =>
			{
				entity.HasKey(p => p.PositionId);

				entity
				.Property(p => p.Name)
				.HasMaxLength(20)
				.IsRequired()
				.IsUnicode(false);

				entity
				.HasMany(po => po.Players)
				.WithOne(pl => pl.Position)
				.HasForeignKey(pl => pl.PositionId);
			});

			modelBuilder.Entity<PlayerStatistic>(entity =>
			{
				entity.HasKey(ps => new { ps.PlayerId, ps.GameId });

				entity
				.Property(ps => ps.ScoredGoals)
				.IsRequired();

				entity
				.Property(ps => ps.Assists)
				.IsRequired();

				entity
				.Property(ps => ps.MinutesPlayed)
				.IsRequired();
			});

			modelBuilder.Entity<Game>(entity =>
			{
				entity.HasKey(g => g.GameId);

				entity
				.Property(g => g.HomeTeamGoals)
				.IsRequired();

				entity
				.Property(g => g.AwayTeamGoals)
				.IsRequired();

				entity
				.Property(g => g.DateTime)
				.HasColumnType("DATETIME2")
				.IsRequired();

				entity
				.Property(g => g.HomeTeamBetRate)
				.IsRequired(false);

				entity
				.Property(g => g.AwayTeamBetRate)
				.IsRequired(false);

				entity
				.Property(g => g.DrawBetRate)
				.IsRequired(false);

				entity
				.Property(g => g.Result)
				.HasMaxLength(5)
				.IsRequired();

				entity
				.HasOne(g => g.HomeTeam)
				.WithMany(t => t.HomeGames)
				.HasForeignKey(g => g.HomeTeamId)
				.OnDelete(DeleteBehavior.Restrict);

				entity
				.HasOne(g => g.AwayTeam)
				.WithMany(t => t.AwayGames)
				.HasForeignKey(g => g.AwayTeamId)
				.OnDelete(DeleteBehavior.Restrict);

				entity
				.HasMany(g => g.PlayerStatistics)
				.WithOne(ps => ps.Game)
				.HasForeignKey(ps => ps.GameId);
			});

			modelBuilder.Entity<Bet>(entity =>
			{
				entity.HasKey(b => b.BetId);

				entity
				.Property(b => b.Amount)
				.IsRequired();

				entity
				.Property(b => b.Prediction)
				.IsRequired();

				entity
				.Property(b => b.DateTime)
				.HasColumnType("DATETIME2")
				.IsRequired();

				entity
				.HasOne(b => b.Game)
				.WithMany(g => g.Bets)
				.HasForeignKey(b => b.GameId);
			});

			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(u => u.UserId);

				entity
				.Property(u => u.Username)
				.HasMaxLength(26)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(u => u.Password)
				.HasMaxLength(30)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(u => u.Email)
				.IsRequired()
				.IsUnicode(false);

				entity
				.Property(u => u.Name)
				.HasMaxLength(30)
				.IsRequired(false)
				.IsUnicode();

				entity
				.Property(u => u.Balance)
				.IsRequired();

				entity
				.HasMany(u => u.Bets)
				.WithOne(b => b.User)
				.HasForeignKey(b => b.UserId);
			});
		}
	}
}
