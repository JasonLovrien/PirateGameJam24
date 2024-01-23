using Godot;
using System;

public partial class Adversary : EntityBase
{
	public PathNode NextPathNode;

	private EntityBase target = null;

	private Skillshot weapon;

	protected override void Initialize()
	{
		EntityType = EntityTag.Adversary;
		BaseStats[Stat.Speed] = 350;
		InitializeVision();
		InitializeWeapon();
	}
	
	protected override Vector2 GetNormalizedMovementDirection()
	{
		if(IsInstanceValid(target)) {
			Vector2 vectorToTarget = target.GlobalPosition - GlobalPosition;
			return vectorToTarget.Length() < 40 ? Vector2.Zero : vectorToTarget.Normalized();
		}

		if((NextPathNode.GlobalPosition - GlobalPosition).Length() < 5) {
			NextPathNode = NextPathNode.NextNode;
		}

		return (NextPathNode.GlobalPosition - GlobalPosition).Normalized();
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

	public override void ApplyEffect(Effect effect)
	{
		throw new NotImplementedException();
	}
}
