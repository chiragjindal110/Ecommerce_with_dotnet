var your_orders_div = document.getElementById("your_orders_div");


fetch("/Home/orders", { method: 'GET' })
    .then(result => result.json())
    .then((orders) => {
        //try{
        console.log(orders);
        orders.forEach((order) => {
            AppendOrders(order);
        })
        //}
        // catch(error)
        // {
        //     alert("Something Went Wrong!!")
        // }
    })



function AppendOrders(order) {
    var cart_item_div = document.createElement("div");
    var product_image = document.createElement("img");
    var product_details_div = document.createElement("div");
    var product_name = document.createElement("h1");
    var seller_name = document.createElement("p");
    var quantity_div = document.createElement("div");
    var quantity = document.createElement("p");
    var product_price = document.createElement("h1");
    var order_status = document.createElement("h1");
    var order_date = document.createElement("p");


    quantity.innerText = "Order Quantity: " + order.Quantity * 1 + "Kgs";
    product_name.innerText = order.ProductName;
    product_image.setAttribute("src", "/Product_Images/" +order.ProductPic);
    product_price.innerText = `₹${order.ItemValue}`;
    seller_name.innerText = "SELLER: chirag Enterprises";
    var status = order_status;
    status = JSON.parse(order.Status);
    order_status.innerText = status.status;
    order_status.style.color = "green";
    order_date.innerText = "Order Placed on: " + order.OrderTime.split("T")[0];

    product_image.style.minWidth = "250px ";
    product_image.style.maxWidth = "250px ";
    product_image.style.minHeight = "250px ";
    product_image.style.maxHeight = "250px ";
    cart_item_div.classList.add("cart-item-div");
    product_image.classList.add("product-image");
    product_details_div.classList.add("product-details-div");
    product_name.classList.add("order-product-name");
    seller_name.style.margin = "5px 0";
    quantity_div.classList.add("quantity-div");
    quantity.classList.add("quantity");
    product_price.classList.add("product-price");

    quantity_div.appendChild(quantity);
    product_details_div.appendChild(product_name)
    product_details_div.appendChild(seller_name)
    product_details_div.appendChild(quantity_div)
    product_details_div.appendChild(order_status);
    product_details_div.appendChild(order_date);

    cart_item_div.appendChild(product_image);
    cart_item_div.appendChild(product_details_div);
    cart_item_div.appendChild(product_price);
    your_orders_div.appendChild(cart_item_div);

}

var start = 0;
var load_more = document.getElementById("load_more");
function getProducts() {
    fetch("/Home/getProducts", { method: "POST", headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ start: start }) })
        .then(res => res.json())
        .then((d) => {
            for (let i = 0; i < d.length; i++) {
                if (d[i].ProductId) {
                    AppendProduct(d[i]);
                }
            }
            if (d.length == 0) {
                load_more.style.display = "none";
            }
            console.log(start);
            start = start + 5;
            load_more.disabled = false;
        })

}
getProducts();

function AppendProduct(product) {
    console.log(product);
    var p = document.createElement("div");
    p.classList.add("product");
    p.setAttribute("id", product.ProductId);
    var image = document.createElement("img");

    image.setAttribute("src", "/Product_Images/" + product.ProductPic);
    image.setAttribute("width", "250px");
    image.setAttribute("height", "200px");
    image.style.cursor = "pointer";
    var product_name = document.createElement("h2");
    product_name.classList.add("product-name");
    product_name.innerText = product.ProductName;
    var btns = document.createElement("div");
    var description_btn = document.createElement("button");
    description_btn.classList.add("description-btn");
    description_btn.innerText = "View Details";
    var add_to_cart_btn = document.createElement("button");
    add_to_cart_btn.classList.add("add-to-cart");
    add_to_cart_btn.innerText = "Add to cart";
    btns.appendChild(description_btn);
    btns.appendChild(add_to_cart_btn);
    // button to add in cart
    add_to_cart_btn.addEventListener("click", () => {
        fetch('/Home/islogin', { method: 'GET' })
            .then(res => res.json())
            .then((data) => {
                if (data.flag) {
                    fetch("/Home/addtocart", { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ product_id: product.ProductId, quantity: 1 }) })
                        .then(res => res.json())
                        .then((result) => {
                            if (result.flag == 0) {
                                window.location.href = "/verifyMessagePage";
                            }
                            else if (result.flag)
                                alert("Added to cart Successfully");
                            else
                                alert("something Went Wrong");
                        })
                }
                else {
                    var show = document.getElementById("login-signup-back");
                    show.style.display = "flex";
                }
            })
    });

    var products = document.getElementById("products");
    p.appendChild(image)
    p.appendChild(product_name)
    p.appendChild(btns);
    products.append(p);

    var description_outer_div = document.createElement("div");
    description_outer_div.classList.add("description_outer_div");
    var description_div = document.createElement("div");
    description_div.classList.add("description_div");
    var descr_inner_div = document.createElement("div");
    descr_inner_div.classList.add("description_inner_div");
    var description_image = document.createElement("img");
    var description_name = document.createElement("h2");
    var description_descr = document.createElement("p");
    var description_close = document.createElement("i");
    var descr_placeorder = document.createElement("button");
    var product_price = document.createElement("h3");
    product_price.innerText = "Price: ₹" + product.Price + "/kg";

    description_image.setAttribute("src", "/Product_Images/" + product.ProductPic);
    description_image.setAttribute("width", "350px");
    description_image.setAttribute("height", "250px");
    description_name.innerText = product.ProductName;
    description_descr.innerText = product.ProductDescription;
    descr_placeorder.className = "descr_btns";
    description_close.classList.add("fa", "fa-close", "descr_close");
    descr_placeorder.innerText = "Buy Now";
    description_div.appendChild(description_close);
    descr_inner_div.appendChild(description_image);
    descr_inner_div.appendChild(description_name);
    descr_inner_div.appendChild(description_descr);
    descr_inner_div.appendChild(product_price);
    description_div.appendChild(descr_inner_div);
    descr_inner_div.appendChild(descr_placeorder);
    description_outer_div.appendChild(description_div);
    document.body.append(description_outer_div);

    description_btn.onclick = () => {
        description_outer_div.style.display = "flex";
    }
    description_close.onclick = () => {
        description_outer_div.style.display = "none";
        login_signup_back.style.display = "none";
    }
    image.onclick = () => {
        description_outer_div.style.display = "flex";
    }
    descr_placeorder.addEventListener("click", () => {
        localStorage.setItem("products", JSON.stringify([{ product_id: product.ProductId, quantity: 1 }]));
        window.location.href = "/checkout";
    })
}

load_more.addEventListener("click", () => {
    load_more.disabled = true;
    getProducts();
})