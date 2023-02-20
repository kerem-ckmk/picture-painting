using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("General")]
    public float distance = 7f;
    public float height = 2f;
    public float tiltAngle = 20f;
    public float panAngle = 0f;
    public int positionClampFactor = 80;
    public Vector3 finalOffset = Vector3.zero;

    [Header("Tween Speeds")]
    public float positionalTweenSpeed = 10f;
    public float rotationalTweenSpeed = 1f;

    [Header("Shake Settings")]
    public float shakeAmount = 0.7f;
    public float shakeDuration = 1f;
    public float shakeDecrease = 0.1f;

    [Header("Slide Settings")]
    public Vector2 minRangeClamp;
    public Vector2 maxRangeClamp;
    public float minZoomClamp;
    public float maxZoomClamp;
    public float slideFactor;
    public float minMaxFactor;
    public float touchFactor;


    private Camera _gameCamera;
    public Camera GameCamera
    {
        get
        {
            if (_gameCamera == null)
                _gameCamera = this.GetComponent<Camera>();

            return _gameCamera;
        }
    }

    public Transform TargetTransform;
    public Transform PivotTransform { get; private set; }

    public Vector3 FlatForwardVector
    {
        get
        {
            Vector3 forwardVector = GameCamera.transform.forward;
            forwardVector.y = 0f;
            forwardVector.Normalize();

            return forwardVector;
        }
    }

    public Vector3 FlatRightVector
    {
        get
        {
            Vector3 rightVector = GameCamera.transform.right;
            rightVector.y = 0f;
            rightVector.Normalize();

            return rightVector;
        }
    }

    private float _currentDistance;
    private float _currentHeight;
    private float _currentTiltAngle;
    private float _currentPanAngle;
    private Vector3 _currentFinalOffset;
    private Vector3 _currentTargetPosition;
    private Quaternion _currentTargetRotation;

    private float _origDistance;
    private float _origHeight;
    private float _origTiltAngle;
    private float _origPanAngle;
    private int _origPositionClampFactor;
    private Vector3 _origFinalOffset;
    private float _origPositionalTweenSpeed;
    private float _origRotationalTweenSpeed;

    private float _shakeLeftTime;
    private Vector3 _shakeVector;

    private bool _touchProtect;
    private float _maxFactorX;
    private float _maxFactorY;
    private Vector2 _previouMousePosition;
    private Vector2 _currentMousePosition;
    private Vector2 _deltaMousePosition;
    private List<RaycastResult> _tempList;
    private bool _isOverUI;

    public void Initialize()
    {
        _origDistance = distance;
        _origHeight = height;
        _origTiltAngle = tiltAngle;
        _origPanAngle = panAngle;
        _origPositionClampFactor = positionClampFactor;
        _origFinalOffset = finalOffset;

        _origPositionalTweenSpeed = positionalTweenSpeed;
        _origRotationalTweenSpeed = rotationalTweenSpeed;

        _tempList = new List<RaycastResult>(4);
    }

    public void ResetParameters()
    {
        distance = _origDistance;
        height = _origHeight;
        tiltAngle = _origTiltAngle;
        panAngle = _origPanAngle;
        positionClampFactor = _origPositionClampFactor;
        finalOffset = _origFinalOffset;

        positionalTweenSpeed = _origPositionalTweenSpeed;
        rotationalTweenSpeed = _origRotationalTweenSpeed;

        _shakeVector = Vector3.zero;
    }

    public void SetTarget(Transform targetTransform)
    {
        TargetTransform = targetTransform;
    }

    public void SetPivot(Transform pivotTransform)
    {
        PivotTransform = pivotTransform;
    }

    public void ForceUpdateInstantly()
    {
        if (TargetTransform != null)
        {
            UpdateForTargetChase(true);
        }
    }

    private void LateUpdate()
    {
        if (TargetTransform != null)
        {
            UpdateForTargetChase();
            CalculateMinMaxRange();
        }
    }

    private void Update()
    {
        if (TargetTransform == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _isOverUI = IsPointerOverUIObject();
            _currentMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && !_touchProtect && !_isOverUI)
        {
            _previouMousePosition = _currentMousePosition;
            _currentMousePosition = Input.mousePosition;
            _deltaMousePosition = _currentMousePosition - _previouMousePosition;

            finalOffset.x += _deltaMousePosition.x * slideFactor;
            finalOffset.y -= _deltaMousePosition.y * slideFactor;

            finalOffset.x = Mathf.Clamp(finalOffset.x, minRangeClamp.x - _maxFactorX, maxRangeClamp.x + _maxFactorX);
            finalOffset.y = Mathf.Clamp(finalOffset.y, minRangeClamp.y - _maxFactorY, maxRangeClamp.y + _maxFactorY);
        }
#if UNITY_EDITOR
        distance = Mathf.Clamp(distance - (Input.GetAxis("Mouse ScrollWheel") * minMaxFactor), minZoomClamp, maxZoomClamp);
#endif

        if (Input.touchCount == 2)
        {
            _touchProtect = true;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;

            distance = Mathf.Clamp(distance - (difference * touchFactor), minZoomClamp, maxZoomClamp);
        }

        if (Input.touchCount == 0)
            _touchProtect = false;
    }
    public void CalculateMinMaxRange()
    {
        float maxRangeX = maxRangeClamp.x;
        float maxRangeY = maxRangeClamp.y;

        float differanceX = 15 - maxRangeX;
        float differanceY = 10 - maxRangeY;

        float differanceZoom = maxZoomClamp - minZoomClamp;

        _maxFactorX = (differanceX / differanceZoom) * (maxZoomClamp - distance);
        _maxFactorY = (differanceY / differanceZoom) * (maxZoomClamp - distance);
    }

    private void UpdateForTargetChase(bool isInstant = false)
    {
        Debug.Assert(TargetTransform != null);

        _currentDistance = isInstant ? distance : Mathf.Lerp(_currentDistance, distance, positionalTweenSpeed * Time.deltaTime);
        _currentHeight = isInstant ? height : Mathf.Lerp(_currentHeight, height, positionalTweenSpeed * Time.deltaTime);

        _currentPanAngle = isInstant ? panAngle : Mathf.Lerp(_currentPanAngle, panAngle, rotationalTweenSpeed * Time.deltaTime);
        _currentTiltAngle = isInstant ? tiltAngle : Mathf.Lerp(_currentTiltAngle, tiltAngle, rotationalTweenSpeed * Time.deltaTime);

        _currentFinalOffset = isInstant ? finalOffset : Vector3.Lerp(_currentFinalOffset, finalOffset, positionalTweenSpeed * Time.deltaTime);


        var targetPosition = Vector3.zero;


        if (PivotTransform == null)
        {
            targetPosition = TargetTransform.position;
        }
        else
        {
            targetPosition = Vector3.Lerp(PivotTransform.position, TargetTransform.position, Mathf.Clamp01(positionClampFactor / 100f));
        }

        targetPosition.y += _currentHeight;

        var targetRotation = TargetTransform.rotation *
                             Quaternion.AngleAxis(_currentPanAngle, Vector3.up) *
                             Quaternion.AngleAxis(_currentTiltAngle, Vector3.right);

        _currentTargetPosition = isInstant ? targetPosition : Vector3.Lerp(_currentTargetPosition, targetPosition, positionalTweenSpeed * Time.deltaTime);
        _currentTargetRotation = targetRotation;

        this.transform.localPosition = _currentTargetPosition - _currentTargetRotation * Vector3.forward * _currentDistance;
        this.transform.LookAt(_currentTargetPosition);


        var twoDimensionalForwardDir = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z).normalized;
        var twoDimensionalRightDir = Vector3.Cross(twoDimensionalForwardDir, Vector3.up);

        var offsetVector = twoDimensionalForwardDir * _currentFinalOffset.z + twoDimensionalRightDir * _currentFinalOffset.x + Vector3.up * _currentFinalOffset.y;
        this.transform.position += offsetVector;

        this.transform.position += _shakeVector;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        _tempList.Clear();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, _tempList);

        return _tempList.Count > 0;
    }
}

