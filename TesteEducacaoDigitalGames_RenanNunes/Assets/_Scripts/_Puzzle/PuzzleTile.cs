using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleTile : ObjectFromPool, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<PuzzleTile> OnDragMovementRequested;
    public event Action<PuzzleTile, bool> OnDragMovementEnded;

    public Vector2 CurrentPosition => _currentPositionOnGrid;
    public Vector2 CorrectPosition => _correctPosition;

    [SerializeField]
    private PuzzleTileVisuals _puzzleTileVisuals;

    private Vector2 _currentPositionOnGrid;
    private Vector2 _correctPosition;

    private bool _canDrag;
    private Vector3 _startDragLocalPos;
    private Vector3 _targetEmptyLocalPos;
    private Vector2 _allowedDragAxis;

    public void InitializeTile(Vector2 positionOnGrid, Vector2 correctPosition)
    {
        _currentPositionOnGrid = positionOnGrid;
        _correctPosition = correctPosition;
        _puzzleTileVisuals.SetCorrectPositionText(
            (_correctPosition.x + (_correctPosition.y * 3)).ToString()
        );
        _puzzleTileVisuals.SetCurrentPositionText(
            (_currentPositionOnGrid.x + (_currentPositionOnGrid.y * 3)).ToString()
        );
    }

    // Called by PuzzleManager/Grid to grant permission to drag
    public void SetupDragParameters(bool canDrag, Vector3 emptyLocalPos, Vector2 axis)
    {
        _canDrag = canDrag;
        _targetEmptyLocalPos = emptyLocalPos;
        _allowedDragAxis = axis;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startDragLocalPos = transform.localPosition;
        _puzzleTileVisuals.SelectTile();

        // Asking the manager to check if we are next to the empty space
        OnDragMovementRequested?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canDrag)
        {
            _puzzleTileVisuals.UnselectTile();
            return;
        }

        // Converting screen mouse movement to world space
        RectTransform parentRect = (RectTransform)transform.parent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );

        // Constraining the movement strictly to the allowed axis (Horizontal or Vertical)
        Vector3 constrainedPos = _startDragLocalPos;
        if (_allowedDragAxis.x != 0)
            constrainedPos.x = localMousePos.x;
        if (_allowedDragAxis.y != 0)
            constrainedPos.y = localMousePos.y;

        // Clamping the position so the player can't drag the piece past the empty space or past its origin
        constrainedPos.x = Mathf.Clamp(
            constrainedPos.x,
            Mathf.Min(_startDragLocalPos.x, _targetEmptyLocalPos.x),
            Mathf.Max(_startDragLocalPos.x, _targetEmptyLocalPos.x)
        );
        constrainedPos.y = Mathf.Clamp(
            constrainedPos.y,
            Mathf.Min(_startDragLocalPos.y, _targetEmptyLocalPos.y),
            Mathf.Max(_startDragLocalPos.y, _targetEmptyLocalPos.y)
        );

        transform.localPosition = constrainedPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_canDrag)
        {
            _puzzleTileVisuals.UnselectTile();
            return;
        }

        // Checking if the player dragged the piece more than halfway to the empty space
        float distanceToTarget = Vector3.Distance(transform.localPosition, _targetEmptyLocalPos);
        float totalDistance = Vector3.Distance(_startDragLocalPos, _targetEmptyLocalPos);

        bool successfullyMoved = distanceToTarget < (totalDistance / 2f);

        if (successfullyMoved)
        {
            // Snapping to the new empty space
            transform.DOLocalMove(_targetEmptyLocalPos, 0.15f);
            OnDragMovementEnded?.Invoke(this, true);
        }
        else
        {
            // Snapping back to origin
            transform.DOLocalMove(_startDragLocalPos, 0.15f);
            OnDragMovementEnded?.Invoke(this, false);
        }

        _puzzleTileVisuals.SetCurrentPositionText(
            (_currentPositionOnGrid.x + (_currentPositionOnGrid.y * 3)).ToString()
        );

        _puzzleTileVisuals.UnselectTile();
        _canDrag = false;
    }

    public void SetTileImage(Sprite sprite) => _puzzleTileVisuals.SetTileImage(sprite);

    //CHEAT - Toggle currect position tip
    public void ToggleCurrentPositionTip() => _puzzleTileVisuals.ToggleCurrectPositionTip();

    //CHEAT - Toggle correct position tip
    public void ToggleCorrectPositionTip() => _puzzleTileVisuals.ToggleCorrectPositionTip();
}
