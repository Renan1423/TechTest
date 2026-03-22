using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    //Only used by the CheatsManager
    public int TimerDuration { get; private set; }

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _timeCountText;

    [SerializeField]
    private Image _timeFill;

    [SerializeField]
    private Transform _timeTextTrans;

    [Space(20), Header("Events")]
    [SerializeField]
    private UnityEvent OnTimerCompletedEvent;
    private Timer _timer;

    private DG.Tweening.Core.TweenerCore<
        Vector3,
        Vector3,
        DG.Tweening.Plugins.Options.VectorOptions
    > _timerTransTween;

    private Color _initialFillColor;

    private void OnEnable()
    {
        Invoke(nameof(SetupDelegates), 0.05f);
    }

    private void SetupDelegates()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        _initialFillColor = _timeFill.color;
    }

    public void ResetTimer(int timerDuration)
    {
        if (_timer == null)
            _timer = gameObject.AddComponent<Timer>();

#if UNITY_EDITOR
        TimerDuration = timerDuration;
#endif

        _timer.SetupTimer(timerDuration);
        _timer.OnTimerStarted.AddListener(OnTimerStarted);
        _timer.OnTimerTick.AddListener(OnTimerTick);
        _timer.OnTimerCompleted.AddListener(OnTimerCompleted);

        _timer.StartTimer();

        if (_initialFillColor != null)
            _timeFill.color = _initialFillColor;
    }

    private void OnTimerStarted()
    {
        OnTimerTick();
    }

    private void OnTimerTick()
    {
        _timeCountText.text = Mathf.RoundToInt(_timer.TimeCount).ToString("D3");
        _timeFill.fillAmount = 1 - _timer.GetProgression();

        AnimateTimerTransform();
        if (_timer.GetProgression() >= 0.65f)
            LerpImageColor();
    }

    private void AnimateTimerTransform()
    {
        if ((Mathf.Abs(_timer.TimeCount) - Mathf.Floor(_timer.TimeCount)) >= 0.5f)
            return;

        if (_timerTransTween != null)
        {
            if (_timerTransTween.IsPlaying())
                return;

            _timerTransTween.Kill();
        }

        _timeTextTrans.localScale = new Vector2(0.5f, 1.5f);
        _timerTransTween = _timeTextTrans.DOScale(Vector2.one, 0.5f);
    }

    private void LerpImageColor()
    {
        Color baseColor = _initialFillColor;
        Color targetColor = new Color(254f, 112f, 130f, 255f) / 255f;

        _timeFill.color = Color.Lerp(baseColor, targetColor, _timer.GetProgression());
    }

    private void OnTimerCompleted()
    {
        OnTimerCompletedEvent?.Invoke();
    }

    private void OnDestroy()
    {
        if (!_timer)
            return;
        _timer.OnTimerStarted.RemoveAllListeners();
        _timer.OnTimerTick.RemoveAllListeners();
        _timer.OnTimerCompleted.RemoveAllListeners();

        Destroy(_timer);
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        bool isPaused = newGameState == GameState.Paused;
        if (_timer != null)
            _timer.ToggleTimerPause(isPaused);
    }
}
