namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUniqueIdxOnArtistName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Artist", "Name", c => c.String(maxLength: 150));
            CreateIndex("dbo.Artist", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Artist", new[] { "Name" });
            AlterColumn("dbo.Artist", "Name", c => c.String());
        }
    }
}
