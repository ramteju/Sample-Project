using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using ProductTracking.Models.Core;
using Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using ProductTracking.Logging;

namespace ProductTracking.Models
{


    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<UrlTrack> UrlTracks { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Tan> Tans { get; set; }
        public DbSet<TanChemical> TanChemicals { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<StageCondition> StageConditions { get; set; }
        public DbSet<ReactionRSD> ReactionRSD { get; set; }
        public DbSet<ReactionRSN> ReactionRSN { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<FtpFolder> FtpFolders { get; set; }
        public DbSet<SessionResult> SessionResult { get; set; }
        public DbSet<CVT> CVT { get; set; }
        public DbSet<FreeText> FreeText { get; set; }
        public DbSet<ActivityTracing> ActivityTracing { get; set; }
        public DbSet<TanData> TanData { get; set; }
        public DbSet<TanHistory> TanHistory { get; set; }
        public DbSet<SolventBoilingPoints> SolventBoilingPoints { get; set; }
        public DbSet<TanWiseKeywords> TanWiseKeywords { get; set; }
        public DbSet<ShipmentException> ShipmentExceptions { get; set; }
        public DbSet<TanKeywords> TanKeywords { get; set; }
        public DbSet<RegulerExpression> RegulerExpressions { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<QueryResponse> QueryResponses { get; set; }
        public DbSet<QueryWorkflow> QueryWorkflows { get; set; }
        public DbSet<QueryWorkflowUser> QueryWorkflowUsers { get; set; }
        public DbSet<TanMetaDataUpdateHistory> TanMetaDataUpdateHistory { get; set; }
        public DbSet<ShippmentOCR> ShippmentOCR { get; set; }
        public DbSet<ShippmentUploadStatus> ShippmentUploadStatus { get; set; }
        public DbSet<TanIssues> TanIssues { get; set; }
        public DbSet<DeliveryBatch> DeliveryBatches { get; set; }
        public DbSet<ShippmentUploadedExcel> ShippmentUploadedExcels { get; set; }
        public DbSet<UserDefaultDensities> UserDefaultDensities { get; set; }
        public DbSet<NamePriorities> NamePriorities { get; set; }
        public DbSet<NotificationTemplates> NotificationTemplates { get; set; }
        public DbSet<DateWiseRXNCount> DateWiseRXNCount { get; set; }
        public DbSet<TanActionHistories> TanActionHistories { get; set; }
        public DbSet<ErrorReport> ErrorReport { get; set; }
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ProductTracking.Migrations.Configuration>("DefaultConnection"));
            this.Database.CommandTimeout = 300;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                if (modelBuilder == null)
                {
                    throw new ArgumentNullException("modelBuilder");
                }
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<TanChemical>().HasRequired<Tan>(t => t.Tan).WithMany(t => t.TanChemicals).WillCascadeOnDelete(false);
                modelBuilder.Entity<Reaction>().HasRequired<Tan>(t => t.Tan).WithMany(t => t.Reactions).WillCascadeOnDelete(false);
                modelBuilder.Entity<Stage>().HasRequired<Reaction>(t => t.Reaction).WithMany(t => t.Stages).WillCascadeOnDelete(true);
                modelBuilder.Entity<SubstanceImagePaths>().HasRequired<TanChemical>(t => t.TanChemical).WithMany(t => t.Substancepaths).WillCascadeOnDelete(false);
                modelBuilder.Entity<Query>().HasRequired<ApplicationUser>(t => t.PostedBy).WithMany(t => t.Queries).WillCascadeOnDelete(false);
                modelBuilder.Entity<QueryResponse>().HasRequired<ApplicationUser>(t => t.User).WithMany(t => t.Responses).WillCascadeOnDelete(false);

                modelBuilder.Entity<Tan>()
                .HasMany<DeliveryBatch>(s => s.DeliveryBatches)
                .WithMany(c => c.Tans)
                .Map(cs =>
                {
                    cs.MapLeftKey("TanID");
                    cs.MapRightKey("DeliveryBatchId");
                    cs.ToTable("TanDeliveryBatches");
                });




                modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");
                EntityTypeConfiguration<ApplicationUser> usersTable = modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
                usersTable.Property((ApplicationUser u) => u.UserName).IsRequired();

                //modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationRole>(u => u.ApplicationRoles).WithMany(r => r.Users);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}