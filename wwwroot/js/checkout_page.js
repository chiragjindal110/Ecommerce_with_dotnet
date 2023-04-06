var products_div = document.getElementById("products_div");
var address_input = document.getElementById("address");
var address_submit = document.getElementById("address-submit");
var placeorder_btn = document.getElementById("placeorder_btn");
var storeaddress = document.getElementById("stored-address");
var change_address_btn =document.getElementById("change-address-btn");
var amount = document.getElementById("amount");
var items = document.getElementById("items");
placeorder_btn.addEventListener("click",()=>{
    if(!localStorage.getItem("products"))
    {
        window.location.href="/Home/Home";
    }
    placeorder_btn.disabled = true;
    fetch("/Checkout/placeorder",{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({products:JSON.parse(localStorage.getItem("products"))})})
    .then(response=>response.json())
    .then((res)=>{
        alert(res.message);
        localStorage.clear();
        window.location.href="/";
    })
})
if(storeaddress.innerText=="")
{
    address_input.style.display='inline';
    address_submit.style.display = 'inline';
    storeaddress.style.display = 'none';
    change_address_btn.style.display='none';
    placeorder_btn.disabled = true;
}
else{
    address_input.style.display='none';
    address_submit.style.display = 'none';
    storeaddress.style.display = 'inline';
    change_address_btn.style.display='inline';
    placeorder_btn.disabled = false;
}
change_address_btn.onclick=()=>{
    address_input.value = storeaddress.innerText;
    address_input.style.display='inline';
    address_submit.style.display = 'inline';
    storeaddress.style.display = 'none';
    change_address_btn.style.display='none';
    document.getElementById("address-saved-flag").style.display = "none";
}


address_submit.addEventListener("click",()=>{
    if(address_input.value.trim()!="")
    {
        fetch("/Checkout/storeaddress",{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({address: address_input.value})})
        .then(response=>response.json())
        .then((res)=>{
            if(res.status)
            {
                placeorder_btn.disabled = false;
                storeaddress.style.display ="inline";
                storeaddress.innerText=address_input.value;
                document.getElementById("address-saved-flag").style.display = "block";
                document.getElementById("change-address-btn").style.display = "inline";
                address_input.style.display='none';
                 address_submit.style.display = 'none';
                 placeorder_btn.disabled = false;
            }
        })
    }
    else{
        alert("Address Field can't be empty");
    }
})

var products = JSON.parse(localStorage.getItem("products"));
if(products)
{
    products.forEach(async element => {
        console.log(element);
        await fetch("/Checkout/getproduct",{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({product_id : element.ProductId})})
        .then(res=>res.json())
        .then((result)=>{
            result["Quantity"] = element.Quantity;
            console.log(result);
            AppendProduct(result);
        })
    });
}


