namespace CollageTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Collages", "JSON", c => c.String());
            AddColumn("dbo.Collages", "Path", c => c.String());
            DropColumn("dbo.Collages", "PhotoIds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Collages", "PhotoIds", c => c.String());
            DropColumn("dbo.Collages", "Path");
            DropColumn("dbo.Collages", "JSON");
        }
    }
}
