mergeInto(LibraryManager.library,
{
	InitCloudStorage_js: function()
	{
		var returnStr = cloudSaves;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},	

	SaveYG: function (jsonData, flush)
	{
		SaveCloud(UTF8ToString(jsonData), flush);
	},
	
	LoadYG: function (sendback) {
        LoadCloud(sendback);
    }
});