using Bindito.Core;
using Timberborn.CoreUI;
using Timberborn.UILayoutSystem;
using UnityEngine;

namespace Mods.HelloWorld.Scripts {
  [Context("Game")]
  public class HelloWorldConfigurator : IConfigurator {

    public void Configure(IContainerDefinition containerDefinition) {
      Debug.Log("[HelloWorld] HelloWorldConfigurator.Configure() called");
      
      // Bind HelloWorldInitializer as a singleton
      containerDefinition.Bind<HelloWorldInitializer>().AsSingleton();
    }

  }
}