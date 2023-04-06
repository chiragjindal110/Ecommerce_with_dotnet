var start = 0;
var load_more = document.getElementById("load_more");
function getProducts() {
    console.log("hello");
    var v = { "start": start };
    fetch("/Home/getProducts", { method: "POST", headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(v) })
        .then(res => res.json())
        .then((d) => {
            console.log(d);
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

    image.setAttribute("src", "/Product_Images/" + product.ProductPic );
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
        localStorage.setItem("products", JSON.stringify([{ ProductId: product.ProductId, Quantity: 1 }]));
        window.location.href = "/Checkout/checkout";
    })
}



load_more.addEventListener("click", () => {
    load_more.disabled = true;
    getProducts();
})

var login_text = document.getElementById("login-text");
var signup_text = document.getElementById("signup-text");
var login = document.getElementById("login");
var signup = document.getElementById("signup");
var close2 = document.getElementById("close");
var close1 = document.getElementById("close1");
var login_signup_back = document.getElementById("login-signup-back");

close2.onclick = () => {
    login_signup_back.style.display = "none";
}
close1.onclick = () => {
    login_signup_back.style.display = "none";
}

login_text.onclick = () => {
    login.style.display = "none";
    signup.style.display = "flex";
}

signup_text.onclick = () => {
    login.style.display = "flex";
    signup.style.display = "none";
}

var login_submit = document.getElementById('login-submit');
var signup_submit = document.getElementById('signup-submit');

login_submit.onclick = () => {
    login_submit.disabled = true;
    login_submit.innerText = "Loading..."
    var login_email = document.getElementById('login-email');
    var login_password = document.getElementById('login-password');
    fetch('/Index/authenticate', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ password: login_password.value, email: login_email.value }) })
        .then(res => res.json())
        .then((d) => {
            if (d.flag) {
                window.location.href = "/Home/Home";
            }
            else {
                alert("No such profile found");
                login_submit.disabled = false;
                login_submit.innerText = "Login"
            }
        })
}


signup_submit.onclick = () => {
    signup_submit.innerText = "Loading..."
    signup_submit.disabled = true;
    var username = document.getElementById('signup-name');
    var email = document.getElementById('signup-email');
    var password = document.getElementById('signup-password');
    var phone = document.getElementById('signup-phone');
    if (username.value.trim() == "" || email.value.trim() == "" || password.value.trim() == "" || phone.value == "") {
        alert("Fields can't be empty");
    }
    else {
        fetch('/Index/validate', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name: username.value, email: email.value, password: password.value, phone: phone.value*1 }) })
            .then(res => res.json())
            .then((d) => {
                if (d.flag) {
                    window.location.href = "/Index/verifyMail";
                }
                else {
                    alert("already exists");
                    signup_submit.disabled = false;
                    signup_submit.innerText = "Signup"
                }
            })
    }
}

var seller = document.getElementById("become-seller");
seller.addEventListener("click", () => {
    fetch('/Home/islogin', { method: 'GET' })
        .then(res => res.json())
        .then((data) => {
            if (data.flag) {
                window.location.href = "/Seller/Index";
            }
            else {
                var show = document.getElementById("login-signup-back");
                show.style.display = "flex";
            }
        })
})
var forgot_password = document.getElementById("forgot-password");
forgot_password.addEventListener("click", () => {
    forgot_password.disabled = true;
    if (login_email.value.trim() != "") {
        fetch("/Index/forgotPassword", { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ email: login_email.value }) })
            .then(res => res.json())
            .then(d => {
                if (d.status) {
                    alert("Please Change your password by clicking the link Sent to you on Mail");
                }
                else {
                    alert("Please Enter Valid Mail");
                    forgot_password.disabled = false;
                }
            })


    }
    else
        alert("Please Specify the Mail");
})