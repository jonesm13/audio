namespace DataModel.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddingMarkers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Marker",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AudioItemId = c.Guid(nullable: false),
                        Offset = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        CustomName = c.String(),
                        Command = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AudioItem", t => t.AudioItemId, cascadeDelete: true)
                .Index(t => t.AudioItemId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Marker", "AudioItemId", "dbo.AudioItem");
            DropIndex("dbo.Marker", new[] { "AudioItemId" });
            DropTable("dbo.Marker");
        }
    }
}
