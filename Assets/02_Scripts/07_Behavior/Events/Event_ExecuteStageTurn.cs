using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Event_ExecuteStageTurn")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Event_ExecuteStageTurn", message: "Start Stage Trun", category: "Events", id: "7e081929a6ef10fb2a760eed9df48093")]
public sealed partial class Event_ExecuteStageTurn : EventChannel { }

