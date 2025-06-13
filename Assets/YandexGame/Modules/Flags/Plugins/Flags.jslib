mergeInto(LibraryManager.library,
{
	FlagsInit_js: function() {
		var returnStr = flasgsData;
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	}
});