using UnityEngine;
using UnityEngine.UI;

namespace YG.Example
{
    public class FlagsExample : MonoBehaviour
    {
        public Text textDataOutput;

        void Start()
        {
            // Вывод в тексте всех флагов путём перебора массива:

            textDataOutput.text = string.Empty;

            if (YandexGame.flags.Length > 0)
            {
                for (int i = 0; i < YandexGame.flags.Length; i++)
                {
                    textDataOutput.text += YandexGame.flags[i].name + ": ";
                    textDataOutput.text += YandexGame.flags[i].value + "\n";
                }
            }
            else
            {
                textDataOutput.text = "No flags found!";
            }

            // Пример получения и обработки флагов:
            // Допустим, из облака мы получаем уровень сложности

            string value = YandexGame.GetFlag("difficult");

            if (value == "easy")
            {
                // Установите лёгкий уровень сложности.
                Debug.Log("Difficulty: easy");
            }
            else if (value == "middle")
            {
                // Установите средний уровень сложности.
                Debug.Log("Difficulty: middle");
            }
            else if (value == "hard")
            {
                // Установите сложный уровень сложности.
                Debug.Log("Difficulty: hard");
            }
            else
            {
                // Значение флага не определено, установите дефолтное значение.
                // Если значение не определено, метод GetFlag вернёт null.
                Debug.Log("Difficulty: middle");
            }
        }
    }
}