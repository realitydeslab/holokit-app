using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class TheScannerCenter : MonoBehaviour
    {
        public static TheScannerCenter Instance { get { return _instance; } }

        private static TheScannerCenter _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
        }
    }
}
