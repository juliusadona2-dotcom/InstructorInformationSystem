// Ask for confirmation before submitting a form marked with data-confirm.
document.addEventListener("submit", function (event) {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) return;

    const confirmationMessage = form.dataset.confirm;
    if (confirmationMessage && !window.confirm(confirmationMessage)) {
        event.preventDefault();
    }
});

// ===== Light / dark theme toggle =====
(function () {
    const root = document.documentElement;
    const button = document.getElementById("themeToggle");
    if (!button) return;

    function apply(theme) {
        root.setAttribute("data-theme", theme);
        const goingTo = theme === "dark" ? "light" : "dark";
        button.setAttribute("aria-label", "Switch to " + goingTo + " mode");
        button.setAttribute("title", "Switch to " + goingTo + " mode");
        const text = button.querySelector(".theme-toggle-text");
        if (text) text.textContent = goingTo === "light" ? "Light mode" : "Dark mode";
    }

    apply(root.getAttribute("data-theme") === "dark" ? "dark" : "light");

    button.addEventListener("click", function () {
        const next = root.getAttribute("data-theme") === "dark" ? "light" : "dark";
        apply(next);
        try { localStorage.setItem("iis-theme", next); } catch (e) { }
    });
})();

// ===== Sidebar drawer (mobile) =====
(function () {
    const sidebar = document.getElementById("sidebar");
    const scrim = document.getElementById("sidebarScrim");
    const openButton = document.getElementById("sidebarOpen");
    if (!sidebar || !openButton) return;

    function setOpen(open) {
        sidebar.classList.toggle("open", open);
        if (scrim) scrim.classList.toggle("show", open);
    }

    openButton.addEventListener("click", function () { setOpen(true); });
    if (scrim) scrim.addEventListener("click", function () { setOpen(false); });
    document.addEventListener("keydown", function (event) { if (event.key === "Escape") setOpen(false); });
    sidebar.querySelectorAll("a.sb-link").forEach(function (link) {
        link.addEventListener("click", function () { setOpen(false); });
    });
})();
