namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingArtistsAndGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArtistGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Artist",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArtistGroups",
                c => new
                    {
                        ArtistId = c.Guid(nullable: false),
                        ArtistGroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArtistId, t.ArtistGroupId })
                .ForeignKey("dbo.Artist", t => t.ArtistId, cascadeDelete: true)
                .ForeignKey("dbo.ArtistGroup", t => t.ArtistGroupId, cascadeDelete: true)
                .Index(t => t.ArtistId)
                .Index(t => t.ArtistGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArtistGroups", "ArtistGroupId", "dbo.ArtistGroup");
            DropForeignKey("dbo.ArtistGroups", "ArtistId", "dbo.Artist");
            DropIndex("dbo.ArtistGroups", new[] { "ArtistGroupId" });
            DropIndex("dbo.ArtistGroups", new[] { "ArtistId" });
            DropTable("dbo.ArtistGroups");
            DropTable("dbo.Artist");
            DropTable("dbo.ArtistGroup");
        }
    }
}
