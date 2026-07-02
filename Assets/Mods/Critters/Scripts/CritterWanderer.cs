using Timberborn.BaseComponentSystem;
using Timberborn.Geometry;
using Timberborn.Random;
using Timberborn.TerrainSystem;

namespace Critters.Wildlife.Scripts {
	internal class CritterWanderer : BaseComponent,
	                                 IAwakableComponent,
	                                 IUpdateListener {
		private IRandomNumberGenerator _random;
		private ITerrainService _terrainService;
		private CritterWandererSpec _spec;

		private Vector3 _targetPosition;
		private bool _moving;
		private float _idleTimer;

		[Inject]
		public void InjectDependencies(IRandomNumberGenerator random,
		                               ITerrainService terrainService) {
			_random = random;
			_terrainService = terrainService;
		}

		public void Awake() {
			_spec = GetComponent<CritterWandererSpec>();
			_idleTimer = 1f;
			StartWandering();
		}

		public void Update() {
			var currentPos = Transform.position;
			var distance = Mathf.Abs(currentPos.x - _targetPosition.x) + Mathf.Abs(currentPos.z - _targetPosition.z);

			if (distance < 0.3f) {
				_moving = false;
				_idleTimer -= Time.fixedDeltaTime;
				if (_idleTimer <= 0) {
					StartWandering();
				}
				return;
			}

			var direction = new Vector3(
				_targetPosition.x - currentPos.x,
				0f,
				_targetPosition.z - currentPos.z
			).normalized;

			var step = _spec.MoveSpeed * Time.fixedDeltaTime;
			Transform.position += direction * step;

			if (direction.x > 0.1f)
				Transform.localScale = new Vector3(1f, 1f, 1f);
			else if (direction.x < -0.1f)
				Transform.localScale = new Vector3(-1f, 1f, 1f);
		}

		private void StartWandering() {
			var currentPos = Transform.position;
			var gridPos = GridCoordinates.FromPosition(currentPos);
			var radius = _spec.WanderRadius;

			for (int i = 0; i < 100; i++) {
				var dx = _random.Next(-(int)radius, (int)radius + 1);
				var dy = _random.Next(-(int)radius, (int)radius + 1);
				var candidate = new GridCoordinates(gridPos.X + dx, gridPos.Y + dy);

				if (_terrainService.IsWalkable(candidate)) {
					_targetPosition = new Vector3(candidate.X + 0.5f, 0f, candidate.Y + 0.5f);
					_moving = true;
					_idleTimer = 2f + _random.NextFloat() * 3f;
					return;
				}
			}

			_idleTimer = 2f;
		}
	}
}
