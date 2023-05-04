using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon m_Weapon;
        [SerializeField] private float m_RespawnTime = 30f;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<Fighter>().EquipWeapon(m_Weapon);
                StartCoroutine(HideForSeconds(m_RespawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool b)
        {
            GetComponent<Collider>().enabled = b;

            foreach (Transform child in transform) 
            {
                child.gameObject.SetActive(b);
            }
        }

    }
}