window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

window.registerResizeCallback = (dotNetHelper) => {
    window.addEventListener("resize", () => {
        dotNetHelper.invokeMethodAsync("OnResize");
    });
};