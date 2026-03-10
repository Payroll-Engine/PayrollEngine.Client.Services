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
    // Badge: absolutely positioned top-right in navbar
    var navbar = document.querySelector(".navbar");
    if (navbar) {
        navbar.style.position = "relative";
        var badge = document.createElement("span");
        badge.textContent = "Automator";
        badge.style.cssText = [
            "position:absolute",
            "top:50%",
            "right:1rem",
            "transform:translateY(-50%)",
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
            "line-height:1.6",
            "z-index:10"
        ].join(";");
        navbar.appendChild(badge);
    }
});

// Hide inherited members section completely
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("h1, h2, h3, h4, h5, h6").forEach(function (heading) {
        if (!heading.textContent.trim().replace(/\s+/g, ' ').includes("Inherited Members")) return;

        heading.style.display = "none";

        // hide siblings until next heading of same or higher level
        var level = parseInt(heading.tagName[1]);
        var sibling = heading.nextElementSibling;
        while (sibling) {
            var sibLevel = sibling.tagName.match(/^H([1-6])$/);
            if (sibLevel && parseInt(sibLevel[1]) <= level) break;
            var next = sibling.nextElementSibling;
            sibling.style.display = "none";
            sibling = next;
        }
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
