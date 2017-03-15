namespace TableTennisTracker
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TableTennisTrackerDb : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Game>  Games { get; set; }
        public DbSet<HitLocation> HitLocations { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }

        // Your context has been configured to use a 'TableTennisTrackerDb' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'TableTennisTracker.TableTennisTrackerDb' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'TableTennisTrackerDb' 
        // connection string in the application configuration file.
        public TableTennisTrackerDb()
             : base("name=TableTennisTrackerDb")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GamePlayer>().HasKey(x => new { x.GameId, x.PlayerId });
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see     .

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}