using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player : EntityBase
{
	private CustomEvents _CustomEvents;
	[Export]
	private string SpellPath = "res://Spells/";
	[Export]
	private Godot.Collections.Array<string> AbilityList;
	private Skillshot skillShot;
	private int AbilityIndex = 0;

	protected override void Initialize()
	{
		BaseStats[Stat.CurrentHealth] = 300;
		EntityType = EntityTag.Player;
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");

		_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdatePlayerHealth,
		BaseStats[Stat.MaxHealth], BaseStats[Stat.CurrentHealth]);
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("primary_button"))
		{
			Attack();
		}
		if(@event.IsActionReleased("scroll_up"))
		{
			ChangeSpellIndex(1);
		}
		if(@event.IsActionReleased("scroll_down")){
			ChangeSpellIndex(-1);
		}
	}

	private void ChangeSpellIndex(int direction){
		AbilityIndex += direction;
		if(AbilityIndex >= AbilityList.Count){
			AbilityIndex = 0;
		} else if(AbilityIndex < 0) {
			AbilityIndex = AbilityList.Count-1;
		}
	}
	
	protected override Vector2 GetNormalizedMovementDirection()
	{
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		return direction;
	}

	private void Attack() {
		Spell spell = ResourceLoader.Load<PackedScene>(SpellPath+AbilityList[AbilityIndex]+".tscn").Instantiate() as Spell;
		GetTree().Root.AddChild(spell);
		spell.Initialize(this);
	}

	protected override void ApplyEffect(EntityEffect effect)
	{
		base.ApplyEffect(effect);
		if(effect.EffectedStat.Equals(Stat.CurrentHealth))
		{
			GD.Print("changing health values");
			//Update this later to incorporate StatModifiers
			_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdatePlayerHealth,
			BaseStats[Stat.MaxHealth], BaseStats[Stat.CurrentHealth]);
		}
	}

	public override void Damage(int damage)
	{
		base.Damage(damage);
		//Update this later to incorporate StatModifiers
		_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdatePlayerHealth, BaseStats[Stat.MaxHealth], BaseStats[Stat.CurrentHealth]);
	}
}
