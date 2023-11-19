using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#4968547185c2460fb70fd6eceaf101d4")]
    public class DebuggingModeYG : MonoBehaviour
    {
        [Tooltip("?payload=\n��� ��������, ������� �� ������ ���������� � ������� Deep Linking. ������ �������� �����, ��������, debug � �������� ���� ������, ��������, 123. ��������� debug123.")]
        public string payloadPassword = "debug123";
        [Tooltip("����������� ������ ���������� � Unity Editor")]
        public bool debuggingInEditor;

        [Serializable]
        public class LeaderboardTest
        {
            public LeaderboardYG leaderboardYG;
            public InputField nameLbInputField;
            public InputField scoreLbInputField;
        }
        public LeaderboardTest leaderboard;

        public static DebuggingModeYG Instance { get; private set; }

        private Canvas canvas;
        private Transform tr;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                if (!canvas) canvas = GetComponent<Canvas>();
                canvas.enabled = false;

                if (YandexGame.SDKEnabled) GetDataEvent();
            }
        }

        private void OnEnable() => YandexGame.GetDataEvent += GetDataEvent;
        private void OnDisable() => YandexGame.GetDataEvent -= GetDataEvent;

        public void GetDataEvent()
        {
            bool draw = false;
#if UNITY_EDITOR
            if (debuggingInEditor) draw = true;
#else
            if (YandexGame.EnvironmentData.payload == payloadPassword) draw = true;
#endif
            if (draw)
            {
                if (!canvas) canvas = GetComponent<Canvas>();
                canvas.enabled = true;

                if (!tr) tr = transform;

                tr.Find("Panel").Find("LanguageDebug").GetChild(0).GetComponent<Text>().text = YandexGame.savesData.language;

                string playerId = YandexGame.playerId;
                if (playerId.Length > 10)
                    playerId = playerId.Remove(10) + "...";

                tr.Find("Panel").Find("DebugData").GetChild(0).GetComponent<Text>().text = "playerName - " + YandexGame.playerName +
                    "\nplayerId - " + playerId +
                    "\nauth - " + YandexGame.auth +
                    "\nSDKEnabled - " + YandexGame.SDKEnabled +
                    "\ninitializedLB - " + YandexGame.initializedLB +
                    "\nphotoSize - " + YandexGame.photoSize +
                    "\ndomain - " + YandexGame.EnvironmentData.domain +
                    "\ndeviceType - " + YandexGame.EnvironmentData.deviceType +
                    "\nisMobile - " + YandexGame.EnvironmentData.isMobile +
                    "\nisDesktop - " + YandexGame.EnvironmentData.isDesktop +
                    "\nisTablet - " + YandexGame.EnvironmentData.isTablet +
                    "\nisTV - " + YandexGame.EnvironmentData.isTV +
                    "\nisTablet - " + YandexGame.EnvironmentData.isTablet +
                    "\nappID - " + YandexGame.EnvironmentData.appID +
                    "\nbrowserLang - " + YandexGame.EnvironmentData.browserLang +
                    "\npayload - " + YandexGame.EnvironmentData.payload +
                    "\npromptCanShow - " + YandexGame.EnvironmentData.promptCanShow +
                    "\nreviewCanShow - " + YandexGame.EnvironmentData.reviewCanShow;
            }
        }

        public void GetDataButton()
        {
            YandexGame.GetDataEvent?.Invoke();
        }

        public void AuthCheckButton()
        {
            GameObject.FindObjectOfType<YandexGame>()._RequestAuth();
        }

        public void AuthDialogButton()
        {
            GameObject.FindObjectOfType<YandexGame>()._OpenAuthDialog();
        }

        public void FullAdButton()
        {
            YandexGame.FullscreenShow();
        }

        public void VideoAdButton()
        {
            if (!tr) tr = transform;
            int id = int.Parse(tr.Find("Panel").Find("RewardAd").GetChild(0).GetComponent<InputField>().text);
            YandexGame.RewVideoShow(id);
        }

        public void StickyAdShowButton() => YandexGame.StickyAdActivity(true);
        public void StickyAdHideButton() => YandexGame.StickyAdActivity(false);


        public static Action onRBTRecalculate;
        public void RBTRecalculateButton()
        {
            onRBTRecalculate?.Invoke();
        }

        public static Action onRBTExecuteCode;
        public void RBTExecuteCodeButton()
        {
            onRBTExecuteCode?.Invoke();
        }

        public static Action<bool> onRBTActivity;
        public void RBTActivityButton(bool ativity)
        {
            onRBTActivity?.Invoke(ativity);
        }

        public void RedefineLangButton()
        {
            GameObject.FindObjectOfType<YandexGame>()._LanguageRequest();
        }

        public void SwitchLanguage(Text text)
        {
            YandexGame.SwitchLanguage(text.text);
        }

        public void PromptDialogButton()
        {
            YandexGame.PromptShow();
        }

        public void ReviewButton()
        {
            YandexGame.ReviewShow(false);
        }

        public void BuyPurchaseButton()
        {
            if (!tr) tr = transform;
            string id = tr.Find("Panel").Find("PurchaseID").GetChild(0).GetComponent<InputField>().text;
            YandexGame.BuyPayments(id);
        }

        public void DeletePurchaseButton()
        {
            if (!tr) tr = transform;
            string id = tr.Find("Panel").Find("PurchaseID").GetChild(0).GetComponent<InputField>().text;
            YandexGame.ConsumePurchaseByID(id);
        }

        public void DeleteAllPurchasesButton()
        {
            YandexGame.ConsumePurchases();
        }

        public void SaveButton()
        {
            YandexGame.SaveProgress();
        }

        public void LoadButton()
        {
            YandexGame.LoadProgress();
        }

        public void ResetSaveButton()
        {
            YandexGame.ResetSaveProgress();
        }

        public void SceneButton(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void NewNameLB()
        {
            leaderboard.leaderboardYG.nameLB = leaderboard.nameLbInputField.text;
            leaderboard.leaderboardYG.UpdateLB();
        }

        public void NewScoreLB()
        {
            YandexGame.NewLeaderboardScores(leaderboard.leaderboardYG.nameLB,
                int.Parse(leaderboard.scoreLbInputField.text));
        }

        public void NewScoreLBTimeConvert()
        {
            YandexGame.NewLBScoreTimeConvert(leaderboard.leaderboardYG.nameLB,
                float.Parse(leaderboard.scoreLbInputField.text));
        }
    }
}
