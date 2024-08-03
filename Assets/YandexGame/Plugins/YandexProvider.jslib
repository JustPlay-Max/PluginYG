mergeInto(LibraryManager.library,
{
	InitGame_js: function ()
	{
		InitGame();
	},

	FullAdShow: function ()
	{
		FullAdShow();
	},

    RewardedShow: function (id)
	{
		RewardedShow(id);
	},

	ReviewInternal: function()
	{
		Review();
	},
	
	PromptShowInternal: function()
	{
		PromptShow();
	},
	
	StickyAdActivityInternal: function(show)
	{
		StickyAdActivity(show);
	},
	
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
	
	GameplayStart_js: function () {
		if (ysdk !== null && ysdk.features !== undefined && ysdk.features.GameplayAPI !== undefined) {
			ysdk.features.GameplayAPI.start();
		}
		else {
			if (ysdk == null) console.error('Gameplay start rejected. The SDK is not initialized!');
			else console.error('Gameplay start undefined!');
		}
	},
	
	GameplayStop_js: function () {
		if (ysdk !== null && ysdk.features !== undefined && ysdk.features.GameplayAPI !== undefined) {
			ysdk.features.GameplayAPI.stop();
		}
		else {
			if (ysdk == null) console.error('Gameplay stop rejected. The SDK is not initialized!');
			else console.error('Gameplay stop undefined!');
		}
	},
	
	ServerTime_js: function() {
        if (ysdk !== null) {
            var serverTime = ysdk.serverTime().toString();
            var lengthBytes = lengthBytesUTF8(serverTime) + 1;
            var stringOnWasmHeap = _malloc(lengthBytes);
            stringToUTF8(serverTime, stringOnWasmHeap, lengthBytes);
            return stringOnWasmHeap;
        }
        return 0;
    },
	
	SetFullscreen_js: function (fullscreen) {
		if (ysdk !== null) {
			if (fullscreen) {
				if (ysdk.screen.fullscreen.status != 'on')
					ysdk.screen.fullscreen.request();
			}
			else if (ysdk.screen.fullscreen.status != 'off')
				ysdk.screen.fullscreen.exit();
		}
	},
	
	IsFullscreen_js: function () {
		if (ysdk !== null) {
			if (ysdk.screen.fullscreen.status == 'on')
				return true;
			else
				return false;
		}
		return false;
	}
});