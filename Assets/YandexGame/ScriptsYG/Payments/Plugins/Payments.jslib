mergeInto(LibraryManager.library,
{
	InitPayments_js: function()
	{
		var returnStr = paymentsData;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	
	GetPayments_js: function()
	{
		GetPayments();
	},
	
	ConsumePurchase_js: function(id)
	{
		ConsumePurchase(UTF8ToString(id));
	},
	
	ConsumePurchase_js: function()
	{
		ConsumePurchases();
	},
	
	BuyPayments_js: function(id)
	{
		BuyPayments(UTF8ToString(id));
	}
});