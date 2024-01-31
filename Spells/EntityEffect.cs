using Godot;

[GlobalClass]
public partial class EntityEffect : Resource
{
    #region VARIABLES
    [Export]
    public Stat EffectedStat;
    [Export]
    public Stat UsedStat;
    [Export]
	public float Modifier;
    [Export]
	public float Duration;
    [Export]
	public bool Instant;
    [Export]
	public bool Percent;
    #endregion
}
