using Godot;
using System;
using System.Collections.Generic;

public enum SpellState
{
	Charging,
	Cast,
	Hit
}

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
	protected Area2D Hitbox2D;
	protected Area2D Range2D;
	protected float Range;
	protected int ChargingTime;
	protected int NumberHit;

	// Called when the node enters the scene tree for the first time.
	public void BasicInit()
	{
		Hitbox2D = GetNode<Area2D>("Hitbox");
		Range2D = GetNode<Area2D>("Range");
		SpellAnimation = Hitbox2D.GetNode<AnimatedSprite2D>("Animation");
		SpellAudioPlayer = Hitbox2D.GetNode<AudioStreamPlayer2D>("Sound");

		Hitbox2D.BodyEntered += OnCollisionEnter;
		NumberHit = 0;

		Range = (Range2D.GetChild<CollisionShape2D>(0).Shape as CircleShape2D).Radius
		- Hitbox2D.GetChild<CollisionShape2D>(0).Shape.GetRect().Size.X;
	}

	public virtual void Initialize(EntityBase caster)
	{
		BasicInit();

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

	protected virtual void OnCollisionEnter(Node2D body)
	{
		if(!(body is EntityBase)){
			return;
		}

		AffectEntity(body as EntityBase);
	}

	protected void PlayAudio()
	{
		SpellAudioPlayer.Play();
	}

	protected void PlayAnimation(SpellState State)
	{
		SpellAnimation.Play(State.ToString());
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
