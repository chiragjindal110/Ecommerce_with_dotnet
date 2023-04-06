// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var login_text = document.getElementById("login-text");
var signup_text = document.getElementById("signup-text");
var login = document.getElementById("login");
var signup = document.getElementById("signup");

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
/*var i = document.createElement("i");
i.classList.add("fa", "fa-spinner", "fa-spin");*/

var login_email = document.getElementById('login-email');
login_submit.onclick = () => {
    login_submit.disabled = true;
    login_submit.innerText = "Loading..."
    var login_password = document.getElementById('login-password');
    fetch('/Index/authenticate', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ password: login_password.value, email: login_email.value }) })
        .then(res => res.json())
        .then((d) => {
            console.log(d);
            if (d.flag) {
                window.location.href = "/Home/Home";
            }
            else
                alert("No such profile found");
            login_submit.disabled = false;
            login_submit.innerText = "Login"
        })
}

signup_submit.onclick = () => {
    signup_submit.innerText = "Loading...";
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
                    signup_submit.innerText = "Signup";
                }
            })
    }
}

var skip = document.getElementById('continue');
skip.onclick = () => {
    window.location.href = "/Home/Home";
}

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