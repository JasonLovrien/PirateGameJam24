using Godot;
using System;

public partial class CustomEvents : Node
{
	[Signal]public delegate void UpdateZombieCountEventHandler(int zombies);
	[Signal]public delegate void UpdateSpellNameEventHandler(string spellName);
	[Signal]public delegate void UpdatePlayerHealthEventHandler(int max, int current);
}
