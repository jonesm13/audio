namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingArtistsAndGroups1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ArtistGroups", newName: "ArtistsArtistGroups");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ArtistsArtistGroups", newName: "ArtistGroups");
        }
    }
}
