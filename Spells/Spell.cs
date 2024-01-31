using Godot;
using System;
using System.Collections.Generic;

public enum SpellState
{
	Charging,
	Cast,
	Hit
}

[GlobalClass]
public abstract partial class Spell : Node2D
{
	[ExportGroup("Spell UI Properties")]
	[Export]
	protected string SpellName;
	[Export]
	protected Sprite2D IconImage;
	[Export(PropertyHint.MultilineText)]
	protected string Description;
	[ExportGroup("Spell Effects")]
	[Export]
	protected float Cooldown;
	[Export]
	protected int ManaCost;
	[Export]
	public Godot.Collections.Array<EntityEffect> AllyEffects = new Godot.Collections.Array<EntityEffect>();
	[Export]
	public Godot.Collections.Array<EntityEffect> EnemyEffects = new Godot.Collections.Array<EntityEffect>();
	[Export]
	public Godot.Collections.Array<EntityEffect> CasterEffects = new Godot.Collections.Array<EntityEffect>();
	protected EntityBase Caster;
	protected List<EntityTag> Friendlies = [];
	protected List<EntityTag> Enemies = [];
	protected SpellState CurrentState;


	protected AnimatedSprite2D SpellAnimation;
	protected AudioStreamPlayer2D SpellAudioPlayer;
	protected int ChargingTime;
	protected int NumberHit;

	// Called when the node enters the scene tree for the first time.
	public void BasicInit()
	{
		SpellAnimation = GetNode<AnimatedSprite2D>("Animation");
		SpellAudioPlayer = GetNode<AudioStreamPlayer2D>("Sound");

		SpellAnimation.AnimationFinished += OnAnimationFinished;
		NumberHit = 0;
	}

	public virtual void Initialize(EntityBase caster)
	{
		Caster = caster;

		switch(caster.EntityType)
		{
			case EntityTag.Hero :
				Friendlies = new List<EntityTag>() { EntityTag.Adversary };
				Enemies = new List<EntityTag>() { EntityTag.Player, EntityTag.Zombie };
				break;
			case EntityTag.Adversary :
				Friendlies = new List<EntityTag>() { EntityTag.Hero };
				Enemies = new List<EntityTag>() { EntityTag.Player, EntityTag.Zombie };
				break;
			case EntityTag.Player :
				Friendlies = new List<EntityTag>() { EntityTag.Zombie };
				Enemies = new List<EntityTag>() { EntityTag.Hero, EntityTag.Adversary };
				break;
			case EntityTag.Zombie :
				Friendlies = new List<EntityTag>() { EntityTag.Player };
				Enemies = new List<EntityTag>() { EntityTag.Hero, EntityTag.Adversary };
				break;
		}
	}

	public virtual void Activate()
	{
		if(CasterEffects.Count > 0){
			Caster.ApplyEffects(CasterEffects);
		}
	}

	protected virtual void OnAnimationFinished()
	{
		if(CurrentState.Equals(SpellState.Hit)){
			QueueFree();
		}
	}

	protected void PlayAudio()
	{
		SpellAudioPlayer.Play();
	}

	protected void PlayAnimation()
	{
		SpellAnimation.Play(CurrentState.ToString());
	}

	protected void AffectEntity(EntityBase body)
	{
		if(Friendlies.Contains(body.EntityType) && AllyEffects.Count > 0){
			body.ApplyEffects(AllyEffects);
			NumberHit += 1;
		} else if(Enemies.Contains(body.EntityType) && EnemyEffects.Count > 0){
			body.ApplyEffects(EnemyEffects);
			NumberHit += 1;
		}
	}
}
