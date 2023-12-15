using FluentMigrator;

namespace bkmarker.Migrations;

[Migration(202315122344)]
public class AddBookmarkTable : Migration
{
  public override void Up()
  {
    Create.Table("bookmarks")
      .WithColumn("id").AsInt64().PrimaryKey().Identity()
      .WithColumn("url").AsString()
      .WithColumn("content").AsString()
      .WithColumn("image_url").AsString();

  }

  public override void Down()
  {
    Delete.Table("bookmarks");
  }
}
