using Godot;
using System;

public partial class CasterSpell : Spell
{
    public override void Initialize(EntityBase caster)
    {
        BasicInit();
        base.Initialize(caster);

        Activate();
    }
}
