// Inject role badge next to logo
document.addEventListener("DOMContentLoaded", function () {
    var brand = document.querySelector(".navbar-brand");
    if (brand) {
        brand.style.cssText = "display:inline-flex;align-items:center;gap:10px;";
        var title = document.createElement("span");
        title.textContent = "Payroll Engine";
        title.style.cssText = "font-size:.85rem;font-weight:700;color:#fff;letter-spacing:.08em;text-transform:uppercase;white-space:nowrap;";
        brand.appendChild(title);
        var badge = document.createElement("span");
        badge.textContent = "Client Services";
        badge.style.cssText = [
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
        brand.appendChild(badge);
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
