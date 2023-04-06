using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    public int? SellerId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductPic { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public int Stock { get; set; }

    public long Price { get; set; }

    public int? Sale { get; set; }

    public short? Status { get; set; }

    public virtual Seller? Seller { get; set; }
}
