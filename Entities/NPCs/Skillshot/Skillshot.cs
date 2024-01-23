using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Skillshot : Node2D
{
	public enum AttackTypes {
		Stab,
		Slash
	}

	[Export]
	public AttackTypes AttackType;

	[Export]
	public int weaponDamage = 10;

	[Export]
	public float weaponCooldown = 2.0f;

	[Export]
	public float weaponSpeed = 40.0f;

	[Export]
	public Godot.Collections.Array<EntityTag> TypesToAffect;

	private Action AttackMethod;
	public Timer attackAnimationTimer;
	public Timer attackCooldownTimer;
	public bool IsWeaponOnCooldown = false;
	public Area2D weaponRange;
	private Area2D weaponHitbox;
	private Vector2 Target;
	private bool IsAttacking = false;

	private Path2D Path;
	private PathFollow2D PathProgress;

	private List<Node2D> HitNodes;

	Sprite2D weaponSprite;
	private float WeaponRangeLength;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeContainerNodes();
		MoveExternalChildNodesToContainers();
		InitializeSprite();
		
		
		Path = GetNode<Path2D>("Path2D");
		PathProgress = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		
		attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer");
		attackCooldownTimer.WaitTime = weaponCooldown;
		attackCooldownTimer.Timeout += AttackCoolDownOver;

		WeaponRangeLength = GetRangeLength();

		SetAttackType();

		weaponHitbox.BodyEntered += OnAttackHittingSomething;
	}
	
	private void InitializeContainerNodes() {
		weaponHitbox = GetNode<Area2D>("Path2D/PathFollow2D/HitboxContainer");
		weaponRange = GetNode<Area2D>("RangeContainer");
	}
	
	private void MoveExternalChildNodesToContainers() {
		GetNode("Hitbox").Reparent(weaponHitbox);
		GetNode("Sprite").Reparent(weaponHitbox);
		GetNode("Range").Reparent(weaponRange);
	}
	
	private void InitializeSprite() {
		weaponSprite = GetNode<Sprite2D>("Path2D/PathFollow2D/HitboxContainer/Sprite");
		weaponSprite.Visible = false;
	}

	private float GetRangeLength() {
		float lengthToOuterBounds = (GetNode<CollisionShape2D>("RangeContainer/Range").Shape as CircleShape2D).Radius;
		float hitboxLength = GetNode<CollisionShape2D>("Path2D/PathFollow2D/HitboxContainer/Hitbox").Shape.GetRect().Size.X;

		return lengthToOuterBounds - hitboxLength;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!IsAttacking) {
			return;
		}

		PathProgress.Progress += (float)delta * weaponSpeed;

		if(PathProgress.ProgressRatio >= .99f) {
			AttackFinish();
		}
	}

	private void SetAttackType() {
		if(AttackType == AttackTypes.Stab) {
			AttackMethod = Stab;
		}
		else {
			AttackMethod = Slash;
		}
	}

	private void Stab() {
		Vector2 directionToTarget = (Target - GlobalPosition).Normalized();
		weaponHitbox.Rotation = directionToTarget.Angle();
		PathProgress.Rotates = false;
		Path.Curve.ClearPoints();
		Path.Curve.AddPoint(Vector2.Zero);
		Path.Curve.AddPoint(directionToTarget * WeaponRangeLength);
		Path.Curve.AddPoint(Vector2.Zero);
	}

	private void Slash() {
		throw new NotImplementedException();
	}

	public void Attack(Vector2 target) {
		Target = target;
		weaponSprite.Visible = true;
		HitNodes = [];
		IsAttacking = true;
		IsWeaponOnCooldown = true;

		AttackMethod();
		attackCooldownTimer.Start();

		foreach(Node2D body in weaponRange.GetOverlappingBodies()){
			OnAttackHittingSomething(body);
		}
	}

	private void OnAttackHittingSomething(Node2D body) {
		if(!ShouldDamage(body)) {
			return;
		}

		
		HitNodes.Add(body);
		(body as EntityBase).Damage(weaponDamage);
	}

	private bool ShouldDamage(Node2D body) {
		return IsAttacking
		&& body is EntityBase
		&& !HitNodes.Contains(body)
		&& TypesToAffect.Contains((body as EntityBase).EntityType);
	}

	private void AttackCoolDownOver() {
		IsWeaponOnCooldown = false;
	}

	private void AttackFinish() {
		IsAttacking = false;
		PathProgress.Progress = 0;
		weaponSprite.Visible = false;
	}
}
