using UnityEngine;
using UnityEngine.UI;

public class ShowHp : MonoBehaviour
{
    [SerializeField] private Sprite _hp3Sprite;
    [SerializeField] private Sprite _hp2Sprite;
    [SerializeField] private Sprite _hp1Sprite;
    [SerializeField] private Sprite _hp0Sprite; // sprite for no lives left

    private SpriteRenderer _spriteRenderer;
    private Image _uiImage;

    [SerializeField] private Player _player;

    private int _lastLives = -1; 

    void Start()
    {
        _uiImage = GetComponent<Image>();

        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_player == null)
        {
            Debug.LogWarning("ShowHp: Player reference not assigned in Inspector. Disabling ShowHp.");
            enabled = false;
            return;
        }
        _lastLives = _player.Lives;
        UpdateHpDisplay();
    }

    void Update()
    {
        if (_player == null) return;

        // Only update if HP changed
        if (_player.Lives != _lastLives)
        {
            UpdateHpDisplay();
            _lastLives = _player.Lives;
        }
    }

    private void UpdateHpDisplay()
    {
        // If using UI Image
        if (_uiImage != null)
        {
            switch (_player.Lives)
            {
                case 3:
                    _uiImage.enabled = true;
                    _uiImage.sprite = _hp3Sprite;
                    break;
                case 2:
                    _uiImage.enabled = true;
                    _uiImage.sprite = _hp2Sprite;
                    break;
                case 1:
                    _uiImage.enabled = true;
                    _uiImage.sprite = _hp1Sprite;
                    break;
                case 0:
                    _uiImage.enabled = true;
                    _uiImage.sprite = _hp0Sprite;
                    break;
                default:
                    _uiImage.enabled = false;
                    break;
            }
            return;
        }

        // Fallback to SpriteRenderer
        if (_spriteRenderer == null) return;

        switch (_player.Lives)
        {
            case 3:
                _spriteRenderer.enabled = true;
                _spriteRenderer.sprite = _hp3Sprite;
                break;
            case 2:
                _spriteRenderer.enabled = true;
                _spriteRenderer.sprite = _hp2Sprite;
                break;
            case 1:
                _spriteRenderer.enabled = true;
                _spriteRenderer.sprite = _hp1Sprite;
                break;
            case 0:
                _spriteRenderer.enabled = true;
                _spriteRenderer.sprite = _hp0Sprite;
                break;
            default:
                _spriteRenderer.enabled = false;
                break;
        }
    }
}