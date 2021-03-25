// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors.Unity.Examples
{
    public class XRUXPickerForInit : XRUXPicker
    {
        private static XRUXPickerForInit _Instance;
        public new static XRUXPickerForInit Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<XRUXPickerForInit>();
                }

                return _Instance;
            }
        }
        
    }
}
