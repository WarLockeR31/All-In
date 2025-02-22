using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProjectile : MonoBehaviour
{
    [SerializeField] private AudioSource _cardHitAudio;


    [SerializeField] private float autoAimSeconds;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rotationSpeed;
    private bool isAutoAiming = true;
    private Vector3 direction;
    private Player player;
    private float damage; public void SetDamage(float newValue) { damage = newValue; }


    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        StartCoroutine(DisableAutoAim());
    }

    IEnumerator DisableAutoAim()
    {
        yield return new WaitForSeconds(autoAimSeconds);
        isAutoAiming = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.uiPanel.activeSelf)
        {
            Destroy(gameObject);
        }

        if (isAutoAiming)
        {
            Vector3 playerPos = player.transform.position;
            direction = (playerPos - transform.position).normalized;
        }

        transform.position += direction * projectileSpeed * Time.deltaTime;

        transform.eulerAngles += new Vector3(0f, rotationSpeed * Time.deltaTime, 0f);
        //transform.Rotate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!player.IsInvincible)
            {
                _cardHitAudio.Play();
            }

            player.TakeDamage(damage);
            
            GetComponent<Collider>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            isAutoAiming = false;
            direction = Vector3.zero;

            Destroy(gameObject, 1);
            return;
        }

        if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile") || other.isTrigger)
        {
            return;
        }

        Destroy(gameObject);
    }
}
