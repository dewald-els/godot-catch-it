using Godot;
using System;

public partial class Player : CharacterBody2D
{

    public AnimatedSprite2D AnimatedSprite;
    public Area2D BombPickupArea;
    private Timer CoyoteTimer;

    private Vector2 StartPosition;

    // State
    private Boolean HasBomb = false;
    private Boolean PickingUpBomb = false;
    private Boolean DroppingBomb = false;

    private Boolean CanMove = true;

    // Movement
    private const float VELOCITY_X_LERP = -40.0f;
    private const float MAX_X_SPEED = 200.0f;
    private const float ACCELERATION_X = 2500.0f;

    private const float AIRBORNE_ACCELERATION_X = 3000.0f;
    private float Direction = 1;

    // Jump
    private int JumpPower = 375;
    private Boolean WasOnFloor = false;
    private const int JUMP_DRAG_MULTIPLIER = 5;



    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        BombPickupArea = GetNode<Area2D>("BombPickupArea");
        CoyoteTimer = GetNode<Timer>("CoyoteTimer");

        BombPickupArea.BodyEntered += OnBombPickupAreaEntered;
        AnimatedSprite.AnimationFinished += OnAnimatedSpriteAnimationFinished;

        StartPosition = Position;

        SignalBus.Instance.Connect("PlayerDroppedBomb", new Callable(this, "BombDropped"));
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        Vector2 inputVector = GetInputVector();

        if (CanMove)
        {
            Velocity = ProcessMove(inputVector, (float)delta);
            Velocity = ProcessJump(inputVector);
        }

        Velocity = ApplyGravity((float)delta);

        WasOnFloor = IsOnFloor();

        MoveAndSlide();
        PlayAnimation();
        LookAtWalkingDirection(inputVector);

        if (WasOnFloor && !IsOnFloor())
        {
            CoyoteTimer.Start();
        }
    }

    public void LookAtWalkingDirection(Vector2 inputVector)
    {
        if (inputVector.X == 0)
            return;

        Vector2 scale = Scale;
        scale.X = Scale.Y * -inputVector.X;
        Scale = scale;
    }

    public void PlayAnimation()
    {

        if (IsOnFloor())
        {
            if (PickingUpBomb)
            {
                AnimatedSprite.Play("PickUpBomb");
            }
            else if (DroppingBomb)
            {
                AnimatedSprite.Play("DropBomb");
            }
            else if (Velocity.X != 0 && HasBomb)
            {
                AnimatedSprite.Play("RunWithBomb");
            }
            else if (Mathf.Abs(Velocity.X) > 0.1)
            {
                AnimatedSprite.Play("Run");
            }
            else if (HasBomb == true)
            {
                AnimatedSprite.Play("IdleWithBomb");
            }
            else
            {
                AnimatedSprite.Play("Idle");
            }
        }
        else if (HasBomb)
        {
            AnimatedSprite.Play("JumpWithBomb");
        }
        else
        {
            AnimatedSprite.Play("Jump");
        }
    }

    private Vector2 ProcessMove(Vector2 moveVector, float delta)
    {
        Vector2 velocity = Velocity;
        if (IsOnFloor())
        {
            velocity.X += moveVector.X * ACCELERATION_X * delta;
        }
        else
        {
            velocity.X += moveVector.X * AIRBORNE_ACCELERATION_X * delta;
        }

        if (moveVector.X == 0)
        {
            velocity.X = Mathf.Lerp(0, velocity.X, Mathf.Pow(2, VELOCITY_X_LERP * delta));
            // See here: https://www.gamedeveloper.com/programming/improved-lerp-smoothing-
        }

        velocity.X = Mathf.Clamp(velocity.X, -MAX_X_SPEED, MAX_X_SPEED);

        return velocity;
    }

    private Vector2 ProcessJump(Vector2 moveVector)
    {
        Vector2 velocity = Velocity;

        if (moveVector.Y < 0 && (IsOnFloor() || IsCoyoteTimerRunning()))
        {
            velocity.Y = moveVector.Y * JumpPower;
            CoyoteTimer.Stop();
        }

        return velocity;
    }

    private Vector2 ApplyGravity(float delta)
    {
        Vector2 velocity = Velocity;
        if (velocity.Y < 0 && !Input.IsActionPressed("player_jump"))
        {
            velocity.Y += gravity * JUMP_DRAG_MULTIPLIER * delta;
        }
        else
        {
            velocity.Y += gravity * delta;
        }
        return velocity;
    }

    private Vector2 GetInputVector()
    {
        // MOVE ---------------------------------------------------------------------
        Vector2 moveVector = Vector2.Zero;
        moveVector.X = Input.GetActionStrength("player_right") - Input.GetActionStrength("player_left");

        Boolean didPlayerJump = Input.IsActionJustPressed("player_jump");

        if (didPlayerJump)
        {

            moveVector.Y = -1; // -1 because we want to go "Up"
        }
        else
        {
            moveVector.Y = 0; // no jumps here
        }

        return moveVector;
    }

    private Boolean IsCoyoteTimerRunning()
    {
        return !CoyoteTimer.IsStopped();
    }

    // Events
    private void BombDropped()
    {
        if (HasBomb)
        {
            HasBomb = false;
            DroppingBomb = true;
            CanMove = false;
            Velocity = Vector2.Zero;
        }
    }
    private void OnAnimatedSpriteAnimationFinished()
    {
        if (PickingUpBomb)
        {
            HasBomb = true;
            PickingUpBomb = false;
        }
        else if (DroppingBomb)
        {
            DroppingBomb = false;
        }

        CanMove = true;
    }
    private void OnBombPickupAreaEntered(Node2D body)
    {
        if (!HasBomb && body is Bomb bomb && bomb.IsExploding == false)
        {
            PickingUpBomb = true;
            CanMove = false;
            Velocity = Vector2.Zero;
            SignalBus.Instance.EmitSignal("PlayerPickedUpBomb", body);
        }
    }
}