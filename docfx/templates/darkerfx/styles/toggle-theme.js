// Collapse inherited members section
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("h3, h2").forEach(function (heading) {
        if (heading.textContent.trim() === "Inherited Members") {
            var list = heading.nextElementSibling;
            if (!list) return;

            heading.style.cursor = "pointer";
            heading.style.userSelect = "none";

            var indicator = document.createElement("span");
            indicator.className = "inherited-toggle";
            indicator.textContent = " ▶";
            heading.appendChild(indicator);

            list.style.display = "none";

            heading.addEventListener("click", function () {
                var collapsed = list.style.display === "none";
                list.style.display = collapsed ? "" : "none";
                indicator.textContent = collapsed ? " ▼" : " ▶";
            });
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
