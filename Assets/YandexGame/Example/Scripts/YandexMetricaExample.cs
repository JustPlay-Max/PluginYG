using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace YG.Example
{
    public class YandexMetricaExample : MonoBehaviour
    {
        public void TestSend1(string someEvent)
        {
            YandexMetrica.Send(someEvent);
        }

        public void TestSend1()
        {
            TestSend1("SomeEvent1");
        }

        public void TestSend2()
        {
            var eventParams2 = new Dictionary<string, string>
            {
                { "Complete", "1" },
                { "Money", "1500" },
            };

            YandexMetrica.Send("SomeEvent2", eventParams2);
        }

        public void TestSend3()
        {
            var eventParams3 = new Dictionary<string, string>
            {
                { "is_string", "RUB" },
                { "is_int", 1.ToString() },
                { "is_true", true.ToString() },
                { "is_false", false.ToString() },
            };

            YandexMetrica.Send("SomeEvent3", eventParams3);
        }

        public void TestSend4()
        {
            var eventParams3 = new Dictionary<string, string>
            {
                { "is_string", "RUB" },
                { "is_int", 1.ToString() },
                { "is_float", 2.5f.ToString(CultureInfo.InvariantCulture) },
                { "is_true", true.ToString() },
                { "is_false", false.ToString() },
                { "null_value", null },  // Проигнорируется и не будет отправленно 
                { string.Empty, null }   // Проигнорируется и не будет отправленно 
            };

            YandexMetrica.Send("SomeEvent4", eventParams3);
        }

        public void TestSend5()
        {
            var eventParams3 = new Dictionary<string, string>
            {
                { "null_value", null }
            };

            // Отправится как просто евент без параметров
            YandexMetrica.Send("SomeEvent5", eventParams3);
        }
    }
}