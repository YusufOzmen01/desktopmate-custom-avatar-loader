using System;
using System.Collections.Generic;

using UnityEngine;

namespace DMMAExporter.comparers
{
    public class ErrorComparer : IComparer<Error>
    {
        private StringComparer stringComparer;
        public ErrorComparer()
        {
            stringComparer = StringComparer.CurrentCultureIgnoreCase;
        }

        int IComparer<Error>.Compare(Error x, Error y)
        {
            if (x.type != y.type)
                return (int)x.type - (int)y.type;
            else
                return stringComparer.Compare(x.msg, y.msg);
        }
    }
}