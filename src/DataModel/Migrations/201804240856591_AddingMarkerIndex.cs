namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMarkerIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Marker", new[] { "AudioItemId" });
            AddColumn("dbo.Marker", "State", c => c.String());
            AlterColumn("dbo.Marker", "Type", c => c.String(maxLength: 50));
            CreateIndex("dbo.Marker", new[] { "AudioItemId", "Offset", "Type" }, unique: true, name: "IX_MarkerKey");
            DropColumn("dbo.Marker", "CustomName");
            DropColumn("dbo.Marker", "Command");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Marker", "Command", c => c.String());
            AddColumn("dbo.Marker", "CustomName", c => c.String());
            DropIndex("dbo.Marker", "IX_MarkerKey");
            AlterColumn("dbo.Marker", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Marker", "State");
            CreateIndex("dbo.Marker", "AudioItemId");
        }
    }
}
