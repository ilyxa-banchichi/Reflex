using UnityEngine;
using Reflex.Scripts.Events;

namespace Reflex.Scripts.Core
{
    [DefaultExecutionOrder(-10000)]
    public class ContainerRebuildSceneContext : AContext
    {
        private static bool _notFirstAwake = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            _notFirstAwake = false;
        }

        private void Awake()
        {
            if (_notFirstAwake)
                UnityStaticEvents.OnContainerRebuild.Invoke();
            else
                _notFirstAwake = true;

            UnityStaticEvents.OnSceneEarlyAwake.Invoke(gameObject.scene);
        }

        public override void InstallBindings(Container container)
        {
            base.InstallBindings(container);
            Debug.Log($"{GetType().Name} Bindings Installed");
        }
    }
}