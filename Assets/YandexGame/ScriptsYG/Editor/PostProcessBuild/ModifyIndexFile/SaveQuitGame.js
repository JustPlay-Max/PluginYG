window.addEventListener('beforeunload', (event) => {
	if(myGameInstance != null) 
		myGameInstance.SendMessage('{{{ObjectName}}}', '{{{MethodName}}}'); 
	});