using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Seller
{
    public int SellerId { get; set; }

    public string SellerCompany { get; set; } = null!;

    public string IsAuthorized { get; set; } = null!;

    public int UserId { get; set; }

    public string Gst { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
