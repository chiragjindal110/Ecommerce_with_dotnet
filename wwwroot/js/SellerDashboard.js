var products_cart = document.getElementById("products-cart");
var product_submit_btn = document.getElementById("product-submit-btn");
var product_pic = document.getElementById("product_pic");
var product_name1 = document.getElementById("product-name");
var product_description1 = document.getElementById("product-description");
var product_stock = document.getElementById("product-stock");
var orders_completed = document.getElementById("orders_completed");
var product_price1 = document.getElementById("product-price");
var image = document.getElementById("image");
image.style.backgroundImage = "url(' ')";
var isUpdatingFlag = false;
var product_id;
var sellerlogout = document.getElementById("sellerlogout");
sellerlogout.addEventListener("click", () => {
    window.location.href = "/Home/Home";
})
product_submit_btn.addEventListener("click", (event) => {

    if ((product_pic.value == "" && !isUpdatingFlag) || product_description1.value.trim() == "" || product_name1.value.trim() == "" || product_price1.value.trim() == "") {
        alert("Fields can't be empty");
    }
    else {
        var content = new FormData();
            var picname = product_pic.files[0];
            content.append("picname", picname);
            content.append("product_name", product_name1.value);
            content.append("product_description", product_description1.value);
            content.append("product_stock", product_stock.value);
            content.append("product_price", product_price1.value);
            content.append("product_id", product_id);
            content.append("product_pic", image.style.backgroundImage.substring(5, image.style.backgroundImage.length - 2))
        if (isUpdatingFlag) {
            fetch("/Seller/updateproduct", { method: 'POST', body: content })
            .then(res => res.json())
                .then((data) => {
                    if (data.status) {
                        alert("Product Updated Succesfully");
                        window.location.reload();
                        isUpdatingFlag = false;
                    }
                    else { alert("Something Went wrong") };
                })
        }
        else {
            fetch("/Seller/addproduct", { method: 'POST', body: content })
                .then(res => res.json())
                .then((data) => {
                    if (data.status) {
                        console.log(data);
                        alert("product added successfully");
                        AppendProduct({ stock: product_stock.value, product_description: product_description1.value, product_name: product_name1.value, product_pic: data.product_pic, price: product_price1.value });
                    }
                    else {
                        alert("Something went wrong");
                    }
                })
        }
    }
})

product_pic.addEventListener("click", () => {

    product_pic.addEventListener("change", () => {
        console.log("in change");
        var reader = new FileReader();
        console.log(product_pic, product_pic.files);
        reader.readAsDataURL(product_pic.files[0]);
        reader.addEventListener("load", () => {
            image.style.backgroundImage = `url(${reader.result})`;
            image.style.backgroundSize = "cover";
            image.style.backgroundRepeat = "no-repeat";

        })
    })
})
fetch("/Seller/getsellerproducts", { method: 'GET' })
    .then(res => res.json())
    .then((result) => {
        result.forEach((product) => {
            AppendProduct(product);
        })
    })

function AppendProduct(product) {
    var cart_item_div = document.createElement("div");
    var product_image = document.createElement("img");
    var product_details_div = document.createElement("div");
    var product_name = document.createElement("h1");
    var seller_name = document.createElement("p");
    var product_description = document.createElement("p");
    var quantity_div = document.createElement("div");
    var quantity = document.createElement("p");
    var remove_btn = document.createElement("button");
    var product_price = document.createElement("h1");
    var edit_product = document.createElement("button");
    var edit_icon = document.createElement("i");


    quantity.innerText = "STOCK: " + product.Stock;
    product_description.innerText = product.ProductDescription;
    product_name.innerText = product.ProductName;
    product_image.setAttribute("src", "/Product_Images/" + product.ProductPic);
    product_price.innerText = `₹${product.Price}/kg`;
    seller_name.innerText = "SELLER: chirag Enterprises";
    remove_btn.innerText = "Remove";
    edit_product.innerText = "Edit   ";
    edit_product.appendChild(edit_icon);


    product_image.style.minWidth = "300px ";
    product_image.style.maxWidth = "300px ";
    product_image.style.minHeight = "300px ";
    product_image.style.maxHeight = "300px ";
    product_image.setAttribute("max-width", "300px !important");
    product_description.style.marginBottom = "0";
    cart_item_div.classList.add("cart-item-div");
    product_image.classList.add("product-image");
    product_details_div.classList.add("product-details-div");
    product_name.classList.add("product-name");
    seller_name.style.margin = "5px 0";
    quantity_div.classList.add("quantity-div");
    quantity.classList.add("quantity");
    remove_btn.classList.add("remove-btn");
    product_price.classList.add("product-price");
    edit_product.classList.add("remove-btn");
    edit_icon.classList.add("fa", "fa-pencil");
    quantity.style.color = "green";
    quantity.style.fontSize = "20px";
    quantity.style.margin = "0";

    quantity_div.appendChild(quantity);
    product_details_div.appendChild(product_name)
    product_details_div.appendChild(seller_name)
    product_details_div.appendChild(product_description)
    product_details_div.appendChild(quantity_div)
    product_details_div.appendChild(remove_btn);
    product_details_div.appendChild(edit_product);

    cart_item_div.appendChild(product_image);
    cart_item_div.appendChild(product_details_div);
    cart_item_div.appendChild(product_price);
    products_cart.appendChild(cart_item_div);




    remove_btn.addEventListener("click", () => {
        fetch("/Seller/removeproduct", { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ product_pic: product.ProductPic }) })
            .then(result => result.json())
            .then((data) => {
                if (data.status) {
                    products_cart.removeChild(cart_item_div);
                    alert("Product Removes Successfully");
                    window.location.reload();
                }
            })
    })

    edit_product.addEventListener("click", () => {
        product_description1.value = product.ProductDescription;
        product_name1.value = product.ProductName;
        product_stock.value = product.Stock;
        product_price1.value = product.Price;
        product_pic.value = "";
        image.style.backgroundImage = `url(../Product_images/${product.ProductPic})`;
        image.style.backgroundSize = "cover";
        isUpdatingFlag = true;
        product_id = product.ProductId;
    })

}

orders_completed.addEventListener("click", () => {
    window.location.href = "/Seller/orderscompleted"
})