using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon m_Weapon;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<Fighter>().EquipWeapon(m_Weapon);
                Destroy(gameObject);
            }
        }
    }
}