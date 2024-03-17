using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txt;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
    }

    public void UpdateValue()
    {
        _txt.text = _slider.value.ToString();
    }
}
