mergeInto(LibraryManager.library,
{
	AuthorizationCheck: function (playerPhotoSize, scopes)
	{
		AuthorizationCheck(UTF8ToString(playerPhotoSize), scopes);
	},
	
	OpenAuthDialog: function (playerPhotoSize, scopes)
	{
		OpenAuthDialog(UTF8ToString(playerPhotoSize), scopes);
	},
	
	SaveYG: function (jsonData, flush)
	{
		SaveCloud(UTF8ToString(jsonData), flush);
	},
	
	LoadYG: function ()
	{
		LoadCloud();
	},
	
	InitLeaderboard: function ()
	{
		InitLeaderboard();
	},
	
	SetLeaderboardScores: function (nameLB, score)
	{
		SetLeaderboardScores(UTF8ToString(nameLB), score);
	},
	
	GetLeaderboardScores: function (nameLB, maxPlayers, quantityTop, quantityAround, photoSizeLB, auth)
	{
		GetLeaderboardScores(UTF8ToString(nameLB), maxPlayers, quantityTop, quantityAround, UTF8ToString(photoSizeLB), auth);
	},

	FullAdShow: function ()
	{
		FullscreenShow();
	},

    RewardedShow: function (id)
	{
		RewardedShow(id);
	},
	
	LanguageRequestInternal: function ()
	{
		LanguageRequest();
	},
	
	RequestingEnvironmentData: function()
	{
		RequestingEnvironmentData();
	},	

	ReviewInternal: function()
	{
		Review();
	},
	
	ActivityRTB1: function(state)
	{
		ActivityRTB1(state);
	},
	
	ActivityRTB2: function(state)
	{
		ActivityRTB2(state);
	},
	
	ExecuteCodeRTB1: function()
	{
		ExecuteCodeRTB1();
	},
	
	ExecuteCodeRTB2: function()
	{
		ExecuteCodeRTB2();
	},
	
	RecalculateRTB1: function(_width, _height, _left, _top)
	{
		RecalculateRTB1(
			UTF8ToString(_width),
			UTF8ToString(_height),
			UTF8ToString(_left),
			UTF8ToString(_top));
	},
	
	RecalculateRTB2: function(_width, _height, _left, _top)
	{
		RecalculateRTB2(
			UTF8ToString(_width),
			UTF8ToString(_height),
			UTF8ToString(_left),
			UTF8ToString(_top));
	},
	
	PaintRBTInternal: function(rbt)
	{
		PaintRBT(UTF8ToString(rbt));
	},
	
	StaticRBTDeactivate: function()
	{
		StaticRBTDeactivate();
	},
	
	BuyPaymentsInternal: function(id)
	{
		BuyPayments(UTF8ToString(id));
	},
	
	GetPaymentsInternal: function()
	{
		GetPayments();
	},
	
	DeletePurchaseInternal: function(id)
	{
		DeletePurchase(UTF8ToString(id));
	},
	
	DeleteAllPurchasesInternal: function()
	{
		DeleteAllPurchases();
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
    }
});

var FileIO = {

  SaveToLocalStorage : function(key, data) {
    localStorage.setItem(Pointer_stringify(key), Pointer_stringify(data));
  },

  LoadFromLocalStorage : function(key) {
    var returnStr = localStorage.getItem(Pointer_stringify(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  RemoveFromLocalStorage : function(key) {
    localStorage.removeItem(Pointer_stringify(key));
  },

  HasKeyInLocalStorage : function(key) {
    if (localStorage.getItem(Pointer_stringify(key))) {
      return 1;
    }
    else {
      return 0;
    }
  }
};

mergeInto(LibraryManager.library, FileIO);