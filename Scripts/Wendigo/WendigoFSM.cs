using Godot;
using System.Collections.Generic;

public partial class WendigoFSM : Node
{
	public static WendigoFSM wendigoFSM;
	[Export] public CharacterBody2D wendigoBody;
	[Export] public AnimationPlayer animPlayer;
	[Export] public Sprite2D sprite;

	public enum LerpModes
	{
		X_Only,
		Y_Only,
		Both
	}

	public enum WENDIGO_STATES
	{
		IDLE,
		HORIZONTAL_DASH,
		VERTICAL_DASH,
		EARTHQUAKE,
		ATTACK,
		JUMP,
		GO_TO_POINT,
		THROW,
		FOUR_POINT_DASH
	}
	private WENDIGO_STATES Enum_state = WENDIGO_STATES.IDLE;
	private static WENDIGO_STATES _previousStateEnum;
	private WENDIGO_STATES _currentStateEnum;

	private State _currentState;
	[Export] private WendigoIdleState wendigoIdleState;
	[Export] private WendigoHorizontalDashState wendigoHorizontalDashState;
	[Export] private WendigoJumpState wendigoJumpState;
	[Export] private WendigoAttackState wendigoAttackState;
	[Export] private WendigoGoToPointState wendigoGoToPointState;
	[Export] private WendigoThrowState wendigoThrowState;
	[Export] private WendigoFourPointDashState wendigoFourPointDashState;
	[Export] private WendigoVerticalDashState wendigoVerticalDashState;

    public static IDictionary<StringName, int> message = new Dictionary<StringName, int>();
	private static RandomNumberGenerator rng = new RandomNumberGenerator();
	private Stack<int> PreviousStatesStack = new Stack<int>();

	public static Node2D player;
	private bool isFacingRight = true;

    public override void _Ready()
    {
		isFacingRight = true;
        if (wendigoFSM == null)
		{
			wendigoFSM = this;
		}

		_currentStateEnum = WENDIGO_STATES.IDLE;
		_currentState = wendigoIdleState;
		_currentState.Enter();
    }

    public override void _Process(double delta)
    {
        _currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate(delta);
    }

	#region Change States Methods
	// Transitions to the state passed into the method
    public void TransitionState(WENDIGO_STATES state, IDictionary<Godot.StringName, int> message = null)
	{
		if (Enum_state != state)
		{
			Enum_state = state;
			_previousStateEnum = _currentStateEnum;
			_currentState.Exit();
			// Assign new state
			_currentState = GetWendigoState(state);
			_currentStateEnum = state;
			// Start new state
			_currentState.Enter(message);
		}
	}

	// Transitions to 1 random state present in the int array passed in
	public void RandomTransitionState(int[] states, IDictionary<StringName, int> message = null)
	{
		// **TODO Pass a previous states variable to prevent states from repeating
		// **Maybe using a stack and if a new state that is not head of stack is chosen, pop it

		// Stores length of array of states passed in
		int length = states.Length;
		int index;

		// Creates a new Random Number Generator Object
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();

		// Get a random index to select the state to transition to
		index = randomNumberGenerator.RandiRange(0, length-1);

		// Transition to the random state selected by the rng
		TransitionState((WENDIGO_STATES)states[index], message);
	}

	// Returns appropriate state object according to the enum value passed into the method
	private State GetWendigoState(WENDIGO_STATES state)
	{
		switch(state)
		{
			case WENDIGO_STATES.IDLE:
				return wendigoIdleState;
			
			case WENDIGO_STATES.HORIZONTAL_DASH:
				return wendigoHorizontalDashState;

			case WENDIGO_STATES.JUMP:
				return wendigoJumpState;

			case WENDIGO_STATES.ATTACK:
				return wendigoAttackState;

			case WENDIGO_STATES.GO_TO_POINT:
				return wendigoGoToPointState;

			case WENDIGO_STATES.THROW:
				return wendigoThrowState;

			case WENDIGO_STATES.FOUR_POINT_DASH:
				return wendigoFourPointDashState;

			case WENDIGO_STATES.VERTICAL_DASH:
				return wendigoVerticalDashState;

			default:
				return wendigoIdleState;
		}
	}

	public static int GetPreviousState()
	{
		return (int)_previousStateEnum;
	}
	#endregion

	#region Wendigo Look At and Direction Methods
	public void LookAtPlayer()
	{
		if(player==null)
		{
			player = PlayerMovement.player;
		}
		if (isFacingRight && wendigoBody.Position.X > player.Position.X 
			|| !isFacingRight && wendigoBody.Position.X < player.Position.X)
		{
			isFacingRight = !isFacingRight;
			Vector2 direction = wendigoBody.Scale;
			direction.X *= -1;
			wendigoBody.Scale = direction;
		}
	}

	public float GetFacingDirection()
	{
		if(isFacingRight)
			return 1;
		else
			return -1;
	}
	#endregion

	#region Movement Lerp Methods
	public static void LerpToPoint(Node2D node, Vector2 targetPos, float weight, bool GlobalPosition=true, LerpModes mode=LerpModes.X_Only)
	{
		Vector2 pos;

		if(GlobalPosition)
			pos = node.GlobalPosition;
		else
			pos = node.Position;

		if(mode == LerpModes.X_Only || mode == LerpModes.Both)
			pos.X = Mathf.Lerp(pos.X ,targetPos.X, weight);

		if(mode == LerpModes.Y_Only || mode == LerpModes.Both)
			pos.Y = Mathf.Lerp(pos.Y ,targetPos.Y, weight);	

		if(GlobalPosition)
			node.GlobalPosition = pos;
		else
			node.Position = pos;
	}
	#endregion

	#region Generate State Message Methods
	// Returns 2 values which would determine which point to move to for the GoToPoint State, Throw State and FourPointDash State
	public static int GetPoint()
	{
		return rng.RandiRange(0,1);
	}
	
	#endregion

	#region Wendigo Boss CharacterBody2D related methods

	#region Setters
	// Sets values for X and Y values of Velocity Vector of Wendigo Character Body
	public static void SetWendigoBodyX(float value)
	{
		Vector2 vel = wendigoFSM.wendigoBody.Velocity;
		vel.X = value;
		wendigoFSM.wendigoBody.Velocity = vel;
	}

	public static void SetWendigoBodyY(float value)
	{
		Vector2 vel = wendigoFSM.wendigoBody.Velocity;
		vel.Y = value;
		wendigoFSM.wendigoBody.Velocity = vel;
	}
	#endregion

	public static void CallMoveAndSlide()
	{
		wendigoFSM.wendigoBody.MoveAndSlide();
	}

	#region Getters
	// Returns CharacterBody2D Velocity Vector
	public static Vector2 GetWendigoVel()
	{
		return wendigoFSM.wendigoBody.Velocity;
	}

	public CharacterBody2D GetWendigoCharacterBody2D()
	{
		return wendigoBody;
	}
	#endregion
	#endregion
}
