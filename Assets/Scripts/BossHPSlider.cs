using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class BossHPSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _hpSlider;

    private CanvasGroup _canvasGroup;

    void Start()
    {
        // If slider not assigned, try to find it on this GameObject or children
        if (_hpSlider == null)
        {
            _hpSlider = GetComponent<Slider>();
        }

        if (_hpSlider == null)
        {
            _hpSlider = GetComponentInChildren<Slider>();
        }

        _canvasGroup = GetComponent<CanvasGroup>();

        if (_hpSlider == null)
        {
            Debug.LogError("BossHPSlider: HP Slider is not assigned and could not be found on this GameObject or its children!");
        }

        if (_canvasGroup == null)
        {
            Debug.LogError("BossHPSlider: Canvas Group component not found!");
        }

        // Deactivate the HP bar at start
        DeactivateHPBar();
    }

    public void ActivateHPBar(float currentHP, float maxHP)
    {
        gameObject.SetActive(true);
        UpdateHPBar(currentHP, maxHP);
        ShowHPBar();
    }

    public void DeactivateHPBar()
    {
        HideHPBar();
        gameObject.SetActive(false);
    }

    public void ShowHPBar()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }
    }

    public void HideHPBar()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        if (_hpSlider != null)
        {
            _hpSlider.maxValue = maxHP;
            _hpSlider.value = currentHP;
        }
    }
}
