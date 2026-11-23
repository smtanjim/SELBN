/* ===== SEL Frontend – main.js =====
   Pure JS, shared by login.html, error.html and all dashboards.
   Backend: ASP.NET Core, cookie authentication (cookies are sent automatically).
   >>> Controllers were not available when this was written, so every endpoint
   >>> is defined in API below. Change only these strings to match your controllers. */

const API = {
  login:    "/api/auth/login",     // POST {email|username, password}  -> {success, role, name, message}
  register: "/api/auth/register",  // POST {name, email, password, role, phone, address}
  logout:   "/api/auth/logout",    // POST
  me:       "/api/auth/me",        // GET  -> {id, name, email, role}
  // Dashboard data (GET, each returns JSON). Adjust to your controllers.
  customer: { stats: "/api/customer/stats", orders: "/api/customer/orders" },
  supplier: { stats: "/api/supplier/stats", products: "/api/supplier/products" },
  sales:    { stats: "/api/sales/stats",    leads: "/api/sales/leads" },
  accounts: { stats: "/api/accounts/stats", invoices: "/api/accounts/invoices" },
  admin:    { stats: "/api/admin/stats",    users: "/api/admin/users" }
};

// role (database enum) -> dashboard page
const ROLE_PAGE = {
  customer: "/dashboards/customer.html",
  supplier: "/dashboards/supplier.html",
  sales_executive: "/dashboards/sales.html",
  accounts_officer: "/dashboards/accounts.html",
  admin: "/dashboards/admin.html"
};

/* ---------- helpers ---------- */
const $ = (sel, root = document) => root.querySelector(sel);
const $$ = (sel, root = document) => [...root.querySelectorAll(sel)];

async function api(url, options = {}) {
  const res = await fetch(url, {
    credentials: "same-origin",
    headers: { "Content-Type": "application/json" },
    ...options,
    body: options.body ? JSON.stringify(options.body) : undefined
  });
  if (res.status === 401) { location.href = "/login.html"; throw new Error("Unauthorized"); }
  if (res.status === 403) { location.href = "/error.html?code=403"; throw new Error("Forbidden"); }
  let data = null;
  try { data = await res.json(); } catch { /* empty body */ }
  if (!res.ok) throw new Error((data && (data.message || data.error)) || `Request failed (${res.status})`);
  return data;
}

function showAlert(el, type, msg) {
  el.className = `alert ${type} show`;
  el.textContent = msg;
}
function esc(s) {
  return String(s ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
}
function money(n) { return "৳" + Number(n || 0).toLocaleString("en-BD"); }
function badge(status) {
  const s = String(status || "").toLowerCase();
  const color = ["paid", "delivered", "completed", "active", "approved", "won"].includes(s) ? "green"
    : ["pending", "processing", "new", "open"].includes(s) ? "amber"
    : ["cancelled", "overdue", "rejected", "lost"].includes(s) ? "red" : "blue";
  return `<span class="badge ${color}">${esc(status)}</span>`;
}

/* ---------- LOGIN PAGE ---------- */
function initLogin() {
  const form = $("#loginForm");
  if (!form) return;
  const alertBox = $("#alertBox");
  const idInput = $("#identifier");
  const idLabel = $("#idLabel");
  const pwd = $("#password");

  // Email / Username toggle
