using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Player : EntityBase
{
	private List<Spell> AbilityList;

	private Skillshot skillShot;

	protected override void Initialize()
	{
		EntityType = EntityTag.Player;
		skillShot = GetNode<Skillshot>("Skillshot");
		return;
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("primary_button"))
		{
			Attack();
		}
	}
	
	protected override Vector2 GetNormalizedMovementDirection()
	{
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		return direction;
	}

	private void Attack() {
		if(!skillShot.IsWeaponOnCooldown){
			skillShot.Attack(GetGlobalMousePosition());
		}
	}

	public override void ApplyEffect(Effect effect)
	{
		//If the effect is damage, subtract it from shield before focusing on current health
		if(effect.EffectedStat.Equals(Stat.CurrentHealth) && effect.Modifier < 0 && BaseStats[Stat.Shield] > 0)
		{
			BaseStats[Stat.Shield] -= effect.Modifier;
			effect.Modifier = BaseStats[Stat.Shield];
			if(BaseStats[Stat.Shield] < 0){
				BaseStats[Stat.Shield] = 0;
			}
		}

		base.ApplyEffect(effect);
	}
}
