using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleSelectionIndicator : MonoBehaviour
    {
        [SerializeField] private Image _dotPrefab;

        [SerializeField] private Sprite _unselectedCircle;

        [SerializeField] private Sprite _selectedCircle;

        private readonly List<Image> _dots = new();

        public void Init(int index, int length)
        {
            foreach (var dot in _dots)
            {
                Destroy(dot.gameObject);
            }
            _dots.Clear();

            for (int i = 0; i < length; i++)
            {
                var dot = Instantiate(_dotPrefab);
                dot.transform.SetParent(transform);
                dot.transform.localScale = Vector3.one;
                if (i == index)
                {
                    dot.sprite = _selectedCircle;
                }
                _dots.Add(dot);
            }
        }

        public void UpdateIndex(int index)
        {
            for (int i = 0; i < _dots.Count; i++)
            {
                if (i == index)
                {
                    _dots[i].sprite = _selectedCircle;
                }
                else
                {
                    _dots[i].sprite = _unselectedCircle;
                }
            }
        }
    }
}
