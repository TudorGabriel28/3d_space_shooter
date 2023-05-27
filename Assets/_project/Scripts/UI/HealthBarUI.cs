using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image _healthBarImage;
    [SerializeField] float _updateRate = 1f;
    [SerializeField] DamageHandler _damageHandler;

    float _targetFillAmount;


    void OnEnable()
    {
        if (_damageHandler)
        {
            _damageHandler.HealthChanged.AddListener(UpdateHealthBar);
            _damageHandler.ObjectDestroyed.AddListener(DisableHealthBar);
            _targetFillAmount = 1;
        }

    }

    void OnDisable()
    {
        if (_damageHandler)
        {
            _damageHandler.HealthChanged.RemoveListener(UpdateHealthBar);
            _damageHandler.ObjectDestroyed.RemoveListener(DisableHealthBar);
        }
    }

    void LateUpdate()
    {

        if (!_damageHandler || _healthBarImage == null) return;
        if (Mathf.Approximately(_healthBarImage.fillAmount, _targetFillAmount)) return;
        _healthBarImage.fillAmount =
            Mathf.MoveTowards(_healthBarImage.fillAmount, _targetFillAmount, _updateRate * Time.deltaTime);
    }

    void UpdateHealthBar()
    {
        if (_damageHandler == null) return;
        if (_damageHandler.Health <= 0)
        {
            _targetFillAmount = 0;
            GameManager.Instance.PlayerLost();
            return;
        }

        _targetFillAmount = (float)_damageHandler.Health / (float)_damageHandler.MaxHealth;
    }
    void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

}
