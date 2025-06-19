mergeInto(LibraryManager.library, {
    OpenURL: function(urlPtr) {
        var url = UTF8ToString(urlPtr);  // Convert the pointer to a string
        window.open(url, "_blank");      // Open the URL in a new tab
    }
});