function AppendProduct(product) {
    var cart_item_div = document.createElement("div");
    var product_image = document.createElement("img");
    var product_details_div = document.createElement("div");
    var product_name = document.createElement("h1");
    var seller_name = document.createElement("p");
    var product_description = document.createElement("p");
    var in_stock = document.createElement("h2");
    var quantity_div = document.createElement("div");
    var quantity_decrease_btn = document.createElement("button");
    var quantity = document.createElement("p");
    var quantity_increase_btn = document.createElement("button");
    var remove_btn = document.createElement("button");
    var product_price = document.createElement("h1");
    quantity.innerText = product.Quantity;
    product_description.innerText = product.ProductDescription;
    product_name.innerText = product.ProductName;
    product_image.setAttribute("src", "/Product_Images/" + product.ProductPic);
    product_price.innerText = `₹${product.Price}/kg`;
    seller_name.innerText = "SELLER: chirag Enterprises";

    if(product.stock>product.Quantity)
    {in_stock.innerText = "IN STOCK";}
    else
    {in_stock.innerText="OUT OF STOCK";}
    quantity_decrease_btn.innerText = "-";
    quantity_increase_btn.innerText = "+";
    remove_btn.innerText = "Remove";

    product_image.style.minWidth="300px ";
    product_image.style.maxWidth="300px ";
    product_image.style.minHeight="300px ";
    product_image.style.maxHeight="300px ";
    product_description.style.marginBottom = "0";
    if(product.Stock>product.Quantity)
    in_stock.style.color = "green";
    else if(product.Stock==0)
    {
        deletefrom_local_storage(product);
        products_div.removeChild(cart_item_div);
    }
    else
    {
        in_stock.style.color = "red";
        quantity.innerText = product.Stock*1;
        change_in_localstorage(product,product.Stock-product.Quantity)
        quantity_increase_btn.disabled = true;
    }
    in_stock.style.marginTop = "0";
    cart_item_div.classList.add("cart-item-div");
    product_image.classList.add("product-image");
    product_details_div.classList.add("product-details-div");
    product_name.classList.add("product-name");
    seller_name.style.margin = "5px 0";
    quantity_div.classList.add("quantity-div");
    quantity_decrease_btn.classList.add("quantity-change-btn", "decrease-btn");
    quantity_increase_btn.classList.add("quantity-change-btn");
    quantity.classList.add("quantity");
    remove_btn.classList.add("remove-btn");
    product_price.classList.add("product-price");

    quantity_div.appendChild(quantity_decrease_btn);
    quantity_div.appendChild(quantity);
    quantity_div.appendChild(quantity_increase_btn);
    product_details_div.appendChild(product_name)
    product_details_div.appendChild(seller_name)
    product_details_div.appendChild(product_description)
    product_details_div.appendChild(in_stock)
    product_details_div.appendChild(quantity_div)
    product_details_div.appendChild(remove_btn);
    cart_item_div.appendChild(product_image);
    cart_item_div.appendChild(product_details_div);
    cart_item_div.appendChild(product_price);
    products_div.appendChild(cart_item_div);
    amount.innerText = amount.innerText*1 + quantity.innerText*1*product.Price;
    items.innerText = items.innerText*1 + quantity.innerText*1;
    if(quantity.innerText*1 ==0)
    {deletefrom_local_storage(product);
        products_div.removeChild(cart_item_div);
    }


    quantity_decrease_btn.addEventListener("click", () => {
                    quantity.innerText = quantity.innerText * 1 - 1;
                    if (quantity.innerText == 0) {
                            products_div.removeChild(cart_item_div);   
                            deletefrom_local_storage(product);
                    }
                    if(quantity.innerText*1 +1 <=product.Stock)
                    {
                        in_stock.innerText = "IN STOCK";
                        in_stock.style.color = "green";
                        quantity_increase_btn.disabled = false;
                    }
                    change_in_localstorage(product,-1); 
                    amount.innerText = amount.innerText*1 - product.Price;
                    items.innerText = items.innerText*1 -1;

    })
    quantity_increase_btn.addEventListener("click", () => {
        if(quantity.innerText*1 +1 >product.Stock)
        {
            in_stock.innerText = "OUT OF STOCK";
            in_stock.style.color = "red";
            quantity_increase_btn.disabled = true;
        }
        quantity.innerText = quantity.innerText * 1 + 1;
        change_in_localstorage(product,1);
        amount.innerText = amount.innerText*1 + product.Price*1;
        items.innerText = items.innerText*1 +1;
  

    })
    remove_btn.addEventListener("click", () => {
        products_div.removeChild(cart_item_div);
        deletefrom_local_storage(product);
        amount.innerText = amount.innerText*1 - quantity.innerText*1*product.Price;
        items.innerText = items.innerText*1 - quantity.innerText ;
    })

}

function change_in_localstorage(product,change)
{
    var products = JSON.parse(localStorage.getItem("products"));
    for(var i=0;i<products.length;i++)
    {
        if(products[i].ProductId==product.ProductId)
        {
            products[i].Quantity = products[i].Quantity+ change;
            break;
        }
    }
    localStorage.setItem("products",JSON.stringify(products));
}

function deletefrom_local_storage(product)
{
    var products = JSON.parse(localStorage.getItem("products"));
    for(var i=0;i<products.length;i++)
    {
        if(products[i].ProductId==product.ProductId)
        {
           products.splice(i,1);
        }
    }
    localStorage.setItem("products",JSON.stringify(products));
}

