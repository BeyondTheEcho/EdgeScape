using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig m_Weapon;
        [SerializeField] private float m_RespawnTime = 30f;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                Pickup(collider.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(m_Weapon);
            StartCoroutine(HideForSeconds(m_RespawnTime));
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

        public bool HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerController.MoveToDestination(transform.position);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}