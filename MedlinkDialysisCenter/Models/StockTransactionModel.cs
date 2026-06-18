using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedlinkDialysisCenter.Models
{
    public enum TransactionType
    {
        Usage,
        Received,
        Adjustment
    }

    public class StockTransactionModel
    {
        public int                  Id                      { get; set; }

        [Required]
        public int                  InventoryItemId         { get; set; }

        [ForeignKey(nameof(InventoryItemId))]
        public InventoryItemModel?  InventoryItem           { get; set; }

        [Required]
        public DateTime             TransactionDate         { get; set; } = DateTime.Now;

        [Required]
        public TransactionType      TransactionType         { get; set; }

        [Required]
        public int                  Quantity                { get; set; }

        [MaxLength(250)]
        public string?              Notes                   { get; set; }

        public DateTime             CreatedAt               { get; set; } = DateTime.Now;


    }
}