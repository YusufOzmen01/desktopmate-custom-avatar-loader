using System;
using System.Collections.Generic;

using UnityEngine;

namespace DMMAExporter.comparers
{
    public class ComponentNameComparer : IComparer<Component>
    {
        private StringComparer stringComparer;
        public ComponentNameComparer()
        {
            stringComparer = StringComparer.CurrentCultureIgnoreCase;
        }

        int IComparer<Component>.Compare(Component x, Component y)
        {
            return stringComparer.Compare(x.gameObject.name, y.gameObject.name);
        }
    }
}