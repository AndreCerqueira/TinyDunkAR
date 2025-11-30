using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Slider
{
    [RequireComponent(typeof(UnityEngine.UI.Slider))]
    public class PowerSliderController : Singleton<PowerSliderController>
    {
        [Header("Gradient Colors")]
        [SerializeField] private Color _colorLow = new Color(0.5f, 0.7f, 1f);
        [SerializeField] private Color _colorMidLow = Color.green;
        [SerializeField] private Color _colorMid = Color.yellow;
        [SerializeField] private Color _colorMidHigh = new Color(1f, 0.6f, 0f);
        [SerializeField] private Color _colorHigh = Color.red;

        private UnityEngine.UI.Slider _slider;
        private Image _fillImage;
        private CanvasGroup _canvasGroup;

        protected override void Awake()
        {
            base.Awake();
            
            _slider = GetComponent<UnityEngine.UI.Slider>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null) 
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (_slider.fillRect != null)
            {
                _fillImage = _slider.fillRect.GetComponent<Image>();
            }
            
            SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1f : 0f;
        }

        public void UpdateProgress(float normalizedValue)
        {
            _slider.value = normalizedValue;
            UpdateFillColor(normalizedValue);
        }

        private void UpdateFillColor(float t)
        {
            if (_fillImage == null) return;
            _fillImage.color = CalculateColor(t);
        }

        private Color CalculateColor(float t)
        {
            if (t < 0.25f)
                return Color.Lerp(_colorLow, _colorMidLow, t / 0.25f);
            if (t < 0.5f)
                return Color.Lerp(_colorMidLow, _colorMid, (t - 0.25f) / 0.25f);
            if (t < 0.75f)
                return Color.Lerp(_colorMid, _colorMidHigh, (t - 0.5f) / 0.25f);
            
            return Color.Lerp(_colorMidHigh, _colorHigh, (t - 0.75f) / 0.25f);
        }
    }
}