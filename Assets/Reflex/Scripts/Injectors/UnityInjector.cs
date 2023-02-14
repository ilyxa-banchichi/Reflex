using UnityEngine;
using Reflex.Scripts.Core;
using Reflex.Scripts.Events;
using Reflex.Scripts.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly] // https://docs.unity3d.com/ScriptReference/Scripting.AlwaysLinkAssemblyAttribute.html

namespace Reflex.Injectors
{
	internal static class UnityInjector
	{
		private static Container _projectContainer;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeAwakeOfFirstSceneOnly()
		{
			CreateProjectContainer();

			StaticEventManager.OnSceneEarlyAwake += OnSceneEarlyAwake;
			StaticEventManager.OnContainerRebuild += OnContainerRebuild;
		}

		private static void CreateProjectContainer()
		{
			_projectContainer = ContainerTree.Root = new Container("ProjectContainer");
			StaticEventManager.Quitting += DisposeProjectContainer;

			if (ResourcesUtilities.TryLoad<ProjectContext>("ProjectContext", out var projectContext))
			{
				projectContext.InstallBindings(_projectContainer);
			}
		}
		
		private static Container CreateSceneContainer(Scene scene, Container projectContainer)
		{
			var container = projectContainer.Scope(scene.name);
			
			var subscription = scene.OnUnload(() =>
			{
				container.Dispose();
			}); 
			
			// If app is being closed, all containers will be disposed by depth first search starting from project container root, see UnityInjector.cs
			StaticEventManager.Quitting += () =>
			{
				subscription.Dispose();
			};
			
			if (scene.TryFindAtRootObjects<SceneContext>(out var sceneContext))
			{
				sceneContext.InstallBindings(container);
			}

			return container;
		}
		
		private static void OnContainerRebuild()
		{
			DisposeProjectContainer();
			CreateProjectContainer();
		}

		private static void DisposeProjectContainer()
		{
			_projectContainer.Dispose();
			ContainerTree.Root = null;
		}

		private static void OnSceneEarlyAwake(Scene scene)
		{
			var sceneContainer = CreateSceneContainer(scene, _projectContainer);
			SceneInjector.Inject(scene, sceneContainer);
		}
	}
}
