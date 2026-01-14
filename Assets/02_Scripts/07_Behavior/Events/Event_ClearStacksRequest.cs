using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Event_ClearStacksRequest")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Event_ClearStacksRequest", message: "Clear PathStack and VisitedList", category: "Events", id: "62199426152d7bb7d19f0881c180f9e9")]
public sealed partial class Event_ClearStacksRequest : EventChannel { }

