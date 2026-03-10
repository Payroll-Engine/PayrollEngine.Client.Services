// Collapse inherited members section
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("h1, h2, h3, h4, h5, h6").forEach(function (heading) {
        if (heading.textContent.trim().replace(/\s+/g, ' ') !== "Inherited Members") return;

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
