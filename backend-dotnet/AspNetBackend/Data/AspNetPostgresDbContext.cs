using Microsoft.EntityFrameworkCore;
using AspNetBackend.Entities;


namespace AspNetBackend.Data;

public class AspNetPostgresDbContext(DbContextOptions<AspNetPostgresDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<QuizSession> QuizSessions => Set<QuizSession>();
    public DbSet<LeaderBoardEntry> LeaderBoardEntries => Set<LeaderBoardEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity description for player model
        modelBuilder.Entity<Player>(entity =>
        {
            // add it to scheme players
            entity.ToTable("players", "players");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnName("refresh_token_expiry_time");
            entity.Property(e => e.UserEmail).HasColumnName("user_email");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // Entity description for leaderboard model
        modelBuilder.Entity<LeaderBoardEntry>(entity =>
        {
            // add it to scheme players
            entity.ToTable("leaderboard", "players");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.PlayerName).HasColumnName("player_name");
            entity.Property(e => e.TotalScore).HasColumnName("total_score");
            entity.Property(e => e.TotalTime).HasColumnName("total_time");
            entity.Property(e => e.SessionsPlayed).HasColumnName("sessions_played");
        });

        // this entity will be ignored in any migrations because
        // it's read only for the dotnet backend and is initialized by java backend
        modelBuilder.Entity<QuizSession>(entity =>
        {
            // specify that it's only for viewing.
            // the entity belongs to the scheme quiz
            entity.ToView("quiz_session", "quiz");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TotalTime).HasColumnName("total_time");
            entity.Property(e => e.TotalScore).HasColumnName("total_score");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
        });

    }
}
