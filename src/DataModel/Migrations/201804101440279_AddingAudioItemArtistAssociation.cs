namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingAudioItemArtistAssociation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AudioArtists",
                c => new
                    {
                        AudioItemId = c.Guid(nullable: false),
                        ArtistId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AudioItemId, t.ArtistId })
                .ForeignKey("dbo.AudioItem", t => t.AudioItemId, cascadeDelete: true)
                .ForeignKey("dbo.Artist", t => t.ArtistId, cascadeDelete: true)
                .Index(t => t.AudioItemId)
                .Index(t => t.ArtistId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AudioArtists", "ArtistId", "dbo.Artist");
            DropForeignKey("dbo.AudioArtists", "AudioItemId", "dbo.AudioItem");
            DropIndex("dbo.AudioArtists", new[] { "ArtistId" });
            DropIndex("dbo.AudioArtists", new[] { "AudioItemId" });
            DropTable("dbo.AudioArtists");
        }
    }
}
