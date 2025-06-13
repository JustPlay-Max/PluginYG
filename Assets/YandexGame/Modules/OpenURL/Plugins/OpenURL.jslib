mergeInto(LibraryManager.library,
{
	GetURLFromPage: function () {
        var returnStr = (window.location != window.parent.location) ? document.referrer : document.location.href;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
		
        return buffer;
    },
	
	OpenURL: function (url) {
		window.open(UTF8ToString(url), "_blank");
	
		//var a = document.createElement("a");
		//a.setAttribute("href", UTF8ToString(url));
		//a.setAttribute("target", "_blank");
		//a.click();
	},
	
	GetAllGames_js: function () {
        var returnStr = allGamesData;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	}
});