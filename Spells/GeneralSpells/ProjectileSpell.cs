using Godot;
using System;
using System.Collections.Generic;

public partial class ProjectileSpell : Spell
{
	Vector2 Target;
	protected Area2D Range2D;
	protected Area2D Hitbox2D;
	private Path2D Path;
	private PathFollow2D PathProgress;
	private float Range;
	[Export]
	float ProjectileSpeed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(CurrentState.Equals(SpellState.Cast)){
			PathProgress.Progress += (float)delta * ProjectileSpeed;

			if(PathProgress.ProgressRatio >= .99f)
			{
				CurrentState = SpellState.Hit;
				Hitbox2D.SetProcess(false);
				PlayAnimation();
			}
		}
	}

	public override void Initialize(EntityBase caster)
	{
		base.Initialize(caster);

		Hitbox2D = GetNode<Area2D>("Hitbox");
		SpellAnimation = Hitbox2D.GetNode<AnimatedSprite2D>("Animation");
		SpellAudioPlayer = Hitbox2D.GetNode<AudioStreamPlayer2D>("Sound");

		Hitbox2D.BodyEntered += OnCollisionEnter;

		Range2D = GetNode<Area2D>("Range");
		Range = (Range2D.GetChild<CollisionShape2D>(0).Shape as CircleShape2D).Radius
		- Hitbox2D.GetChild<CollisionShape2D>(0).Shape.GetRect().Size.X;

		Path = GetNode<Path2D>("Path2D");
		PathProgress = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		GlobalPosition = Caster.GlobalPosition;

		CurrentState = SpellState.Cast;
		PlayAnimation();
		Activate();
	}

	public override void Activate()
	{
		Target = GetGlobalMousePosition();

		Vector2 directionToTarget = (Target - Caster.GlobalPosition).Normalized();
		Hitbox2D.Rotation = directionToTarget.Angle();
		PathProgress.Rotates = false;
		Path.Curve.ClearPoints();
		Path.Curve.AddPoint(Vector2.Zero);
		Path.Curve.AddPoint(directionToTarget * Range);

		Hitbox2D.Reparent(PathProgress);
	}

	private void OnCollisionEnter(Node2D body)
	{
		if(!(body is EntityBase)){
			return;
		}

		AffectEntity(body as EntityBase);

		if(NumberHit > 0){
			CurrentState = SpellState.Hit;
			Hitbox2D.SetProcess(false);
			PlayAnimation();
		}
	}
}
