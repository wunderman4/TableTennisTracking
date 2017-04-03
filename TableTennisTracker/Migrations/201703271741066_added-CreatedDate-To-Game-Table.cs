namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCreatedDateToGameTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "CreatedDate", c => c.DateTime(nullable: false));
        }

        
        public override void Down()
        {
            DropColumn("dbo.Games", "CreatedDate");
        }
    }
}
