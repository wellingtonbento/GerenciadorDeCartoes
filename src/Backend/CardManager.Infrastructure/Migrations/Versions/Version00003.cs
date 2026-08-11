using FluentMigrator;
using System.Data;

namespace CardManager.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_TRANSACTION, "Create table to save the Transaction´s information")]
    public class Version00003 : VersionBase
    {
        public override void Up()
        {
            CreateTable("Transactions")
                .WithColumn("CardId").AsInt64().NotNullable().ForeignKey("Cards", "Id").OnDelete(Rule.Cascade)
                .WithColumn("PaymentMethod").AsInt32().NotNullable()
                .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("Description").AsString(255).NotNullable();
        }
    }
}
