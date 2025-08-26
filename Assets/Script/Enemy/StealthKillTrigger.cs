using UnityEngine;

public class StealthKillTrigger : MonoBehaviour
{
    private PlayerStealthKill _playerStealthKill;

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
