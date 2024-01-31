using Godot;
using System;
using System.Collections.Generic;

public partial class BasicSpell : Spell
{
	Vector2 Target;
	private Path2D Path;
	private PathFollow2D PathProgress;
	[Export]
	float ProjectileSpeed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(CurrentState.Equals(SpellState.Cast)){
			PathProgress.Progress += (float)delta * ProjectileSpeed;

			if(PathProgress.ProgressRatio >= .99f)
			{
				QueueFree();
			}
		}
	}

	public override void Initialize(EntityBase caster)
	{
		base.Initialize(caster);
		EnemyEffects = new Godot.Collections.Array<EntityEffect>(){
			new EntityEffect() { EffectedStat = Stat.CurrentHealth, buff = false, Duration = 0, Instant = true, Modifier = -25 }
		};

		Path = GetNode<Path2D>("Path2D");
		PathProgress = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		GlobalPosition = Caster.GlobalPosition;

		CurrentState = SpellState.Cast;
		PlayAnimation(CurrentState);
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

	protected override void OnCollisionEnter(Node2D body)
	{
		base.OnCollisionEnter(body);

		if(NumberHit > 0){
			QueueFree();
		}
	}
}
