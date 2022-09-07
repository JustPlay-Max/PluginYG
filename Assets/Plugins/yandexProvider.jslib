mergeInto(LibraryManager.library,
{
	AuthorizationCheck: function (playerPhotoSize, scopes)
	{
		AuthorizationCheck(Pointer_stringify(playerPhotoSize), scopes);
	},
	
	OpenAuthDialog: function (playerPhotoSize, scopes)
	{
		OpenAuthDialog(Pointer_stringify(playerPhotoSize), scopes);
	},
	
	SaveYG: function (jsonData, flush)
	{
		SaveCloud(Pointer_stringify(jsonData), flush);
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
		SetLeaderboardScores(Pointer_stringify(nameLB), score);
	},
	
	GetLeaderboardScores: function (nameLB, maxPlayers, quantityTop, quantityAround, photoSizeLB, auth)
	{
		GetLeaderboardScores(Pointer_stringify(nameLB), maxPlayers, quantityTop, quantityAround, Pointer_stringify(photoSizeLB), auth);
	},

	FullAdShow: function ()
	{
		FullscreenShow();
	},

    RewardedShow: function (id)
	{
		RewardedShow(id);
	},
	
	LanguageRequest: function ()
	{
		LanguageRequest();
	},
	
	RequestingEnvironmentData: function()
	{
		RequestingEnvironmentData();
	},	

	Review: function()
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
	
	ActivityRTB3: function(state)
	{
		ActivityRTB3(state);
	},
	
	ActivityRTB4: function(state)
	{
		ActivityRTB4(state);
	},
	
	ActivityRTB5: function(state)
	{
		ActivityRTB5(state);
	},
	
	ActivityRTB6: function(state)
	{
		ActivityRTB6(state);
	},
	
	RenderRTB1: function()
	{
		RenderRTB1();
	},
	
	RenderRTB2: function()
	{
		RenderRTB2();
	},
	
	RenderRTB3: function()
	{
		RenderRTB3();
	},
	
	RenderRTB4: function()
	{
		RenderRTB4();
	},
	
	RenderRTB5: function()
	{
		RenderRTB5();
	},
	
	RenderRTB6: function()
	{
		RenderRTB6();
	},
	
	RecalculateRTB1: function(_width, _height, _left, _top)
	{
		RecalculateRTB1(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	RecalculateRTB2: function(_width, _height, _left, _top)
	{
		RecalculateRTB2(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	RecalculateRTB3: function(_width, _height, _left, _top)
	{
		RecalculateRTB3(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	RecalculateRTB4: function(_width, _height, _left, _top)
	{
		RecalculateRTB4(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	RecalculateRTB5: function(_width, _height, _left, _top)
	{
		RecalculateRTB5(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	RecalculateRTB6: function(_width, _height, _left, _top)
	{
		RecalculateRTB6(
			Pointer_stringify(_width),
			Pointer_stringify(_height),
			Pointer_stringify(_left),
			Pointer_stringify(_top));
	},
	
	PaintRBTInternal: function(rbt)
	{
		PaintRBT(Pointer_stringify(rbt));
	},
	
	StaticRBTDeactivate: function()
	{
		StaticRBTDeactivate();
	},
	
	BuyPaymentsInternal: function(id)
	{
		BuyPayments(Pointer_stringify(id));
	},
	
	GetPaymentsInternal: function()
	{
		GetPayments();
	},
	
	DeletePurchaseInternal: function(id)
	{
		DeletePurchase(Pointer_stringify(id));
	},
	
	DeleteAllPurchasesInternal: function()
	{
		DeleteAllPurchases();
	},
	
	PromptShowInternal: function()
	{
		PromptShow();
	},
	
	GetURLFromPage: function () {
        var returnStr = (window.location != window.parent.location) ? document.referrer : document.location.href;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
		
        return buffer;
    }
});