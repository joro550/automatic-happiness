using FluentMigrator;

namespace bkmarker.Migrations;

[Migration(202312112344)]
public class AddBookmarkTable : Migration
{
    public override void Up()
    {
        Create.Table("bookmarks")
          .WithColumn("id").AsInt64().PrimaryKey().Identity()
          .WithColumn("url").AsString()
          .WithColumn("content").AsString().Nullable()
          .WithColumn("image_url").AsString().Nullable()
          .WithColumn("image_alt_text").AsString().Nullable()
          .WithColumn("title").AsString();
    }

    public override void Down()
    {
        Delete.Table("bookmarks");
    }
}
