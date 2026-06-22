using UnityEngine;
using UnityEngine.UI;

public class BossHPSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _hpSlider;

    private CanvasGroup _canvasGroup;

    // Flag set when ActivateHPBar is called before Start runs so Start won't overwrite it
    private bool _externallyActivated = false;

    void Start()
    {
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

        // Deactivate the HP bar at start unless it was already activated externally
        if (!_externallyActivated)
        {
            DeactivateHPBar();
        }
    }

    public void ActivateHPBar(float currentHP, float maxHP)
    {
        // Mark that an external caller wants this visible so Start won't hide it
        _externallyActivated = true;

        gameObject.SetActive(true);
        UpdateHPBar(currentHP, maxHP);
        ShowHPBar();
    }

    public void DeactivateHPBar()
    {
        // Clear external activation flag when deactivating
        _externallyActivated = false;
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
            _hpSlider.value = Mathf.Clamp(currentHP, 0f, maxHP);
        }
    }
}
