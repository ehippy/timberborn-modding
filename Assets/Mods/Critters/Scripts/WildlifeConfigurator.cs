using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Critters.Wildlife.Scripts {
	[Context("Game")]
	public class WildlifeConfigurator : Configurator {
		protected override void Configure() {
			Bind<CritterSpawner>().AsSingleton();

			MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		}

		private static TemplateModule ProvideTemplateModule() {
			var builder = new TemplateModule.Builder();
			builder.AddDecorator<CritterWandererSpec, CritterWanderer>();
			return builder.Build();
		}
	}
}
