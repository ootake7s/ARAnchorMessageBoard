// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors.Unity.Examples
{
    public class XRUXPickerForMenu : XRUXPicker
    {
        private static XRUXPickerForMenu _Instance;
        public new static XRUXPickerForMenu Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<XRUXPickerForMenu>();
                }

                return _Instance;
            }
        }
        
    }
}
