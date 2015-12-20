﻿using Common;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class SharedChestComponentData : ComponentData {
        public float duration { get; private set; }
        public float updateDropListInterval { get; private set; }
        public DropList dropList { get; private set; }
#if UP
        public SharedChestComponentData(UPXElement e) {
            duration = e.GetFloat("duration");
            updateDropListInterval = e.GetFloat("update_drop_list_interval");
            dropList = (DropList)Enum.Parse(typeof(DropList), e.GetString("drop_list"));
        }
#else
        public SharedChestComponentData(XElement e) {
            duration = e.GetFloat("duration");
            updateDropListInterval = e.GetFloat("update_drop_list_interval");
            dropList = (DropList)Enum.Parse(typeof(DropList), e.GetString("drop_list"));
        }
#endif
        public SharedChestComponentData(float duration, float updateDropListInterval, DropList dropList) {
            this.duration = duration;
            this.updateDropListInterval = updateDropListInterval;
            this.dropList = dropList;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.SharedChest;
            }
        }

    }
}