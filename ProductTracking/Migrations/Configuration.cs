namespace ProductTracking.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ProductTracking.Models;
    using System.IO;
    using System.Web;
    using System.Transactions;
    using ProductTracking.Models.Core;
    using Entities;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using Excelra.Utils.Library;

    internal sealed class Configuration : DbMigrationsConfiguration<ProductTracking.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ProductTracking.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);
            if (!context.Roles.Any())
            {
                var store = new RoleStore<ApplicationRoles>(context);
                var manager = new RoleManager<ApplicationRoles>(store);
                manager.Create(new ApplicationRoles { Name = Role.Curator.DescriptionAttribute(), Role = Role.Curator});
                manager.Create(new ApplicationRoles { Name = Role.Reviewer.DescriptionAttribute(), Role = Role.Reviewer });
                manager.Create(new ApplicationRoles { Name = Role.ToolManager.DescriptionAttribute(), Role = Role.ToolManager });
                manager.Create(new ApplicationRoles { Name = Role.QC.DescriptionAttribute(), Role = Role.QC });
                manager.Create(new ApplicationRoles { Name = Role.ProjectManger.DescriptionAttribute(), Role = Role.ProjectManger });
            }
        }
    }
}
