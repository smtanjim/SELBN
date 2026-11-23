/* ===== SEL Integrated UI Script ===== */
const $ = (sel, root = document) => root.querySelector(sel);
const $$ = (sel, root = document) => [...root.querySelectorAll(sel)];

function showAlert(el, type, msg) {
  if (!el) return;
  el.className = `alert ${type} show`;
  el.textContent = msg;
}

/* ---------- LOGIN / REGISTER PAGE ---------- */
function initLogin() {
  const form = $("#loginForm");
  if (!form) return;

  const alertBox = $("#alertBox");
  const idInput = $("#identifier");
  const idLabel = $("#idLabel");
  const idIcon = $("#idIcon");
  const pwd = $("#password");
  const eyeBtn = $("#eye");
  const switchLink = $("#switchLink");
  let isSignup = false;

  // Email / Username label toggle
  $$('input[name="mode"]').forEach(radio => {
    radio.addEventListener("change", () => {
      if (radio.value === "username") {
        idLabel.innerHTML = 'Username or Email <span class="req">*</span>';
        idInput.type = "text";
        idInput.placeholder = "Enter your email address";
        if (idIcon) idIcon.textContent = "👤";
      } else {
        idLabel.innerHTML = 'Email Address <span class="req">*</span>';
        idInput.type = "email";
        idInput.placeholder = "Enter your email address";
        if (idIcon) idIcon.textContent = "✉";
      }
    });
  });

  // Password visibility toggle
  if (eyeBtn && pwd) {
    eyeBtn.addEventListener("click", () => {
      const hidden = pwd.type === "password";
      pwd.type = hidden ? "text" : "password";
      eyeBtn.textContent = hidden ? "🙈" : "👁";
    });
  }

  // Toggle between Sign In (/login) and Sign Up (/register)
  if (switchLink) {
    switchLink.addEventListener("click", e => {
      e.preventDefault();
      isSignup = !isSignup;
      form.action = isSignup ? "/register" : "/login";
      $("#title").textContent = isSignup ? "Create Account" : "Sign In";
      $("#submitText").textContent = isSignup ? "Sign Up" : "Sign In";
      $("#switchText").textContent = isSignup ? "Already have an account? " : "Don't have an account? ";
      switchLink.textContent = isSignup ? "Sign In" : "Sign Up";

      $$(".signup-only").forEach(el => el.classList.toggle("hidden", !isSignup));
      const nameField = $("#name");
      if (nameField) nameField.required = isSignup;
    });
  }

  // Client-side empty check before MVC form POST
  form.addEventListener("submit", e => {
    if (!idInput.value.trim() || !pwd.value.trim()) {
      e.preventDefault();
      showAlert(alertBox, "error", "Please enter both email and password.");
    }
  });
}

/* ---------- DASHBOARD SIDEBAR NAVIGATION ---------- */
function initDashboardNav() {
  const navLinks = $$(".sidebar .nav a[data-target]");
  if (!navLinks.length) return;

  navLinks.forEach(link => {
    link.addEventListener("click", e => {
      e.preventDefault();
      navLinks.forEach(a => a.classList.remove("active"));
      link.classList.add("active");
      const target = document.getElementById(link.dataset.target);
      if (target) target.scrollIntoView({ behavior: "smooth" });
    });
  });
}

document.addEventListener("DOMContentLoaded", () => {
  initLogin();
  initDashboardNav();
});