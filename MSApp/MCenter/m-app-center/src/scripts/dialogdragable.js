const d = {};
const isMovable = (targ) => {
    const moveElement = targ.closest(".mdialog__header");
    if ((targ.classList.contains("note-info") && moveElement.classList?.contains("m-movable")) || (targ.classList?.contains("m-movable"))) {
        return true;
    } else {
        return false;
    }
};
document.addEventListener("mousedown", (e) => {
    const closestDialog = e.target.closest(".mdialog-wrapper");
    const title = closestDialog?.querySelector(".mdialog__header");
    if (
        (e.button === 0 && closestDialog != null && isMovable(e.target)) ||
        isMovable(e.target.parentNode)
    ) {
        d.el = closestDialog; // movable element
        d.handle = title; // enable dlg to be moved down beyond bottom
        d.mouseStartX = e.clientX;
        d.mouseStartY = e.clientY;
        d.elStartX = d.el.getBoundingClientRect().left;
        d.elStartY = d.el.getBoundingClientRect().top;
        d.el.style.position = "fixed";
        d.el.style.margin = 0;
        d.oldTransition = d.el.style.transition;
        d.el.style.transition = "none";
    }
});
document.addEventListener("mousemove", (e) => {
    if (d.el === undefined) return;
    d.el.style.left =
        Math.min(
            Math.max(d.elStartX + e.clientX - d.mouseStartX, 0),
            window.innerWidth - d.el.getBoundingClientRect().width
        ) + "px";
    d.el.style.top =
        Math.min(
            Math.max(d.elStartY + e.clientY - d.mouseStartY, 0),
            window.innerHeight - d.handle.getBoundingClientRect().height
        ) + "px";
});
document.addEventListener("mouseup", () => {
    if (d.el === undefined) return;
    d.el.style.transition = d.oldTransition;
    d.el = undefined;
});