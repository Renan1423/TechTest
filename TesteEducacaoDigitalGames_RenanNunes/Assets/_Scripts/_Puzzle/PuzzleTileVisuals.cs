using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PuzzleTileVisuals : MonoBehaviour
{
    [SerializeField]
    private Image _puzzleTileImage;

    [SerializeField]
    private Material _imageMaterial;

    [SerializeField]
    private TextMeshProUGUI _correctPositionText;

    [SerializeField]
    private TextMeshProUGUI _currentPositionText;

    [SerializeField]
    private UnityEvent _OnSelect;

    [SerializeField]
    private UnityEvent _OnUnselect;

    private Transform _transformCache;
    private DG.Tweening.Core.TweenerCore<
        Vector3,
        Vector3,
        DG.Tweening.Plugins.Options.VectorOptions
    > _scaleTween;

    private void Awake()
    {
        InstatiateMaterialCopy();
    }

    private void InstatiateMaterialCopy()
    {
        if (_puzzleTileImage != null)
        {
            _imageMaterial = Instantiate(_puzzleTileImage.material);
            _puzzleTileImage.material = _imageMaterial;
        }
    }

    private void OnEnable()
    {
        if (_transformCache == null)
            _transformCache = transform;

        _puzzleTileImage.color = new Vector4(1f, 1f, 1f, 0f);
        _puzzleTileImage.DOFade(1f, 1f);
        _transformCache.localScale = Vector2.zero;
        _transformCache.DOScale(Vector2.one, 1f);
        _transformCache.rotation = Quaternion.Euler(0f, 0f, 360f);
        _transformCache.DORotate(Vector3.zero, 1f);
    }

    public void SetTileImage(Sprite imageSprite) => _puzzleTileImage.sprite = imageSprite;

    public void SetCorrectPositionText(string correctPositionText) =>
        _correctPositionText.text = correctPositionText;

    public void SetCurrentPositionText(string currentPositionText) =>
        _currentPositionText.text = currentPositionText;

    public void ToggleCurrectPositionTip()
    {
        GameObject currectPositionGO = _currentPositionText.gameObject;

        currectPositionGO.SetActive(!currectPositionGO.activeInHierarchy);
    }

    public void ToggleCorrectPositionTip()
    {
        GameObject correctPositionGO = _correctPositionText.gameObject;

        correctPositionGO.SetActive(!correctPositionGO.activeInHierarchy);
    }

    public void SelectTile()
    {
        _transformCache.localScale = Vector3.one;
        _scaleTween = _transformCache.DOScale(Vector2.one * 0.8f, 0.25f);

        _OnSelect?.Invoke();
    }

    public void UnselectTile()
    {
        if (_scaleTween.IsPlaying())
            _scaleTween.Kill();

        _transformCache.localScale = Vector2.one * 0.8f;
        _scaleTween = _transformCache.DOScale(Vector2.one, 0.1f);

        _OnUnselect?.Invoke();
    }
}
