using UnityEngine;
using RPG.Movement;

namespace RPG.Control
{
    [RequireComponent(typeof(Mover))]
    public class PlayerController : MonoBehaviour
    {
        private Mover m_Mover;

        // Start is called before the first frame update
        void Start()
        {
            m_Mover = GetComponent<Mover>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    m_Mover.MoveTo(hit.point);
                }
            }
        }
    }
}