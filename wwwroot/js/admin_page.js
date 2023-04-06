var sellers_div = document.getElementById("sellers-div");


fetch("/Seller/unauthorizedsellers", { method: 'GET' })
    .then(result => result.json())
    .then((sellers) => {
        sellers.forEach((seller) => {
            AppendSeller(seller);
        })
    })

function AppendSeller(seller) {
    var seller_div = document.createElement("div");
    var company_name = document.createElement("h3");
    var seller_address = document.createElement("p");
    var seller_gst = document.createElement("p");
    var accept = document.createElement("button");
    accept.className = "descr_btns";
    company_name.innerText = seller.SellerCompany;
    seller_address.innerText = "Location: " + seller.Address;
    seller_gst.innerText = "GSTIN: " + seller.Gst;
    accept.innerText = "Authorize";
    seller_div.appendChild(company_name);
    seller_div.appendChild(seller_address);
    seller_div.appendChild(seller_gst);
    seller_div.appendChild(accept);
    seller_div.className = "seller-div";
    sellers_div.appendChild(seller_div);

    accept.addEventListener("click", () => {
        fetch("/Admin/authorizeseller", { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ seller_id: seller.SellerId }) })
            .then(res => res.json())
            .then((result) => {
                if (result.status) {
                    alert("Authorization successfull");
                    sellers_div.removeChild(seller_div);
                }
            })
    })
}