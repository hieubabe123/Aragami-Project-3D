using UnityEngine;

public class StealthKillTrigger : MonoBehaviour
{
    private PlayerStealthKill _playerStealthKill;

    void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnPlayerKilledEnemy>(OnPlayerKilledEnemy);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnPlayerKilledEnemy>(OnPlayerKilledEnemy);
    }

    private void OnPlayerKilledEnemy(EventDefine.OnPlayerKilledEnemy evt)
    {
        if (evt.BotTrigger == this)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerStealthKill = other.GetComponent<PlayerStealthKill>();
            if (_playerStealthKill != null)
            {
                _playerStealthKill.SetKillTarget(transform.parent.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _playerStealthKill != null)
        {
            _playerStealthKill.ClearKillTarget(transform.parent.gameObject);
            _playerStealthKill = null;
        }
    }
}
