using Godot;
using System;
using System.Diagnostics;

public partial class Adversary : EntityBase
{
	private CustomEvents _CustomEvents;

	[Export]
	private string ZombiePath;
	[Export]
	private int PercentZombieSpawn;
	public PathNode NextPathNode;

	public Vector2 PathOffset = new Vector2(0,0);

	private EntityBase target = null;

	private Skillshot weapon;

	protected override void Initialize()
	{
		EntityType = EntityTag.Adversary;
		BaseStats[Stat.Speed] = 350;
		InitializeVision();
		InitializeWeapon();
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
	}
	
	protected override Vector2 GetNormalizedMovementDirection()
	{
		if(IsInstanceValid(target)) {
			Vector2 vectorToTarget = target.GlobalPosition - GlobalPosition;
			return vectorToTarget.Length() < 40 ? Vector2.Zero : vectorToTarget.Normalized();
		}

		if((NextPathNode.GlobalPosition - GlobalPosition + PathOffset).Length() < 5) {
			NextPathNode = NextPathNode.NextNode;
		}

		return (NextPathNode.GlobalPosition - GlobalPosition + PathOffset).Normalized();
	}

	protected override int GetMovementSpeed() {
		if(!IsInstanceValid(target)) {
			return BaseStats[Stat.Speed] / 3;
		}
		return BaseStats[Stat.Speed];
	}

	private void InitializeVision() {
		Area2D visionRange = GetNode<Area2D>("VisionRange");
		visionRange.BodyEntered += OnBodyEnteringVisionRange;
	}

	private void OnBodyEnteringVisionRange(Node2D body) {
		if(body is EntityBase && ShouldSwitchTargets(body as EntityBase)) {
			target = body as EntityBase;
		}
	}
	
	private void InitializeWeapon() {
		weapon = GetNode<Skillshot>("Skillshot");
		weapon.weaponRange.BodyEntered += OnBodyEnteringWeaponRange;
		weapon.attackCooldownTimer.Timeout += EndAttack;
	}

	private void OnBodyEnteringWeaponRange(Node2D body) {
		if(!(body is EntityBase) || body.IsInGroup("Enemy") || weapon.IsWeaponOnCooldown) {
			return;
		}
		
		target = body as EntityBase;
		Attack(target);
	}
	
	private void Attack(EntityBase body) {
		CurrentState = AnimationState.Idle;
		weapon.Attack(body.GlobalPosition);
	}

	private void EndAttack() {
		if(HasValidTarget()) {
			Attack(target);
		}
	}
	
	private bool HasValidTarget() {
		return IsInstanceValid(target) && weapon.weaponRange.OverlapsBody(target);
	}


	private bool ShouldSwitchTargets(EntityBase entity) {
		return (target == null || entity is Player) && !entity.IsInGroup("Enemy");
	}

	protected override void Die()
	{
		//Roll die to see if zombie spawns from this enemy
		Random Rand = new Random();
		int RandNum = Rand.Next(100);
		if(RandNum <= PercentZombieSpawn)
		{
			Vector2 positionToSpawn = GlobalPosition;
			CallDeferred(nameof(SpawnZombie), positionToSpawn);
		}
		
		_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdateAdversaryCount, -1);
		base.Die();
	}

	private void SpawnZombie(Vector2 position) {
		_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdateZombieCount, 1);
		ZombieBase Zombie = ResourceLoader.Load<PackedScene>(ZombiePath).Instantiate() as ZombieBase;
		GetParent().AddChild(Zombie);
		Zombie.GlobalPosition = position;
	}

	private void OnLevelFinish() {
		QueueFree();
	}
}
