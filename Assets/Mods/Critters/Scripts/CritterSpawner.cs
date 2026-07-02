using Timberborn.BaseComponentSystem;
using Timberborn.Geometry;
using Timberborn.Navigation;
using Timberborn.Random;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;

namespace Critters.Wildlife.Scripts {
	public class CritterSpawner : ILoadableSingleton, IUpdateListener {
		private readonly IEntityLoader _entityLoader;
		private readonly IRandomNumberGenerator _random;
		private readonly ITerrainService _terrainService;

		private float _nextSpawnTimer;
		private const int MaxCritters = 8;
		private int _activeCount = 0;

		public CritterSpawner(IEntityLoader entityLoader,
		                      IRandomNumberGenerator random,
		                      ITerrainService terrainService) {
			_entityLoader = entityLoader;
			_random = random;
			_terrainService = terrainService;
			_nextSpawnTimer = 5f;
		}

		public void Load() {
			for (int i = 0; i < 5; i++) {
				SpawnCritter();
			}
		}

		public void Update() {
			_nextSpawnTimer -= Time.fixedDeltaTime;
			if (_nextSpawnTimer <= 0 && _activeCount < MaxCritters) {
				SpawnCritter();
				_nextSpawnTimer = 20f + _random.NextFloat() * 15f;
			}
		}

		private void SpawnCritter() {
			var position = FindRandomLandPosition();
			if (position.HasValue) {
				_entityLoader.LoadEntity("Critter", position.Value);
				_activeCount++;
			}
		}

		private Vector3? FindRandomLandPosition() {
			var mapSize = _terrainService.GetTerrainSize();
			for (int i = 0; i < 200; i++) {
				var x = _random.NextFloat() * mapSize.x;
				var y = _random.NextFloat() * mapSize.y;
				var gridPos = new GridCoordinates((int)x, (int)y);
				if (_terrainService.IsWalkable(gridPos)) {
					return new Vector3(gridPos.X + 0.5f, 0, gridPos.Y + 0.5f);
				}
			}
			return null;
		}
	}
}
