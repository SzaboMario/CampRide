// CampRide JS segédfüggvények

window.downloadCsv = function (filename, content) {
    var bom = '\uFEFF'; // UTF-8 BOM az Excel kompatibilitáshoz
    var blob = new Blob([bom + content], { type: 'text/csv;charset=utf-8;' });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

window.downloadFile = function (filename, contentBase64, mimeType) {
    var bytes = atob(contentBase64);
    var arr = new Uint8Array(bytes.length);
    for (var i = 0; i < bytes.length; i++) arr[i] = bytes.charCodeAt(i);
    var blob = new Blob([arr], { type: mimeType });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};
