using Microsoft.EntityFrameworkCore;

namespace KurosukeHomeFantasmicUWP.Models.SQL
{
    public class RecentProjectEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProjectFilePath { get; set; }
    }

    public class RecentProjectContext : DbContext
    {
        public DbSet<RecentProjectEntity> RecentProjects { get; set; }

        public RecentProjectContext()
        {
            Database.EnsureCreated();
        }
        private string dbFilePath = "recentprojects.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + dbFilePath);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
