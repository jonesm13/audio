namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPlayRestriction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayRestriction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AudioItemId = c.Guid(nullable: false),
                        Days = c.Int(nullable: false),
                        Start = c.Int(nullable: false),
                        End = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AudioItem", t => t.AudioItemId, cascadeDelete: true)
                .Index(t => t.AudioItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayRestriction", "AudioItemId", "dbo.AudioItem");
            DropIndex("dbo.PlayRestriction", new[] { "AudioItemId" });
            DropTable("dbo.PlayRestriction");
        }
    }
}
