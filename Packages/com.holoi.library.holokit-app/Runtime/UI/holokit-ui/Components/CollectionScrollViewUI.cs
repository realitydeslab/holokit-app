using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class CollectionScrollViewUI : MonoBehaviour
    {
         public int count;
         public int currentIndex;
        public List<GameObject> CollectionContainerList;
        [SerializeField] Scrollbar _scrollBar;

        float value;

        private void Update()
        {
            value = Mathf.Clamp01(_scrollBar.value);
            currentIndex = Mathf.RoundToInt(value * (count - 1));

            for (int i = 0; i < CollectionContainerList.Count; i++)
            {
                if(currentIndex == i)
                {
                    CollectionContainerList[i].GetComponent<CollectionContainer>().activeState = CollectionContainer.ActiveState.Active;
                    CollectionContainerList[i].GetComponent<CollectionContainer>().UpdateUITheme();
                }
                else
                {
                    CollectionContainerList[i].GetComponent<CollectionContainer>().activeState = CollectionContainer.ActiveState.InActive;
                    CollectionContainerList[i].GetComponent<CollectionContainer>().UpdateUITheme();

                }
            }
        }
    }

}
