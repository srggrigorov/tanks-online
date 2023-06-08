using Unity.Netcode;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace GameAssets._Scripts.GameScene
{
    [RequireComponent(typeof(Canvas))]
    public class UIElementsPlacer : MonoBehaviour
    {
        [SerializeField] private RectTransform _joystickTransform;
        [SerializeField] private RectTransform _shootButtonTransform;
        [SerializeField] private Camera _camera;
        [SerializeField] private RectTransform _canvasRectTransform;

        private ETouch.Finger _joystickFinger;
        private ETouch.Finger _shootButtonFinger;

        private void Awake()
        {
            _camera ??= Camera.main;
            _canvasRectTransform ??= GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ETouch.EnhancedTouchSupport.Enable();
            ETouch.Touch.onFingerDown += HandleFingerDown;
            ETouch.Touch.onFingerUp += HandleFingerUp;
        }

        private void HandleFingerDown(ETouch.Finger finger)
        {
            if (finger.screenPosition.x <= Screen.width / 2)
            {
                if (_joystickFinger != null)
                {
                    return;
                }

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform,
                    finger.screenPosition, _camera, out var localPoint);
                _joystickFinger = finger;
                _joystickTransform.anchoredPosition = localPoint;
                _joystickTransform.gameObject.SetActive(true);
            }
            else
            {
                if (_shootButtonFinger != null)
                {
                    return;
                }

                _shootButtonFinger = finger;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform,
                    finger.screenPosition, _camera, out var localPoint);
                _shootButtonTransform.anchoredPosition = localPoint;
                _shootButtonTransform.gameObject.SetActive(true);
            }
        }

        private void HandleFingerUp(ETouch.Finger finger)
        {
            if (finger == _joystickFinger)
            {
                _joystickTransform.anchoredPosition =
                    Vector2.left * 10000; //Can't disable because of InputSystem errors
                _joystickFinger = null;
            }
            else if (finger == _shootButtonFinger)
            {
                _shootButtonTransform.anchoredPosition =
                    Vector2.right * 10000; //Can't disable because of InputSystem errors
                _shootButtonFinger = null;
            }
        }

        private void OnDisable()
        {
            ETouch.Touch.onFingerDown -= HandleFingerDown;
            ETouch.Touch.onFingerUp -= HandleFingerUp;
            ETouch.EnhancedTouchSupport.Disable();
        }
    }
}