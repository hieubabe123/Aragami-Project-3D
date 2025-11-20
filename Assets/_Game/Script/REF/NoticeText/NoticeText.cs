using DG.Tweening;
using TMPro;
using UnityEngine;

public class NoticeText : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI noticeText;

    private static NoticeText _ins;
    public static NoticeText Instance {
        get {
            if (_ins != null) return _ins;
            var g = Resources.Load<GameObject>($"{typeof(NoticeText).Name}/{typeof(NoticeText).Name}");
            if (g == null) {
                Debug.LogError($"File not exist in path: Resources/{typeof(NoticeText).Name}/{typeof(NoticeText).Name}");
            }
            else _ins = GameObject.Instantiate(g).GetComponent<NoticeText>();

            return _ins;
        }
    }

    private void Awake() {
        if (_ins != null && _ins.gameObject.GetInstanceID() != gameObject.GetInstanceID()) {
            DestroyImmediate(gameObject);
            return;
        }

        _ins = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowNotice(string message, bool errorMessage = false) {
        if (noticeText == null) {
            var g = new GameObject("Text");
            g.transform.SetParent(transform);
            noticeText = g.AddComponent<TextMeshProUGUI>();
            noticeText.fontSize = 50;
            noticeText.raycastTarget = false;
            //return;
        }

        noticeText.gameObject.SetActive(true);
        noticeText.DOKill();
        noticeText.text = message;
        noticeText.color = Color.white;
        noticeText.outlineColor = errorMessage ? Color.red : Color.black;
        (noticeText.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
        (noticeText.transform as RectTransform).DOAnchorPosY(100, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        noticeText.DOFade(0, .3f).SetDelay(0.7f).SetUpdate(true).OnComplete(() => {
            if (noticeText && noticeText.gameObject) noticeText.gameObject.SetActive(false);
        });
    }

}