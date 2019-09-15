using UnityEngine;

namespace TableFootball
{
    public class CamRotate : MonoBehaviour
    {
        [SerializeField]
        float speed = 10f;

        void LateUpdate()
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * speed);
        }
    }
}