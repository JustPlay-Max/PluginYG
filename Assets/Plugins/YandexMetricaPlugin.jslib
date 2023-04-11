const library = {
  
  // Class definition.

  $yandexMetrica: {
    yandexMetricaSend: function (eventName, eventData) {
      const eventDataJson = eventData === '' ? undefined : JSON.parse(eventData);
      ym(window.yandexMetricaCounterId, 'reachGoal', eventName, eventDataJson);
    },
  },

  // External C# calls.

  YandexMetricaSendInternal: function (eventNamePtr, eventDataPtr) {
    const eventName = UTF8ToString(eventNamePtr);
    const eventData = UTF8ToString(eventDataPtr);
    try {
      yandexMetrica.yandexMetricaSend(eventName, eventData);
    } catch (e) {
      console.error('Yandex Metrica send evnet error: ', e.message);
    }
  },
}

autoAddDeps(library, '$yandexMetrica');
mergeInto(LibraryManager.library, library);
