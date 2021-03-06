﻿/*
 * Copyright 2015 Huysentruit Wouter
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WinBeacon.Stack;
using WinBeacon.Stack.Hci.Events;

namespace WinBeacon.Tests
{
    [TestFixture]
    public class BeaconTests
    {
        private Beacon beacon;
        private byte[] data;

        [TestFixtureSetUp]
        public void Setup()
        {
            beacon = new Beacon("4fe5d5f6-abce-ddfe-1587-123d1a4b567f", 1234, 5678, -48, 0xAABB) { Rssi = -52 };
            data = new byte[] {
                0x02, 0x01, 0x1A, 0x1A, 0xFF, 0xAA, 0xBB, 0x02, 0x15, 0x4F, 0xE5, 0xD5, 0xF6, 0xAB, 0xCE,
                0xDD, 0xFE, 0x15, 0x87, 0x12, 0x3D, 0x1A, 0x4B, 0x56, 0x7F, 0x04, 0xD2, 0x16, 0x2E, 0xD0
            };
        }

        [Test]
        public void Beacon_Constructor()
        {
            Assert.AreEqual("4fe5d5f6-abce-ddfe-1587-123d1a4b567f", beacon.Uuid);
            Assert.AreEqual(1234, beacon.Major);
            Assert.AreEqual(5678, beacon.Minor);
            Assert.AreEqual(-48, beacon.CalibratedTxPower);
            Assert.AreEqual(0xAABB, beacon.CompanyId);
        }

        [Test]
        public void Beacon_ToAdvertisingData()
        {
            Assert.AreEqual(data, beacon.ToAdvertisingData());
        }

        [Test]
        public void Beacon_Parse()
        {
            var address = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC };
            var queue = new Queue<byte>();
            queue.Enqueue(0x00);
            queue.Enqueue(0x00);
            queue.Enqueue(address.Reverse());
            queue.Enqueue((byte)data.Length);
            queue.Enqueue(data);
            unchecked { queue.Enqueue((byte)-52); }
            var beacon = Beacon.Parse(LeAdvertisingEvent.Parse(queue));
            Assert.AreEqual(address, beacon.Address);
            Assert.AreEqual("4fe5d5f6-abce-ddfe-1587-123d1a4b567f", beacon.Uuid);
            Assert.AreEqual(1234, beacon.Major);
            Assert.AreEqual(5678, beacon.Minor);
            Assert.AreEqual(-48, beacon.CalibratedTxPower);
            Assert.AreEqual(0xAABB, beacon.CompanyId);
            Assert.AreEqual(-52, beacon.Rssi);
            Assert.IsFalse(beacon.IsAppleIBeacon);
        }
    }
}
