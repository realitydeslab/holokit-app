using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class DeathCircleEnemy : MonoBehaviour
    {
        private float _accumulatedTime;

        private const float Lifetime = 3f;

        private void Update()
        {
            _accumulatedTime += Time.deltaTime;

            if (_accumulatedTime > Lifetime)
                Destroy(gameObject);
        }
    }
}
