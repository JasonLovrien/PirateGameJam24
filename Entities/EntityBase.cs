using Godot;
using System;
using System.Collections;
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
		{ Stat.Shield, 0 },
		{ Stat.Speed, 300 }
	};
	protected Dictionary<Stat, int> ModifierStats = new Dictionary<Stat, int>();
	protected List<Timer> EffectTimers;
	public EntityTag EntityType;

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

	public virtual void ApplyEffects(Godot.Collections.Array<EntityEffect> effects)
	{
		foreach(EntityEffect effect in effects){
			ApplyEffect(effect);
		}
	}

	//Apply Effect could be implemented here since every creature will
	//have the same stats to effect
	protected virtual void ApplyEffect(EntityEffect effect)
	{
		if(effect.EffectedStat.Equals(Stat.CurrentHealth) && effect.Modifier < 0 && BaseStats[Stat.Shield] > 0 && effect.Instant)
		{
			BaseStats[Stat.Shield] += Mathf.RoundToInt(effect.Modifier);
			if(BaseStats[Stat.Shield] < 0){
				effect.Modifier = BaseStats[Stat.Shield];
				BaseStats[Stat.Shield] = 0;
			} else {
				return;
			}
		}

		if(effect.Instant)
		{
			float modifierValue = effect.Modifier;

			//Calculate percent if need be
			if(effect.Percent){
				modifierValue = GetPercentageValue(effect.UsedStat, effect.Modifier);
			}
			BaseStats[effect.EffectedStat] += Mathf.RoundToInt(modifierValue);

			//Prevent health and mana from going over the max or under 0
			switch(effect.EffectedStat)
			{
				case Stat.CurrentHealth :
					if(BaseStats[effect.EffectedStat] > BaseStats[Stat.MaxHealth])
					{
						BaseStats[effect.EffectedStat] = BaseStats[Stat.MaxHealth];
					} else if (BaseStats[effect.EffectedStat] < 0){
						BaseStats[effect.EffectedStat] = 0;
					}
					break;
				case Stat.CurrentMana :
					if(BaseStats[effect.EffectedStat] > BaseStats[Stat.MaxMana])
					{
						BaseStats[effect.EffectedStat] = BaseStats[Stat.MaxMana];
					} else if (BaseStats[effect.EffectedStat] < 0){
						BaseStats[effect.EffectedStat] = 0;
					}
					break;
			}
		} else
		{
			ModifierStats[effect.EffectedStat] += Mathf.RoundToInt(effect.Modifier);
			Timer EffectTimer = new Timer
			{
				OneShot = true,
				WaitTime = effect.Duration
			};
			EffectTimer.Timeout += ()=> {
				ModifierStats[effect.EffectedStat] -= Mathf.RoundToInt(effect.Modifier);
				EffectTimers.Remove(EffectTimer);
			};

			EffectTimers.Add(EffectTimer);
		}
		
		if(BaseStats[Stat.CurrentHealth] <= 0) {
			Die();
		}
	}

	private float GetPercentageValue(Stat effectedStat, float modifier)
	{
		return BaseStats[effectedStat] * (modifier/100);
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
