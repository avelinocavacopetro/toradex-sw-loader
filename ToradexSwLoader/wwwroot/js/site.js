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

function downloadFile(fileName, fileContent) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + fileContent;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function blazorLog(msg) { console.log(msg); }
