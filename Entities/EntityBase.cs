using Godot;
using System;
using System.Collections.Generic;


public enum Stat
{
	MaxHealth,
	CurrentHealth,
	MaxMana,
	CurrentMana,
	Defense,
	Attack,
	Shield,
	Speed
}

public enum AnimationState
{
	Moving,
	Attacking,
	Casting,
	Damage,
	Death,
	Idle
}

public enum EntityTag{
	Player,
	Zombie,
	Adversary,
	Hero
}

public abstract partial class EntityBase : CharacterBody2D
{
	//Defined here just so I remember how to define dictionarys inline
	protected Dictionary<Stat, int> BaseStats = new Dictionary<Stat, int>()
	{
		{ Stat.MaxHealth, 300 },
		{ Stat.CurrentHealth, 100 },
		{ Stat.MaxMana, 150 },
		{ Stat.CurrentMana, 150 },
		{ Stat.Defense, 20 },
		{ Stat.Attack, 25 },
		{ Stat.Shield, 150 },
		{ Stat.Speed, 300 }
	};
	protected Dictionary<Stat, int> ModifierStats = new Dictionary<Stat, int>();
	protected List<Timer> EffectTimers;
	protected EntityTag EntityType;

	protected AnimatedSprite2D AnimatedSprite;
	protected AnimationState CurrentState;
	protected bool left;

	protected abstract void Initialize();
	protected abstract Vector2 GetNormalizedMovementDirection();
	protected virtual int GetMovementSpeed()
	{
		return BaseStats[Stat.Speed];
	}

	protected int deceleration = 300;

	public override void _Ready()
	{
		AnimatedSprite = GetNode<AnimatedSprite2D>("Animation");
		EffectTimers = [];
		Initialize();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get the input direction and handle the movement/deceleration.
		Move();
		UpdateAnimation();
	}

	public virtual void Damage(int damage)
	{
		BaseStats[Stat.CurrentHealth] -= damage;
		
		if(BaseStats[Stat.CurrentHealth] <= 0) {
			Die();
		}
	}

	//Apply Effect could be implemented here since every creature will
	//have the same stats to effect
	public virtual void ApplyEffect(Effect effect)
	{
		if(effect.Instant)
		{
			BaseStats[effect.EffectedStat] += effect.Modifier;
		} else
		{
			ModifierStats[effect.EffectedStat] += effect.Modifier;
			Timer EffectTimer = new Timer
			{
				OneShot = true,
				WaitTime = effect.Duration
			};
			EffectTimer.Timeout += ()=> {
				ModifierStats[effect.EffectedStat] -= effect.Modifier;
				EffectTimers.Remove(EffectTimer);
			};

			EffectTimers.Add(EffectTimer);
		}
	}

	protected virtual void Die()
	{
		foreach(Timer timer in EffectTimers){
			timer.QueueFree();
		}
		QueueFree();
	}

	protected void Move()
	{
		Vector2 direction = GetNormalizedMovementDirection();
		Vector2 velocity = Velocity;
		
		if (direction != Vector2.Zero)
		{
			velocity = direction * GetMovementSpeed();
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, deceleration);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, deceleration);
		}
		
		//for future reference, this should probably be moved since it's not really a part of "moving"
		SetAnimationState(direction);

		Velocity = velocity;
		MoveAndSlide();
	}

	private void SetAnimationState(Vector2 direction)
	{
		CurrentState = Velocity == Vector2.Zero ? AnimationState.Idle : AnimationState.Moving;

		//not using velocity so if character is knocked back, they still face the same way
		if(direction.X > 0.1) {
			left = false;
		}
		else if(direction.X < -0.1) {
			left = true;
		}
	}

	protected void UpdateAnimation()
	{
		AnimatedSprite.FlipH = !left;
		AnimatedSprite.Play(CurrentState.ToString());
	}
}
