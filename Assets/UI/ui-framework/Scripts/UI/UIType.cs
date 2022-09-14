using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class UIType
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public UIType(string path)
        {
            Path = path;
            Name = path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}
