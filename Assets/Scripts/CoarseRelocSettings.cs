// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.Azure.SpatialAnchors.Unity.Examples
{
    public class CoarseRelocSettings
    {
        /// <summary>
        /// Whitelist of Bluetooth-LE beacons used to find anchors and improve the locatability
        /// of existing anchors.
        /// Add the UUIDs for your own Bluetooth beacons here to use them with Azure Spatial Anchors.
        /// </summary>
        public static readonly string[] KnownBluetoothProximityUuids =
        {
            "11111111-2222-3333-4444-555566667777",
            "12345678-2345-3456-4567-888888888888",
            "AAAABBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF",
        };
    }
}