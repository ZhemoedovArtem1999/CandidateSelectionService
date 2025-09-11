using CandidateSelectionService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<DataCandidate> DataCandidates { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<SocialNetwork> SocialNetworks { get; set; }
        public virtual DbSet<SocialNetworkType> SocialNetworkTypes { get; set; }
        public virtual DbSet<WorkSchedule> WorkSchedules { get; set; }
        public virtual DbSet<Verification> Verifications { get; set; }
        public virtual DbSet<VerificationEvent> VerificationEvents { get; set; }
        public virtual DbSet<VerificationEventResult> VerificationEventResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var schemeHr = "hr";
            var schemeAuth = "auth";
            var schemeVerification = "verification";

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("user_pkey");

                entity.ToTable("user", schemeAuth);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.MiddleName).HasColumnName("middlename");
                entity.Property(e => e.Login).HasColumnName("login");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Salt).HasColumnName("salt");

            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("refresh_token_pkey");

                entity.ToTable("refresh_token", schemeAuth);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(e => e.CreatedDate)
                    .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                    .HasColumnName("created_date");
                entity.Property(e => e.RevokedDate).HasColumnName("revoked_date");
                entity.Property(e => e.ReplacedByToken).HasColumnName("replaced_by_token").IsRequired(false);

                entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("refresh_token_user_id_fkey");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("candidate_pkey");

                entity.ToTable("candidate", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DataId).HasColumnName("data_id");
                entity.Property(e => e.LastUpdated)
                    .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                    .HasColumnName("last_updated");
                entity.Property(e => e.CreatedUserId).HasColumnName("created_user_id");
                entity.Property(e => e.WorkScheduleId).HasColumnName("work_schedule_id");

                entity.HasOne(d => d.WorkSchedule).WithMany(p => p.Candidates)
                    .HasForeignKey(d => d.WorkScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("candidate_work_schedule_id_fkey");

                entity.HasOne(d => d.DataCandidate).WithOne(p => p.Candidate)
                    .HasForeignKey<Candidate>(d => d.DataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("employee_pkey");

                entity.ToTable("employee", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DataId).HasColumnName("data_id");
                entity.Property(e => e.EmploymentDate)
                 .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .HasColumnName("employment_date");
                entity.Property(e => e.WorkScheduleId).HasColumnName("work_schedule_id");

                entity.HasOne(d => d.WorkSchedule).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.WorkScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("employee_work_schedule_id_fkey");

                entity.HasOne(d => d.DataCandidate).WithOne(p => p.Employee)
                    .HasForeignKey<Employee>(d => d.DataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DataCandidate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("data_candidate_pkey");

                entity.ToTable("data_candidate", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.MiddleName).HasColumnName("middlename");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.DateBirth).HasColumnName("date_birth");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<SocialNetwork>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("social_network_pkey");

                entity.ToTable("social_network", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DataCandidateId).HasColumnName("data_candidate_id");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.TypeId).HasColumnName("type_id");
                entity.Property(e => e.DateAdded)
                    .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                    .HasColumnName("date_added");

                entity.HasOne(d => d.DataCandidate).WithMany(p => p.SocialNetworks)
                    .HasForeignKey(d => d.DataCandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("social_network_data_candidate_fkey");

                entity.HasOne(d => d.SocialNetworkType).WithMany(p => p.SocialNetworks)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("social_network_social_type_fkey");
            });

            modelBuilder.Entity<SocialNetworkType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("social_network_type_pkey");

                entity.ToTable("social_network_type", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<WorkSchedule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("work_schedule_pkey");

                entity.ToTable("work_schedule", schemeHr);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<Verification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("verification_pkey");

                entity.ToTable("verification", schemeVerification);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserName).HasColumnName("user_name");
                entity.Property(e => e.Date)
                    .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                    .HasColumnName("date");
                entity.Property(e => e.SearchData).HasColumnName("search_data");
            });

            modelBuilder.Entity<VerificationEvent>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("verification_event_pkey");

                entity.ToTable("verification_event", schemeVerification);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.VerificationId).HasColumnName("verification_id");

                entity.HasOne(d => d.Verification).WithMany(p => p.VerificationEvents)
                    .HasForeignKey(d => d.VerificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("verification_event_verification_id_fkey");
            });

            modelBuilder.Entity<VerificationEventResult>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("verification_event_result_pkey");

                entity.ToTable("verification_event_result", schemeVerification);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.VerificationEventId).HasColumnName("verification_event_id");
                entity.Property(e => e.EntityId).HasColumnName("entity_id");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.MiddleName).HasColumnName("middlename");
                entity.Property(e => e.DateBirth).HasColumnName("date_birth");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.WorkSchedule).HasColumnName("work_schedule");

                entity.HasOne(d => d.VerificationEvent).WithMany(p => p.VerificationEventResults)
                    .HasForeignKey(d => d.VerificationEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("verification_event_result_verification_event_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
