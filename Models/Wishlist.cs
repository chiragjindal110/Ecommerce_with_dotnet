using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Wishlist
{
    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public virtual User? User { get; set; }
}
