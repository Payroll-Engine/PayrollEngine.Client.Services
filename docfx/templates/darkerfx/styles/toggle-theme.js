// Inject title + right-aligned role badge
document.addEventListener("DOMContentLoaded", function () {
    // Brand: icon + title text
    var brand = document.querySelector(".navbar-brand");
    if (brand) {
        brand.style.cssText = "display:inline-flex;align-items:center;gap:10px;";
        // Remove any appName span injected by template
        brand.querySelectorAll("span").forEach(function(s) { s.remove(); });
        var title = document.createElement("span");
        title.textContent = "Client Services";
        title.style.cssText = "font-size:1rem;font-weight:700;color:#fff;letter-spacing:.06em;text-transform:uppercase;white-space:nowrap;";
        brand.appendChild(title);
    }
    // Badge: right-aligned in navbar container
    var container = document.querySelector(".navbar .container, .navbar .container-fluid, .navbar > div");
    if (container) {
        container.style.cssText = "display:flex;align-items:center;width:100%;";
        var badge = document.createElement("span");
        badge.textContent = "Automator";
        badge.style.cssText = [
            "margin-left:auto",
            "font-size:.7rem",
            "font-weight:700",
            "letter-spacing:.05em",
            "text-transform:uppercase",
            "padding:.15rem .55rem",
            "border-radius:4px",
            "border-left:3px solid #f97316",
            "background:rgba(249,115,22,.1)",
            "color:#f97316",
            "white-space:nowrap",
            "line-height:1.6"
        ].join(";");
        container.appendChild(badge);
    }
});

// Collapse inherited members section
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("h1, h2, h3, h4, h5, h6").forEach(function (heading) {
        if (!heading.textContent.trim().replace(/\s+/g, ' ').includes("Inherited Members")) return;

        // find the next sibling that is a list or div (skip hr etc.)
        var list = heading.nextElementSibling;
        while (list && list.tagName === "HR") {
            list = list.nextElementSibling;
        }
        if (!list) return;

        heading.style.cursor = "pointer";
        heading.style.userSelect = "none";

        var indicator = document.createElement("span");
        indicator.className = "inherited-toggle";
        indicator.style.marginLeft = "6px";
        indicator.style.fontSize = ".8em";
        indicator.textContent = "▶";
        heading.appendChild(indicator);

        list.style.display = "none";

        heading.addEventListener("click", function () {
            var collapsed = list.style.display === "none";
            list.style.display = collapsed ? "" : "none";
            indicator.textContent = collapsed ? "▼" : "▶";
        });
    });
});

const sw = document.getElementById("switch-style"), b = document.body;

if (sw && b) {
    sw.checked = window.localStorage && localStorage.getItem("theme") === "theme-dark" || !window.localStorage;

    b.classList.toggle("theme-dark", sw.checked)
    b.classList.toggle("theme-light", !sw.checked)

    sw.addEventListener("change", function () {
        b.classList.toggle("theme-dark", this.checked)
        b.classList.toggle("theme-light", !this.checked)

        if (window.localStorage) {
            localStorage.setItem("theme", this.checked ? "theme-dark" : "theme-light");
        }
    })
}
