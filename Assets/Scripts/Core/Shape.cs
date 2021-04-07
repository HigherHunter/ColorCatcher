using System.Collections;
using UnityEngine;

namespace Core
{
    public class Shape : MonoBehaviour
    {
        public Color[] shapePiecesColor;

        [SerializeField] private Vector3 rotationAngle;

        private bool _isMoving;

        private void Start()
        {
            shapePiecesColor = new Color[transform.childCount];

            for (int i = 0; i < shapePiecesColor.Length; i++)
            {
                shapePiecesColor[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().color;
            }
        }

        public void RotateShapeLeft(float duration)
        {
            if (!_isMoving)
                StartCoroutine(RotationLerpRoutine(rotationAngle, duration));
        }

        public void RotateShapeRight(float duration)
        {
            if (!_isMoving)
                StartCoroutine(RotationLerpRoutine(-rotationAngle, duration));
        }

        private IEnumerator RotationLerpRoutine(Vector3 angle, float duration)
        {
            _isMoving = true;
            float time = 0;
            Quaternion startValue = transform.rotation;
            Quaternion endValue = Quaternion.Euler(transform.eulerAngles + angle);

            while (time < duration)
            {
                transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            transform.rotation = endValue;
            _isMoving = false;
        }

        public void MoveShapeTo(Vector3 position)
        {
            transform.position = position;
        }
    }
}