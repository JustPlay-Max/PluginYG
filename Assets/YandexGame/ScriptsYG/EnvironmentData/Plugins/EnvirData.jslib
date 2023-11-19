
mergeInto(LibraryManager.library,
{
	InitEnvironmentData_js: function()
	{
		var returnStr = environmentData;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},	
	
	RequestingEnvironmentData_js: function (sendback) {
        RequestingEnvironmentData(sendback);
    }
});