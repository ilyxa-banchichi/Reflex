using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Scripts.Events
{
    internal static class StaticEventManager
    {
        public static event Action Quitting;
        public static event Action<Scene> OnSceneEarlyAwake;
        public static event Action OnContainerRebuild;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RunOnStart()
        {
            UnsubscribeOnApplicationEvents();
            ResetSubscriptions();
            SubscribeOnApplicationEvents();
        }
        
        private static void ResetSubscriptions()
        {
            Quitting = null;
            OnSceneEarlyAwake = null;
        }

        private static void SubscribeOnApplicationEvents()
        {
            Application.quitting += RaiseQuitting;
            UnityStaticEvents.OnSceneEarlyAwake += RaiseOnSceneEarlyAwake;
            UnityStaticEvents.OnContainerRebuild += RaiseOnContainerRebuild;
        }
        
        private static void UnsubscribeOnApplicationEvents()
        {
            Application.quitting -= RaiseQuitting;
            UnityStaticEvents.OnSceneEarlyAwake -= RaiseOnSceneEarlyAwake;
            UnityStaticEvents.OnContainerRebuild -= RaiseOnContainerRebuild;
        }
        
        private static void RaiseQuitting() => Quitting?.Invoke();
        private static void RaiseOnSceneEarlyAwake(Scene scene) => OnSceneEarlyAwake?.Invoke(scene);
        private static void RaiseOnContainerRebuild() => OnContainerRebuild?.Invoke();
    }
}