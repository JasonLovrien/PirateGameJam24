using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class ZombieBase : EntityBase
{
	[Export]
	private float Threshhold = 70;
	private Vector2 RallyPoint = Vector2.Zero;
	private bool IsForceMove = false;
	
	private Adversary target;
	private Skillshot weapon;

	public void SetThreshhold(float amount)
	{
		Threshhold = amount;
	}
	
	protected override void Initialize()
	{
		EntityType = EntityTag.Zombie;
		InitializeWeapon();
		deceleration = 15;
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("secondary_button"))
		{
			RallyPoint = GetGlobalMousePosition();
			IsForceMove = Input.IsActionPressed("force_move");
		}
	}

	protected override Vector2 GetNormalizedMovementDirection()
	{
		Vector2 vectorToRallyPoint = RallyPoint-GlobalPosition;

		if(ShouldNotMove(vectorToRallyPoint.Length())){
			return Vector2.Zero;
		}

		return vectorToRallyPoint.Normalized();
	}

	private bool ShouldNotMove(float distanceToRallyPoint ) {
		return 
			weapon.IsWeaponOnCooldown
		|| (HasValidTarget() && !IsForceMove) 
		|| distanceToRallyPoint <= Threshhold;
	}
	
	private void InitializeWeapon() {
		weapon = GetNode<Skillshot>("Skillshot");
		weapon.weaponRange.BodyEntered += OnBodyEnteringWeaponRange;
		weapon.attackCooldownTimer.Timeout += EndAttack;
	}

	private void OnBodyEnteringWeaponRange(Node2D body) {
		if(!(body is EntityBase) || !body.IsInGroup("Enemy") || weapon.IsWeaponOnCooldown) {
			return;
		}
		
		target = body as Adversary;
		Attack((Adversary)body);
	}

	
	private void Attack(Adversary body) {
		CurrentState = AnimationState.Idle;
		weapon.Attack(body.GlobalPosition);
	}

	private void EndAttack() {
		if(HasValidTarget() && !IsForceMove) {
			Attack(target);
		}
	}
	
	private bool HasValidTarget() {
		return IsInstanceValid(target) && weapon.weaponRange.OverlapsBody(target);
	}
}
