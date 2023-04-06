using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? ItemValue { get; set; }

    public DateTime OrderTime { get; set; }

    public string? CustomerAddress { get; set; }

    public string Status { get; set; } = null!;

    public int? UserId { get; set; }
}
