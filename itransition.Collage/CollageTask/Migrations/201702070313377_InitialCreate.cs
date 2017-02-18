namespace CollageTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Collages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CollageName = c.String(),
                        PhotoIds = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        UserId = c.String(),
                        Effects = c.String(),
                        Collage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Collages", t => t.Collage_Id)
                .Index(t => t.Collage_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Images", "Collage_Id", "dbo.Collages");
            DropIndex("dbo.Images", new[] { "Collage_Id" });
            DropTable("dbo.Images");
            DropTable("dbo.Collages");
        }
    }
}
