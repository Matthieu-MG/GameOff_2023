using Godot;
using System;
using System.Collections.Generic;

public partial class State : Node
{
	public virtual void Enter(IDictionary<Godot.StringName, int> message = null){}

	public virtual void Update(double delta){}

	public virtual void PhysicsUpdate(double delta){}

	public virtual void Exit(){}
}
