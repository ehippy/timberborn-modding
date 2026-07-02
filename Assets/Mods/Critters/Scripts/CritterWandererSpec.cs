using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;

namespace Critters.Wildlife.Scripts {
	public record CritterWandererSpec : ComponentSpec {
		[Serialize]
		public float MoveSpeed { get; init; } = 2.0f;

		[Serialize]
		public float WanderRadius { get; init; } = 8f;

		[Serialize]
		public float WanderInterval { get; init; } = 10f;
	}
}
