var FileIO = {

  SetKey_LocalStorage_js : function(key, value) {
	try {
		localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
	}
	catch (e) {
		console.error('Save to Local Storage error: ', e.message);
	}
  },

  GetKey_LocalStorage_js : function(key) {
    var returnStr = localStorage.getItem(UTF8ToString(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  HasKey_LocalStorage_js : function(key) {
	try {
		if (localStorage.getItem(UTF8ToString(key))) {
		  return 1;
		}
		else {
		  return 0;
		}
	}
	catch (e) {
		console.error('Has key in Local Storage error: ', e.message);
		return 0;
	}
  },

  DeleteKey_LocalStorage_js : function(key) {
    localStorage.removeItem(UTF8ToString(key));
  }
};

mergeInto(LibraryManager.library, FileIO);